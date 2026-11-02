using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Gps.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace mapsence.Infrastructure.Import;

/// <summary>
/// TLS-сервер импорта: ReceiverApp подключается как клиент и передаёт строки по одной.
/// Подтверждение уходит только после записи в БД. При ошибке соединение закрывается,
/// и экспорт повторяет отправку этой строки.
/// </summary>
public class SocketImportServer(
    GeoDataImportRunner importer,
    IConfiguration configuration,
    IHostEnvironment environment) : BackgroundService
{
    private static readonly TimeSpan HandshakeTimeout = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using X509Certificate2 certificate = TlsStreams.LoadServerCertificate(
            Path.Combine(environment.ContentRootPath, configuration["Tls:CertPath"]!),
            Path.Combine(environment.ContentRootPath, configuration["Tls:KeyPath"]!));

        int port = int.Parse(configuration["Import:SocketPort"]!);
        using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, port));
        socket.Listen();
        Console.WriteLine($"[socket] Ожидание подключений по TLS на порту {port}");

        while (!stoppingToken.IsCancellationRequested)
        {
            Socket client = await socket.AcceptAsync(stoppingToken);
            Console.WriteLine($"[socket] Подключение: {client.RemoteEndPoint}");
            _ = Task.Run(() => ProcessClientAsync(client, certificate, stoppingToken), stoppingToken);
        }
    }

    private async Task ProcessClientAsync(Socket client, X509Certificate2 certificate, CancellationToken token)
    {
        await using var inner = new NetworkStream(client, ownsSocket: true);

        try
        {
            using var handshakeTimeout = CancellationTokenSource.CreateLinkedTokenSource(token);
            handshakeTimeout.CancelAfter(HandshakeTimeout);
            await using var stream = await TlsStreams.AuthenticateAsServerAsync(inner, certificate, handshakeTimeout.Token);

            while (!token.IsCancellationRequested)
            {
                var point = RawPointJson.Deserialize(await Frame.ReadAsync(stream, token));
                bool imported = await importer.ImportAsync(point, token);
                await Frame.WriteAckAsync(stream, token);
                Console.WriteLine($"[socket] Строка #{point.SourceId} {(imported ? "импортирована" : "уже есть в БД")}");
            }
        }
        catch (EndOfStreamException)
        {
            Console.WriteLine("[socket] Экспорт отключился");
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[socket] Ошибка, соединение закрыто: {ex.Message}");
        }
    }
}
