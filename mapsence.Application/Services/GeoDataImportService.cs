using Gps.Transport;
using mapsence.Application.Services.Interfaces;
using mapsence.Domain.Models;
using NetTopologySuite.Geometries;

namespace mapsence.Application.Services;

/// <summary>
/// Нормализация строки raw_points: устройство, сенсор, сессия и событие выносятся
/// в справочники device_model, sensor, sensor_session, event_type, а сама точка пишется в geo_data.
/// Каждый шаг идемпотентен, и geo_data пишется последней, поэтому повторная доставка строки
/// или падение посередине не создают дублей и общая транзакция не нужна.
/// </summary>
public class GeoDataImportService(IGeoDataImportRepository repository) : IGeoDataImportService
{
    public async Task<bool> ImportAsync(RawPointDto point)
    {
        if (await repository.ExistsBySourceIdAsync(point.SourceId))
            return false;

        int modelId = await repository.GetOrCreateDeviceModelAsync(point.Manufacturer, point.Model);
        int sensorId = await repository.GetOrCreateSensorAsync(point.Imei, modelId);
        // PostgreSQL хранит время с точностью до микросекунд, а сессия ищется по точному совпадению,
        // поэтому начало сессии округляется до секунд
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
