using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mapsence.Infrastructure.Entities;

[Table("track", Schema = "zxc")]
public class TrackEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("date_start")] public DateTime DateStart { get; set; }

    [Column("date_stop")] public DateTime? DateStop { get; set; }

    [Column("name")] public string? Name { get; set; }

    [ForeignKey("Sensor")]
    [Column("sensor_id")]
    public int SensorId { get; set; }

    public SensorEntity Sensor { get; set; }

    [ForeignKey("User")]
    [Column("user_id")]
    public int UserId { get; set; }

    public UserEntity User { get; set; }
}