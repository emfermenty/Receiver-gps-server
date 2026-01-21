using Confluent.Kafka;
using System.Configuration;

public class KafkaConsumer
{
    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly string _groupId;
    private readonly TrackStateService _stateManager;

    public KafkaConsumer(TrackStateService stateManager)
    {
        _bootstrapServers = ConfigurationManager.AppSettings["KafkaBootstrapServers"]!;
        _topic = ConfigurationManager.AppSettings["KafkaTopic"]!;
        _groupId = ConfigurationManager.AppSettings["KafkaGroupId"] ?? "receiver-group";
        _stateManager = stateManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Latest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(cancellationToken);
                    var message = result.Message.Value;
                    Console.WriteLine($"[Kafka] Получено сообщение: {message}");

                    var parts = message.Split(':');
                    if (parts[0].Equals("START", StringComparison.OrdinalIgnoreCase))
                    {
                        int trackId = int.Parse(parts[1]);
                        int userId = int.Parse(parts[2]);
                        string imei = parts[3];
                        _stateManager.StartRecording(imei, trackId, userId);
                    }
                    else if (parts[0].Equals("STOP", StringComparison.OrdinalIgnoreCase))
                    {
                        string imei = parts[1];
                        int trackId = int.Parse(parts[2]);
                        _stateManager.StopRecording(imei, trackId);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Kafka consumer остановлен.");
            }
            finally
            {
                consumer.Close();
            }
        }, cancellationToken);
    }
}
