using Receiver_server.Database;

namespace Receiver_server;

public static class Parser
{
    public async static Task ParseGeodata(List<byte> geodata, int userId, bool occupancy)
    {
        DataBaseHandler handler = new DataBaseHandler();
        int mask = 0b00001111;

        byte[] latBytes = geodata.Skip(9).Take(4).ToArray();
        byte[] lonBytes = geodata.Skip(13).Take(4).ToArray();
        byte[] courseBytes = geodata.Skip(17).Take(2).ToArray();
        byte[] speedBytes = geodata.Skip(19).Take(2).ToArray();
        byte[] heightBytes = geodata.Skip(22).Take(2).ToArray();
        byte satCountBytes = geodata.Skip(25).Take(1).ToArray()[0];
        byte[] datetimeBytes = geodata.Skip(26).Take(4).ToArray();
        byte[] vBatteryBytes = geodata.Skip(32).Take(2).ToArray();

        double lat = BitConverter.ToSingle(latBytes);
        double lon = BitConverter.ToSingle(lonBytes);
        float course = BitConverter.ToUInt16(courseBytes) / 10;
        float speed = BitConverter.ToUInt16(speedBytes) / 10;
        int height = BitConverter.ToUInt16(heightBytes);
        int satCount = (satCountBytes & mask) + ((satCountBytes >> 4) & mask);
        uint seconds = BitConverter.ToUInt32(datetimeBytes);
        DateTime dateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        int vBattery = BitConverter.ToUInt16(vBatteryBytes);
        if (occupancy)
        {
            await handler.CallProcedureOccupancyAsync(GeoData.Create(lat, lon, course, speed, height, satCount, vBattery, userId));
            Console.WriteLine($"ЗАПИСЬ В БД {lat}\t{lon}\t{course} {height} {speed} {satCount} {dateTime} {vBattery}");
        }
    }
}