using mapsence.Domain.Models;

namespace mapsenceto.Application.Services;

public interface ISensorService
{
    Task<List<Sensor>> FindAllSensorsAsync();
    Task<GeoData?> CurrentGeoDataOfSensorsAsync(int sensorId);
    Task<List<Sensor>> FindByUserIdAsync(int userId);
}