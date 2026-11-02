using Gps.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Receiver_server.Database;

namespace Receiver_server.Export;

/// <summary>
/// Построчно отправляет неотправленные строки raw_points. Каналы перебираются в порядке приоритета:
/// упавший канал пропускается на Export:RetrySeconds, затем снова пробуется первым.
/// Последний канал не пропускается: запасного у него нет, а восстановление не должно ждать паузу.
/// Строка помечается отправленной только после подтверждения, поэтому при недоступности всех каналов
/// данные копятся в SQLite. Повторная доставка безопасна: импорт отсекает дубли по SourceId.
/// </summary>
public class ExportService(RawPointStore store, IEnumerable<IExportChannel> channels, IConfiguration configuration)
    : BackgroundService
{
    private const int BatchSize = 100;
    private static readonly TimeSpan IdleDelay = TimeSpan.FromMilliseconds(500);

    private readonly List<IExportChannel> _channels = channels.ToList();
    private readonly Dictionary<IExportChannel, DateTime> _retryAt = new();
    private readonly TimeSpan _retryInterval = TimeSpan.FromSeconds(configuration.GetValue<int>("Export:RetrySeconds"));

    private IExportChannel? _current;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"Каналы экспорта: {string.Join(" -> ", _channels.Select(c => c.Name))}");

        while (!stoppingToken.IsCancellationRequested)
        {
            var points = await store.GetUnsentAsync(BatchSize);
            bool allSent = true;

            foreach (var point in points)
            {
                allSent = await TrySendAsync(point, stoppingToken);
                if (!allSent)
                    break;

                await store.MarkSentAsync(point.SourceId);
            }

            if (points.Count == 0 || !allSent)
                await Task.Delay(IdleDelay, stoppingToken);
        }
    }

    private async Task<bool> TrySendAsync(RawPointDto point, CancellationToken token)
    {
        foreach (var channel in _channels)
        {
            bool isLast = channel == _channels[^1];
            if (!isLast && _retryAt.TryGetValue(channel, out var retryAt) && DateTime.UtcNow < retryAt)
                continue;

            try
            {
                await channel.SendAsync(point, token);
            }
            catch (Exception ex) when (!token.IsCancellationRequested)
            {
                // О падении сообщаем один раз, а не на каждую повторную попытку
                if (!_retryAt.ContainsKey(channel))
                    Console.WriteLine($"[{channel.Name}] Канал недоступен: {ex.Message}");

                _retryAt[channel] = DateTime.UtcNow + _retryInterval;
                continue;
            }

            _retryAt.Remove(channel);
            if (_current != channel)
            {
                Console.WriteLine($"Передача идёт через {channel.Name}");
                _current = channel;
            }

            Console.WriteLine($"[{channel.Name}] Отправлена строка #{point.SourceId}");
            return true;
        }

        _current = null;
        return false;
    }
}
