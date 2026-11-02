using System.Text;
using Gps.Transport;
using Receiver_server.Receiver;

namespace Receiver_server;

public static class Parser
{
    // 4 байта префикс + 15 байт IMEI + 1 байт длины описания устройства
    public const int HandshakeHeaderLength = 20;
    public const int PacketLength = 34;

    public static DeviceInfo ParseHandshake(byte[] header, byte[] info)
    {
        string imei = Encoding.UTF8.GetString(header, 4, 15);
        // описание: "производитель;модель;прошивка"
        string[] parts = Encoding.UTF8.GetString(info).Split(';');
        return new DeviceInfo(
            imei,
            parts.ElementAtOrDefault(0) ?? "unknown",
            parts.ElementAtOrDefault(1) ?? "unknown",
            parts.ElementAtOrDefault(2) ?? "unknown");
    }

    public static RawPointDto ParseGeodata(byte[] packet, DeviceInfo device, SessionInfo session)
    {
        int mask = 0b00001111;
        int eventCode = packet[3];

        return new RawPointDto
        {
            Imei = device.Imei,
            Manufacturer = device.Manufacturer,
            Model = device.Model,
            Firmware = device.Firmware,
            SessionIp = session.Ip,
            SessionStartedAt = session.StartedAt,
            EventCode = eventCode,
            EventName = EventCodes.GetName(eventCode),
            DeviceTime = DateTimeOffset.FromUnixTimeSeconds(BitConverter.ToUInt32(packet, 26)).UtcDateTime,
            ReceivedAt = DateTime.UtcNow,
            Latitude = BitConverter.ToSingle(packet, 9),
            Longitude = BitConverter.ToSingle(packet, 13),
            Course = BitConverter.ToUInt16(packet, 17) / 10.0,
            Speed = BitConverter.ToUInt16(packet, 19) / 10.0,
            Height = BitConverter.ToUInt16(packet, 21),
            // младший полубайт — спутники GPS, старший — ГЛОНАСС
            SatelliteCount = (packet[25] & mask) + ((packet[25] >> 4) & mask),
            BatteryMv = BitConverter.ToUInt16(packet, 32),
        };
    }
}
