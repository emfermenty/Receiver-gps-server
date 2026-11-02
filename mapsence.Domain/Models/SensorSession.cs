namespace mapsence.Domain.Models;

public class SensorSession
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public string Ip { get; set; } = "";
    public string Firmware { get; set; } = "";
    public DateTime StartedAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public Sensor? Sensor { get; set; }
}
