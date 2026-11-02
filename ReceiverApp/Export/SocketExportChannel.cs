using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Gps.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Receiver_server.Export;

/// <summary>
/// Клиент сокета: подключается к mapsence по TLS и держит соединение, пока оно не оборвётся.
/// </summary>
public sealed class SocketExportChannel(IConfiguration configuration, IHostEnvironment environment)
    : IExportChannel, IDisposable
{
    private readonly string _host = configuration["Export:SocketHost"]!;
    private readonly int _port = configuration.GetValue<int>("Export:SocketPort");
    private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(configuration.GetValue<int>("Export:SocketTimeoutMs"));
    private readonly X509Certificate2 _ca =
        TlsStreams.LoadCa(Path.Combine(environment.ContentRootPath, configuration["Tls:CaPath"]!));

    private TcpClient? _client;
    private SslStream? _stream;

    public string Name => "socket";

    public async Task SendAsync(RawPointDto point, CancellationToken token)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeout.CancelAfter(_timeout);

        try
        {
            if (_stream == null)
            {
                _client = new TcpClient();
                await _client.ConnectAsync(_host, _port, timeout.Token);
                _stream = await TlsStreams.AuthenticateAsClientAsync(_client.GetStream(), _host, _ca, timeout.Token);
            }

            await Frame.WriteAsync(_stream, RawPointJson.Serialize(point), timeout.Token);
            await Frame.ReadAckAsync(_stream, timeout.Token);
        }
        catch
        {
            Disconnect();
            throw;
        }
    }

    public void Dispose()
    {
        Disconnect();
        _ca.Dispose();
    }

    private void Disconnect()
    {
        _stream?.Dispose();
        _stream = null;
        _client?.Dispose();
        _client = null;
    }
}
