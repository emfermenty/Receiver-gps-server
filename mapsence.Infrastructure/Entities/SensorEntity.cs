using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mapsence.Domain.Models;

namespace mapsence.Infrastructure.Entities;

[Table("sensor", Schema="zxc")]
public class SensorEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id", Order = 0)]
    public int Id { get; set; }
    
    [Column("imei")]
    public string? Imei { get; set; }
    
    [ForeignKey("User")]
    [Column("user_id")]
    public int UserId { get; set; }
    public UserEntity User { get; set; }
    
    public virtual List<TrackEntity>? Tracks { get; set; }
    public virtual List<GeoDataEntity>? GeoData { get; set; }
}