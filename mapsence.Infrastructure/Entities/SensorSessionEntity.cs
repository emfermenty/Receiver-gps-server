using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mapsence.Infrastructure.Entities;

[Table("sensor_session", Schema = "zxc")]
public class SensorSessionEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey("Sensor")]
    [Column("sensor_id")]
    public int SensorId { get; set; }

    [Column("ip")]
    public string Ip { get; set; } = "";

    [Column("firmware")]
    public string Firmware { get; set; } = "";

    [Column("started_at")]
    public DateTime StartedAt { get; set; }

    [Column("last_seen_at")]
    public DateTime LastSeenAt { get; set; }

    public SensorEntity? Sensor { get; set; }
}
