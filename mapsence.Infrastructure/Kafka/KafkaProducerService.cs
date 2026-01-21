using Confluent.Kafka;
using mapsenceto.Application.Services;
using Microsoft.Extensions.Configuration;

namespace mapsencetoWebApi.Kafka;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly string? _bootstrapServers;
    private readonly string? _topic;

    public KafkaProducerService(IConfiguration configuration)
    {
        _bootstrapServers = configuration["Kafka:BootstrapServers"];
        _topic = configuration["Kafka:Topic"];
    }
    public async Task SendMessageAsync(string message)
    {
        var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

        using var producer = new ProducerBuilder<Null, string>(config).Build();
        var result = await producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });

        Console.WriteLine($"Sent: {result.Value} to partition {result.Partition} at offset {result.Offset}");
    }
}