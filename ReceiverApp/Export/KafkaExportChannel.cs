using Confluent.Kafka;
using Gps.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Receiver_server.Export;

public sealed class KafkaExportChannel : IExportChannel, IDisposable
{
    private readonly IProducer<Null, byte[]> _producer;
    private readonly string _topic;

    public string Name => "kafka";

    public KafkaExportChannel(IConfiguration configuration, IHostEnvironment environment)
    {
        _topic = configuration["Kafka:Topic"]!;

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            SecurityProtocol = SecurityProtocol.Ssl,
            SslCaLocation = Path.Combine(environment.ContentRootPath, configuration["Tls:CaPath"]!),
            Acks = Acks.All,
            // Если брокер выключен, отправка быстро падает и строка уходит через резервный канал
            MessageTimeoutMs = configuration.GetValue<int>("Kafka:SendTimeoutMs")
        };

        // Пока брокер выключен, librdkafka постоянно пишет об ошибках подключения.
        // О смене канала сообщает ExportService, а ошибка TLS сама не пройдёт, поэтому её показываем
        _producer = new ProducerBuilder<Null, byte[]>(config)
            .SetLogHandler((_, _) => { })
            .SetErrorHandler((_, error) =>
            {
                if (error.Code == ErrorCode.Local_Ssl)
                    Console.WriteLine($"[kafka] Ошибка TLS: {error.Reason}");
            })
            .Build();
    }

    public async Task SendAsync(RawPointDto point, CancellationToken token)
    {
        await _producer.ProduceAsync(_topic, new Message<Null, byte[]> { Value = RawPointJson.Serialize(point) }, token);
    }

    public void Dispose() => _producer.Dispose();
}
