using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Receiver_server.Database;
using Receiver_server.Export;
using Receiver_server.Receiver;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using Grpc.Net.Client;
using RawPoints.Grpc;

namespace Receiver_server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(
                new HostApplicationBuilderSettings
                {
                    Args = args,
                    ContentRootPath = AppContext.BaseDirectory
                });

            builder.Services.AddSingleton<RawPointStore>();
            builder.Services.AddHostedService<GpsEndPoint>();

            var transport = builder.Configuration["Export:Transport"]
                ?? throw new InvalidOperationException(
                    "Export:Transport не задан");

            if (transport is not ("grpc" or "queue" or "auto"))
                throw new InvalidOperationException(
                    $"Неизвестный Export:Transport: {transport}");

            if (transport is "grpc" or "auto")
            {
                var grpcAddress = builder.Configuration["Grpc:Address"]
                    ?? throw new InvalidOperationException(
                        "Grpc:Address не задан");

                var clientCertPath =
                    builder.Configuration["Grpc:ClientCertificate:Path"]
                    ?? throw new InvalidOperationException(
                        "Не задан путь клиентского сертификата");

                var clientCertPassword =
                    builder.Configuration["Grpc:ClientCertificate:Password"];

                if (!Uri.TryCreate(grpcAddress, UriKind.Absolute, out var address) ||
                    address.Scheme != Uri.UriSchemeHttps)
                    throw new InvalidOperationException("Grpc:Address должен использовать https://");

                var caPath = builder.Configuration["Tls:CaPath"]
                    ?? throw new InvalidOperationException("Tls:CaPath не задан");
                var ca = X509Certificate2.CreateFromPem(File.ReadAllText(
                    Path.Combine(builder.Environment.ContentRootPath, caPath)));

                var clientCertificate = new X509Certificate2(
                    Path.Combine(builder.Environment.ContentRootPath, clientCertPath),
                    clientCertPassword);

                var chainPolicy = new X509ChainPolicy
                {
                    TrustMode = X509ChainTrustMode.CustomRootTrust,
                    RevocationMode = X509RevocationMode.NoCheck
                };
                chainPolicy.CustomTrustStore.Add(ca);
                chainPolicy.ApplicationPolicy.Add(new System.Security.Cryptography.Oid("1.3.6.1.5.5.7.3.1"));

                var handler = new SocketsHttpHandler
                {
                    SslOptions = new SslClientAuthenticationOptions
                    {
                        EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                        CertificateChainPolicy = chainPolicy,
                        ClientCertificates = new X509CertificateCollection { clientCertificate }
                    }
                };

                builder.Services.AddSingleton(_ =>
                    GrpcChannel.ForAddress(
                        grpcAddress,
                        new GrpcChannelOptions
                        {
                            HttpHandler = handler
                        }));

                builder.Services.AddSingleton(sp =>
                    new RawPointTransfer.RawPointTransferClient(
                        sp.GetRequiredService<GrpcChannel>()));
            }

            // Порядок регистрации определяет приоритет каналов.
            if (transport is "grpc" or "auto")
                builder.Services.AddSingleton<IExportChannel, GrpcExportChannel>();

            if (transport is "queue" or "auto")
                builder.Services.AddSingleton<IExportChannel, KafkaExportChannel>();

            builder.Services.AddHostedService<ExportService>();

            var host = builder.Build();

            await host.Services
                .GetRequiredService<RawPointStore>()
                .InitializeAsync();

            await host.RunAsync();
        }
    }
}