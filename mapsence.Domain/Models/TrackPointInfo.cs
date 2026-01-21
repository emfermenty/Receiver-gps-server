using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mapsence.Domain.Models;

[Table("v_track_info", Schema = "zxc")]
public class TrackPointInfo
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

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

    [Column("sensor_id")]
    public long SensorId { get; set; }

    [Column("track_id")] 
    public long TrackId { get; set; }
}