using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mapsence.Domain.Models;

[Table("v_tracks_view", Schema = "zxc")]
public class TrackView
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    public string? Name { get; set; }
    
    [Column("date_start")]
    public DateTime DateStart { get; set; }
    
    [Column("date_stop")]
    public DateTime? DateStop { get; set; }
    
    [Column("sensor_id")]
    public int SensorId { get; set; }
    
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("distance")]
    public float Distance { get; set; }
    
    [Column("avg_speed")]
    public float AvgSpeed { get; set; }
    
    [Column("duration")]
    public string? Duration { get; set; }
}