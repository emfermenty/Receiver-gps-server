using mapsence.Application.Services.Interfaces;
using mapsence.Domain.Models;

namespace mapsenceto.Application.Services;

public class GeoDataService(IGeoDataRepository geoDataRepository) : IGeoDataService
{
    public async Task<List<GeoData>> FindBySensorId(int sensorId)
    {
        return await geoDataRepository.FindBySensorId(sensorId);
    }
}