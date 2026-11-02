namespace Gps.Transport;

/// <summary>
/// Строка ненормализованной таблицы raw_points: точка вместе со всем контекстом
/// (устройство, сессия подключения, событие). Передаётся от экспорта к импорту построчно.
/// </summary>
public class RawPointDto
{
    public long SourceId { get; set; }

    public string Imei { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public string Model { get; set; } = "";
    public string Firmware { get; set; } = "";

    public string SessionIp { get; set; } = "";
    public DateTime SessionStartedAt { get; set; }

    public int EventCode { get; set; }
    public string EventName { get; set; } = "";

    public DateTime DeviceTime { get; set; }
    public DateTime ReceivedAt { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Height { get; set; }
    public double Speed { get; set; }
    public double Course { get; set; }
    public int SatelliteCount { get; set; }
    public int BatteryMv { get; set; }
}
