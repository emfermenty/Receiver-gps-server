using Microsoft.Extensions.DependencyInjection;
using Receiver_server.Database;
using Receiver_server.Receiver;

namespace Receiver_server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddSingleton<DataBaseHandler>();
            services.AddSingleton<TrackStateService>();
            services.AddSingleton<GpsEndPoint>();
            services.AddSingleton<KafkaConsumer>();

            var provider = services.BuildServiceProvider();
            
            var gps = provider.GetRequiredService<GpsEndPoint>();
            var kafka = provider.GetRequiredService<KafkaConsumer>();

            var gpsTask = gps.OpenEndPoint();
            var kafkaTask = kafka.StartAsync(CancellationToken.None);

            await Task.WhenAll(gpsTask, kafkaTask);
        }
    }
}