using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Receiver_server.Database;

namespace Receiver_server.Receiver;

public class GpsEndPoint(RawPointStore store, IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int port = configuration.GetValue<int>("Sensor:Port");
        using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, port));
        socket.Listen();
        Console.WriteLine($"Сервер запущен на порту {port}. Ожидание подключений...");

        while (!stoppingToken.IsCancellationRequested)
        {
            Socket client = await socket.AcceptAsync(stoppingToken);
            Console.WriteLine($"Адрес подключенного клиента: {client.RemoteEndPoint}");
            _ = Task.Run(() => ProcessClientAsync(client, stoppingToken), stoppingToken);
        }
    }

    private async Task ProcessClientAsync(Socket client, CancellationToken token)
    {
        var session = new SessionInfo(((IPEndPoint)client.RemoteEndPoint!).Address.ToString(), DateTime.UtcNow);
        await using var stream = new NetworkStream(client, ownsSocket: true);
        string imei = "?";

        try
        {
            var handshake = new byte[Parser.HandshakeHeaderLength];
            await stream.ReadExactlyAsync(handshake, token);
            var info = new byte[handshake[^1]];
            await stream.ReadExactlyAsync(info, token);

            var device = Parser.ParseHandshake(handshake, info);
            imei = device.Imei;
            Console.WriteLine($"IMEI: {imei} ({device.Manufacturer} {device.Model}, прошивка {device.Firmware})");

            var header = new byte[3];
            while (!token.IsCancellationRequested)
            {
                // Пакет читается целиком по длине из байта 2,
                // поэтому склеенные или разрезанные TCP-сегменты не теряют данные
                await stream.ReadExactlyAsync(header, token);
                if (header[2] < Parser.PacketLength)
                    throw new InvalidDataException($"Некорректная длина пакета: {header[2]}");

                var packet = new byte[header[2]];
                header.CopyTo(packet, 0);
                await stream.ReadExactlyAsync(packet.AsMemory(header.Length), token);

                var point = Parser.ParseGeodata(packet, device, session);
                point.SourceId = await store.InsertAsync(point);
                Console.WriteLine($"[{imei}] #{point.SourceId} {point.EventName}: {point.Latitude:F6} {point.Longitude:F6}, " +
                                  $"{point.Speed} км/ч, курс {point.Course}, высота {point.Height}, " +
                                  $"спутников {point.SatelliteCount}, {point.BatteryMv} мВ");
            }
        }
        catch (EndOfStreamException)
        {
            Console.WriteLine($"Клиент {imei} отключился");
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка клиента {imei}: {ex.Message}");
        }
    }
}
