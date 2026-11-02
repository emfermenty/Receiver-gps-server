using System.Text.Json;
using Confluent.Kafka;
using Gps.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace mapsence.Infrastructure.Import;

public class KafkaImportConsumer(
    GeoDataImportRunner importer,
    IConfiguration configuration,
    IHostEnvironment environment) : BackgroundService
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

    // Consume блокирует поток, поэтому цикл вынесен с потока запуска приложения
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);

    private async Task ConsumeAsync(CancellationToken token)
    {
        string topic = configuration["Kafka:Topic"]!;
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            SecurityProtocol = SecurityProtocol.Ssl,
            SslCaLocation = Path.Combine(environment.ContentRootPath, configuration["Tls:CaPath"]!),
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            AllowAutoCreateTopics = true
        };

        // Пока брокер выключен, librdkafka постоянно пишет об ошибках подключения: это штатный режим.
        // Ошибка TLS сама не пройдёт, поэтому её показываем
        using var consumer = new ConsumerBuilder<Ignore, byte[]>(config)
            .SetLogHandler((_, _) => { })
            .SetErrorHandler((_, error) =>
            {
                if (error.Code == ErrorCode.Local_Ssl)
                    Console.WriteLine($"[kafka] Ошибка TLS: {error.Reason}");
            })
            .Build();
        consumer.Subscribe(topic);
        Console.WriteLine($"[kafka] Чтение топика {topic}");

        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(token);
                    await ImportAsync(result.Message.Value, token);
                    consumer.Commit(result);
                }
                catch (KafkaException ex) when (!ex.Error.IsFatal)
                {
                    // Например, топик ещё не создан. Незакоммиченная строка будет прочитана повторно
                    Console.WriteLine($"[kafka] {ex.Error.Reason}");
                    await Task.Delay(RetryDelay, token);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ImportAsync(byte[] payload, CancellationToken token)
    {
        RawPointDto point;
        try
        {
            point = RawPointJson.Deserialize(payload);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[kafka] Пропущено некорректное сообщение: {ex.Message}");
            return;
        }

        while (true)
        {
            try
            {
                bool imported = await importer.ImportAsync(point, token);
                Console.WriteLine($"[kafka] Строка #{point.SourceId} {(imported ? "импортирована" : "уже есть в БД")}");
                return;
            }
            catch (Exception ex) when (!token.IsCancellationRequested)
            {
                Console.WriteLine($"[kafka] Ошибка импорта строки #{point.SourceId}: {ex.Message}");
                await Task.Delay(RetryDelay, token);
            }
        }
    }
}
