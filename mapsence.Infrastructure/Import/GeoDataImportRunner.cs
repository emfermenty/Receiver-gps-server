using Gps.Transport;
using mapsence.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace mapsence.Infrastructure.Import;

/// <summary>
/// Общая точка входа для каналов приёма. Строки импортируются строго по одной:
/// Kafka и сокет работают одновременно, а поиск и создание записей справочников не атомарны.
/// </summary>
public class GeoDataImportRunner(IServiceScopeFactory scopeFactory)
{
    private readonly SemaphoreSlim _gate = new(1, 1);

    /// <returns>false, если строка уже была импортирована ранее</returns>
    public async Task<bool> ImportAsync(RawPointDto point, CancellationToken token)
    {
        await _gate.WaitAsync(token);
        try
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IGeoDataImportService>();
            return await service.ImportAsync(point);
        }
        finally
        {
            _gate.Release();
        }
    }
}
