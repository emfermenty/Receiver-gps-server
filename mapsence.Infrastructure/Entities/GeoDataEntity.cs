using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace mapsence.Infrastructure.Entities;

[Table("geo_data", Schema ="zxc")]
public class GeoDataEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey("Sensor")]
    [Column("sensor_id")]
    public int SensorId { get; set; }

    [Column("latitude")]
    public double Latitude { get; set; }
    
    [Column("longitude")]
    public double Longitude { get; set; }
    
    [Column("height")]
    public double Height { get; set; }
    
    [Column("speed")]
    public double Speed { get; set; }
    
    [Column("date_time")]
    public DateTime DateTime { get; set; }

    [Column("course")] 
    public double Course { get; set; }
    
    [Column("battery_voltage")]
    public double BatteryVoltage { get; set; }
    
    [Column("satellite_count")]
    public int SatelliteCount { get; set; }
    
    [Column("geom", TypeName = "geometry (Point,4326)")]
    public Point? GeometryBytes { get; set; }
    
    [Column("user_id")]
    public int User_id { get; set; }
    
    public UserEntity? User { get; set; }
    public SensorEntity? Sensor { get; set; }
}