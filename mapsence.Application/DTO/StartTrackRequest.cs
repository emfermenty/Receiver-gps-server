namespace mapsence.Application.DTO;

public class StartTrackRequest
{
    public int TrackId { get; set; }
    public string Name { get; set; }
    public int SensorId { get; set; }
    public int UserId { get; set; }

    private DateTime _dateStart = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

    public DateTime DateStart
    {
        get => _dateStart;
        set => _dateStart = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
    }
}