namespace mapsence.Application.DTO;

public class TrackDisplay
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime? DateStop { get; set; }
    public int SensorId { get; set; }
    public int UserId { get; set; }
    public double Distance { get; set; }
    public double AvgSpeed { get; set; }
    public string Duration { get; set; }

    private TrackDisplay(int id,
        string name,
        DateTime dateStart,
        DateTime? dateStop,
        int sensorId,
        int userId,
        double distance,
        double avgSpeed,
        string duration)
    {
        Id = id;
        Name = name;
        DateStart = dateStart;
        DateStop = dateStop;
        SensorId = sensorId;
        UserId = userId;
        Distance = distance;
        AvgSpeed = avgSpeed;
        Duration = duration;
    }

    public static TrackDisplay Create(int id,
        string name,
        DateTime dateStart,
        DateTime? dateStop,
        int sensorId,
        int userId,
        double distance,
        double avgSpeed,
        string duration)
    {
        return new TrackDisplay(id, name, dateStart, dateStop, sensorId, userId, distance, avgSpeed, duration);
    }
}