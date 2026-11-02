using AutoMapper;
using mapsence.Application.Services.Interfaces;
using mapsence.Domain.Models;
using mapsence.Infrastructure.Data;
using mapsence.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace mapsence.Infrastructure.Repositories;

public class GeoDataImportRepository(ApplicationContext context, IMapper _mapper) : IGeoDataImportRepository
{
    public async Task<bool> ExistsBySourceIdAsync(long sourceId)
    {
        return await context.GeoData.AnyAsync(g => g.SourceId == sourceId);
    }

    public async Task<int> GetOrCreateDeviceModelAsync(string manufacturer, string name)
    {
        var entity = await context.DeviceModels
            .FirstOrDefaultAsync(m => m.Manufacturer == manufacturer && m.Name == name);
        if (entity == null)
        {
            entity = new DeviceModelEntity { Manufacturer = manufacturer, Name = name };
            context.DeviceModels.Add(entity);
            await context.SaveChangesAsync();
        }
        return entity.Id;
    }

    public async Task<int> GetOrCreateSensorAsync(string imei, int modelId)
    {
        var entity = await context.Sensors.FirstOrDefaultAsync(s => s.Imei == imei);
        if (entity == null)
        {
            // неизвестный трекер регистрируется без владельца, к пользователю его привязывают позже
            entity = new SensorEntity { Imei = imei, ModelId = modelId };
            context.Sensors.Add(entity);
        }
        else
        {
            entity.ModelId = modelId;
        }
        await context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task SaveSessionAsync(int sensorId, string ip, string firmware, DateTime startedAt, DateTime lastSeenAt)
    {
        var entity = await context.SensorSessions
            .FirstOrDefaultAsync(s => s.SensorId == sensorId && s.StartedAt == startedAt);
        if (entity == null)
        {
            context.SensorSessions.Add(new SensorSessionEntity
            {
                SensorId = sensorId,
                Ip = ip,
                Firmware = firmware,
                StartedAt = startedAt,
                LastSeenAt = lastSeenAt
            });
        }
        else if (lastSeenAt > entity.LastSeenAt)
        {
            entity.LastSeenAt = lastSeenAt;
        }
        await context.SaveChangesAsync();
    }

    public async Task<int> GetOrCreateEventTypeAsync(int code, string name)
    {
        var entity = await context.EventTypes.FirstOrDefaultAsync(e => e.Code == code);
        if (entity == null)
        {
            entity = new EventTypeEntity { Code = code, Name = name };
            context.EventTypes.Add(entity);
            await context.SaveChangesAsync();
        }
        return entity.Id;
    }

    public async Task AddGeoDataAsync(GeoData geoData)
    {
        context.GeoData.Add(_mapper.Map<GeoDataEntity>(geoData));
        await context.SaveChangesAsync();
    }
}
