using System.ComponentModel.DataAnnotations.Schema;

namespace mapsence.Domain.Models;

public class Sensor
{
    public int Id { get; set; }
    public string? Imei { get; set; }
    public int? ModelId { get; set; }
    public DeviceModel? Model { get; set; }
    // null — трекер зарегистрирован импортом и ещё не привязан к пользователю
    public int? UserId { get; set; }
    public User? User { get; set; }

    public virtual List<Track>? Tracks { get; set; }
    public virtual List<GeoData>? GeoData { get; set; }
    public virtual List<SensorSession>? Sessions { get; set; }
}
