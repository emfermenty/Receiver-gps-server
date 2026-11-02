using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Receiver_server.Database;
using Receiver_server.Export;
using Receiver_server.Receiver;

namespace Receiver_server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            {
                Args = args,
                ContentRootPath = AppContext.BaseDirectory
            });

            builder.Services.AddSingleton<RawPointStore>();
            builder.Services.AddHostedService<GpsEndPoint>();

            // queue — только Kafka, socket — только сокет, auto — Kafka с переходом на сокет при сбое.
            // Порядок регистрации задаёт приоритет каналов
            string transport = builder.Configuration["Export:Transport"]!;
            if (transport is not ("queue" or "socket" or "auto"))
                throw new InvalidOperationException($"Неизвестный Export:Transport: {transport}");

            if (transport is "queue" or "auto")
                builder.Services.AddSingleton<IExportChannel, KafkaExportChannel>();
            if (transport is "socket" or "auto")
                builder.Services.AddSingleton<IExportChannel, SocketExportChannel>();
            builder.Services.AddHostedService<ExportService>();

            var host = builder.Build();
            await host.Services.GetRequiredService<RawPointStore>().InitializeAsync();
            await host.RunAsync();
        }
    }
}
