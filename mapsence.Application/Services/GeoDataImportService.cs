using Gps.Transport;
using mapsence.Application.Services.Interfaces;
using mapsence.Domain.Models;
using NetTopologySuite.Geometries;

namespace mapsence.Application.Services;

public class GeoDataImportService(IGeoDataImportRepository repository) : IGeoDataImportService
{
    public async Task<bool> ImportAsync(RawPointDto point)
    {
        if (await repository.ExistsBySourceIdAsync(point.SourceId))
            return false;

        int modelId = await repository.GetOrCreateDeviceModelAsync(point.Manufacturer, point.Model);
        int sensorId = await repository.GetOrCreateSensorAsync(point.Imei, modelId);
        await repository.SaveSessionAsync(sensorId, point.SessionIp, point.Firmware,
            TruncateToSeconds(point.SessionStartedAt), point.DeviceTime);
        int eventTypeId = await repository.GetOrCreateEventTypeAsync(point.EventCode, point.EventName);

        await repository.AddGeoDataAsync(new GeoData
        {
            SourceId = point.SourceId,
            SensorId = sensorId,
            EventTypeId = eventTypeId,
            Latitude = point.Latitude,
            Longitude = point.Longitude,
            Height = point.Height,
            Speed = point.Speed,
            Course = point.Course,
            DateTime = point.DeviceTime,
            BatteryVoltage = point.BatteryMv,
            SatelliteCount = point.SatelliteCount,
            GeometryBytes = new Point(point.Longitude, point.Latitude) { SRID = 4326 }
        });
        return true;
    }

    private static DateTime TruncateToSeconds(DateTime value) =>
        new(value.Ticks - value.Ticks % TimeSpan.TicksPerSecond, value.Kind);
}
