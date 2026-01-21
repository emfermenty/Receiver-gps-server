using System.ComponentModel.DataAnnotations.Schema;

namespace mapsence.Domain.Models;

public class Sensor
{
    public int Id { get; set; }
    public string? Imei { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    
    public virtual List<Track>? Tracks { get; set; }
    public virtual List<GeoData>? GeoData { get; set; }
}