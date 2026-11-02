using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mapsence.Infrastructure.Entities;

[Table("device_model", Schema = "zxc")]
public class DeviceModelEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("manufacturer")]
    public string Manufacturer { get; set; } = "";

    [Column("name")]
    public string Name { get; set; } = "";

    public virtual List<SensorEntity>? Sensors { get; set; }
}
