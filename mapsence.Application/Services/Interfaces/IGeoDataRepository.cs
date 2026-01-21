using mapsence.Domain.Models;

namespace mapsence.Application.Services.Interfaces;

public interface IGeoDataRepository
{
    Task<List<GeoData>> FindBySensorId(int sensorId);
    Task<GeoData?> findFirstBySensorIdOrderByDateTimeDesc(int sensorId);
}