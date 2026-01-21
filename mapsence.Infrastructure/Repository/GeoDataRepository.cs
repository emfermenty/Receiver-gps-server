using AutoMapper;
using mapsence.Application.Services.Interfaces;
using mapsence.Domain.Models;
using mapsence.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace mapsence.Infrastructure.Repositories;

public class GeoDataRepository(ApplicationContext context, IMapper _mapper) : IGeoDataRepository
{
    public async Task<List<GeoData>> FindBySensorId(int sensorId)
    {
        var entity = await context.GeoData.Where(g => g.SensorId == sensorId)
            .OrderBy(g => g.DateTime)
            .Include(t => t.User)
            .Include(t => t.Sensor)
            .AsNoTracking()
            .ToListAsync();
        return _mapper.Map<List<GeoData>>(entity);
    }

    public async Task<GeoData?> findFirstBySensorIdOrderByDateTimeDesc(int sensorId)
    {
        var entity = await context.GeoData.Where(g => g.SensorId == sensorId)
            .OrderByDescending(g => g.DateTime)
            .FirstOrDefaultAsync();
        return _mapper.Map<GeoData>(entity);
    } 
}