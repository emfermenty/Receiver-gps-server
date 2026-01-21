using System.Net;
using System.Net.Sockets;
using System.Text;
using Receiver_server.Database;

namespace Receiver_server.Receiver;

public class GpsEndPoint(DataBaseHandler handler, TrackStateService stateManager)
{
    public async Task OpenEndPoint()
    {
        try
        {
            List<Socket> clients = new List<Socket>();
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 40433);
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                clients.Add(await socket.AcceptAsync());
                Console.WriteLine($"Адрес подключенного клиента: {clients.Last().RemoteEndPoint}");
                _ = Task.Run(async () => await ProcessClientAsync(clients.Last()));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private async Task ProcessClientAsync(Socket client)
    {
        var buffer = new byte[100];
        List<byte> packet = new List<byte>();

        int recievedBytesCount = await client.ReceiveAsync(buffer);

        string IMEI = Encoding.UTF8.GetString(buffer.Skip(4).Take(15).ToArray());
        int HandleIMEI = await handler.CheckAvailableImeiAsync(IMEI);
        Console.WriteLine($"IMEI: {IMEI}");
        if (HandleIMEI != -1)
        {
            Console.WriteLine("Imei недоступен");
            client.Close();
        }
        while (true)
        {
            recievedBytesCount = await client.ReceiveAsync(buffer);
            if (recievedBytesCount == 0)
            {
                Console.WriteLine("Клиент отключился");
                break;
            }

            packet.AddRange(buffer.Take(recievedBytesCount));

            int i = 0;
            for (; i + packet[2 + i] < packet.Count; i += packet[2 + i])
            {
                Console.WriteLine(IMEI);
                await Parser.ParseGeodata(packet.Skip(i).Take(packet[2 + i]).ToList(), 4, false);

                if (stateManager.HasAnyActiveTrack(IMEI))
                {
                    await Parser.ParseGeodata(packet.Skip(i).Take(packet[2 + i]).ToList(), 4, true);
                }
            }

            if (i > 0)
                packet.RemoveRange(0, i);

            await Task.Delay(2000);
        }
    }
}