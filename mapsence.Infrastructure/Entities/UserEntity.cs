using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mapsence.Domain.Models;

namespace mapsence.Infrastructure.Entities;

[Table("user", Schema = "zxc")]
public class UserEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("login")]
    public string Username { get; set; }
    [Column("pass")]
    public string Pass { get; set; }

    public virtual List<SensorEntity>? Sensors { get; set; }
    public virtual List<TrackEntity>? Tracks { get; set; }
}