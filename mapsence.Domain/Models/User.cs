using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mapsence.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Pass { get; set; }
    public virtual List<GeoData>? GeoData { get; set; }
    public virtual List<Sensor>? Sensors { get; set; }
    public virtual List<Track>? Tracks { get; set; }
}