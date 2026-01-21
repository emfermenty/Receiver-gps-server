using mapsence.Domain.Models;

namespace mapsenceto.Application.Services;

public interface IGeoDataService
{
    Task<List<GeoData>> FindBySensorId(int sensorId);
}