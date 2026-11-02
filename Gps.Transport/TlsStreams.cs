using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Gps.Transport;

/// <summary>
/// TLS для сокета между экспортом и импортом: сервер предъявляет свой сертификат,
/// клиент проверяет его подпись по нашему CA и совпадение адреса.
/// </summary>
public static class TlsStreams
{
    private const SslProtocols Protocols = SslProtocols.Tls12 | SslProtocols.Tls13;

    public static X509Certificate2 LoadCa(string caPath) =>
        X509Certificate2.CreateFromPem(File.ReadAllText(caPath));

    public static X509Certificate2 LoadServerCertificate(string certPath, string keyPath)
    {
        // На Windows SslStream не принимает ключ, загруженный из PEM напрямую, поэтому сертификат импортируется заново
        using var certificate = X509Certificate2.CreateFromPemFile(certPath, keyPath);
        return new X509Certificate2(certificate.Export(X509ContentType.Pkcs12));
    }

    public static async Task<SslStream> AuthenticateAsClientAsync(
        Stream inner, string host, X509Certificate2 ca, CancellationToken token)
    {
        var chainPolicy = new X509ChainPolicy
        {
            TrustMode = X509ChainTrustMode.CustomRootTrust,
            RevocationMode = X509RevocationMode.NoCheck
        };
        chainPolicy.CustomTrustStore.Add(ca);

        var stream = new SslStream(inner, leaveInnerStreamOpen: false);
        try
        {
            await stream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
            {
                TargetHost = host,
                EnabledSslProtocols = Protocols,
                CertificateChainPolicy = chainPolicy
            }, token);
            return stream;
        }
        catch
        {
            await stream.DisposeAsync();
            throw;
        }
    }

    public static async Task<SslStream> AuthenticateAsServerAsync(
        Stream inner, X509Certificate2 certificate, CancellationToken token)
    {
        var stream = new SslStream(inner, leaveInnerStreamOpen: false);
        try
        {
            await stream.AuthenticateAsServerAsync(new SslServerAuthenticationOptions
            {
                ServerCertificate = certificate,
                EnabledSslProtocols = Protocols
            }, token);
            return stream;
        }
        catch
        {
            await stream.DisposeAsync();
            throw;
        }
    }
}
