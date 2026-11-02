namespace mapsence.Domain.Models;

public class DeviceModel
{
    public int Id { get; set; }
    public string Manufacturer { get; set; } = "";
    public string Name { get; set; } = "";
    public virtual List<Sensor>? Sensors { get; set; }
}
