using mapsence.Application.Services.Interfaces;
using mapsence.Domain.Models;
using mapsenceto.Application.Services;

namespace mapsence.Application.Services;

public class SensorService(
    ISensorRepository sensorRepository,
    IGeoDataRepository geoDataRepository
    ) : ISensorService
{
    public async Task<List<Sensor>> FindAllSensorsAsync()
    {
        return await sensorRepository.FindAllSensorsAsync();
    }

    public async Task<GeoData?> CurrentGeoDataOfSensorsAsync(int sensorId)
    {
        return await geoDataRepository.findFirstBySensorIdOrderByDateTimeDesc(sensorId);
    }
    
    public async Task<List<Sensor>> FindByUserIdAsync(int userId)
    {
        return await sensorRepository.FindByUserId(userId);
    }
}