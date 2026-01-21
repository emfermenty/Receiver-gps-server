using AutoMapper;
using mapsence.Application.Services.Interfaces;
using mapsence.Domain.Models;
using mapsence.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace mapsence.Infrastructure.Repositories;
public class SensorRepository(ApplicationContext context, IMapper _mapper) : ISensorRepository
{
    public async Task<List<Sensor>> FindAllSensorsAsync()
    {
        var entitys = await context.Sensors.ToListAsync();
        return _mapper.Map<List<Sensor>>(entitys);
    }

    public async Task<Sensor?> FindSensorByUserIdAsync(int id)
    {
        return _mapper.Map<Sensor?>(await context.Sensors.Where(s => s.UserId == id).FirstOrDefaultAsync());
    }
    
    public async Task<List<Sensor>> FindByUserId(int userid)
    {
        var entitys =  await context.Sensors.Where(u => u.UserId == userid).ToListAsync();
        return _mapper.Map<List<Sensor>>(entitys);
    }
}