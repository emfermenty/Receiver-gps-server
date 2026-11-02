using mapsence.Domain.Models;

namespace mapsence.Application.Services.Interfaces;

public interface IGeoDataImportRepository
{
    Task<bool> ExistsBySourceIdAsync(long sourceId);
    Task<int> GetOrCreateDeviceModelAsync(string manufacturer, string name);
    Task<int> GetOrCreateSensorAsync(string imei, int modelId);
    Task SaveSessionAsync(int sensorId, string ip, string firmware, DateTime startedAt, DateTime lastSeenAt);
    Task<int> GetOrCreateEventTypeAsync(int code, string name);
    Task AddGeoDataAsync(GeoData geoData);
}
