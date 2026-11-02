using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mapsence.Infrastructure.Entities;

[Table("event_type", Schema = "zxc")]
public class EventTypeEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    public int Code { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    public virtual List<GeoDataEntity>? GeoData { get; set; }
}
