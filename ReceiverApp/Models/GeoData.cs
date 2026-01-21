namespace Receiver_server;

public class GeoData
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Course { get; set; }
    public float Speed { get; set; }
    public int Height { get; set; }
    public int SatCount { get; set; }
    public DateTime DateTime { get; set; }
    public int VBattery { get; set; }
    public int UserId {get; set;}

    private GeoData(double latitude, double longitude, double course, float speed, int height, int satCount,
        DateTime dateTime, int vBattery, int UserId)
    {
        this.Latitude = latitude;
        this.Longitude = longitude;
        this.Course = course;
        this.Speed = speed;
        this.Height = height;
        this.SatCount = satCount;
        this.DateTime = dateTime;
        this.VBattery = vBattery;
        this.UserId = UserId;
    }

    public static GeoData Create(double latitude, double longitude, double course, float speed, int height,
        int satCount, int vBattery, int UserId)
    {
        DateTime dateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        return new GeoData(latitude, longitude, course, speed, height, satCount, dateTime, vBattery, UserId);
    }
}