using mapsence.Domain.Models;

namespace mapsence.Application.Services.Interfaces;

public interface ISensorRepository
{
    Task<List<Sensor>> FindAllSensorsAsync();
    Task<Sensor?> FindSensorByUserIdAsync(int id);
    Task<List<Sensor>> FindByUserId(int userid);
}