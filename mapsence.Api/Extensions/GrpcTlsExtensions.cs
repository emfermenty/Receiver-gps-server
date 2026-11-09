
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace mapsence.Extensions;

public static class GrpcTlsExtensions
{
    public static void AddGrpcTls(this WebApplicationBuilder builder)
    {
        var tls = builder.Configuration.GetSection("Tls");

        var caPath = tls["CaPath"]
            ?? throw new InvalidOperationException("Tls:CaPath не задан");

        var certPath = tls["CertPath"]
            ?? throw new InvalidOperationException("Tls:CertPath не задан");

        var keyPath = tls["KeyPath"]
            ?? throw new InvalidOperationException("Tls:KeyPath не задан");

        // Проверяем наличие TLS-файлов.
        foreach (var path in new[] { caPath, certPath, keyPath })
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    $"TLS-файл не найден: {path}", path);
            }
        }

        // Загружаем корневой сертификат CA.
        var caPem = File.ReadAllText(caPath);

        Console.WriteLine($"CA path: {Path.GetFullPath(caPath)}");
        Console.WriteLine($"CA PEM header: {caPem.Split('\n')[0].Trim()}");

        var caCertificate = X509Certificate2.CreateFromPem(caPem);

        Console.WriteLine($"CA loaded: {caCertificate.Subject}");

        // Повторный импорт PKCS#12 поддерживает приватный ключ и в Windows Kestrel.
        var serverCertificate = Gps.Transport.TlsStreams.LoadServerCertificate(certPath, keyPath);

        var httpPort = builder.Configuration.GetValue<int>(
            "HttpPort", 8080);

        var grpcPort = builder.Configuration.GetValue<int>(
            "Grpc:Port", 5001);

        builder.WebHost.ConfigureKestrel(options =>
        {
            // HTTP endpoint для Web API и Swagger.
            options.ListenAnyIP(httpPort, listen =>
            {
                listen.Protocols = HttpProtocols.Http1;
            });

            // HTTPS endpoint для gRPC с обязательным mTLS.
            options.ListenAnyIP(grpcPort, listen =>
            {
                listen.Protocols = HttpProtocols.Http2;

                listen.UseHttps(https =>
                {
                    https.ServerCertificate = serverCertificate;

                    https.ClientCertificateMode =
                        ClientCertificateMode.RequireCertificate;

                    https.ClientCertificateValidation =
                        (certificate, _, _) =>
                        {
                            if (certificate is null)
                                return false;

                            using var chain = new X509Chain();

                            chain.ChainPolicy.TrustMode =
                                X509ChainTrustMode.CustomRootTrust;

                            chain.ChainPolicy.CustomTrustStore.Add(
                                caCertificate);

                            chain.ChainPolicy.RevocationMode =
                                X509RevocationMode.NoCheck;

                            // Разрешаем только сертификаты clientAuth.
                            chain.ChainPolicy.ApplicationPolicy.Add(
                                new Oid("1.3.6.1.5.5.7.3.2"));

                            return chain.Build(certificate);
                        };
                });
            });
        });
    }
}
