using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mapsence.Domain.Models;

public class Track
{
    public int Id { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime? DateStop { get; set; }
    public string? Name { get; set; }
    public int SensorId { get; set; }
    public Sensor Sensor { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}