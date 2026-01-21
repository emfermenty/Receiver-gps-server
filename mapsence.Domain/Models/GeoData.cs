using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace mapsence.Domain.Models;
public class GeoData
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Height { get; set; }
    public double Speed { get; set; }
    public DateTime DateTime { get; set; }
    public double Course { get; set; }
    public double BatteryVoltage { get; set; }
    public int SatelliteCount { get; set; }
    public Point? GeometryBytes { get; set; }
    public int User_id { get; set; }
    public User? User { get; set; }
    public Sensor? Sensor { get; set; }
}