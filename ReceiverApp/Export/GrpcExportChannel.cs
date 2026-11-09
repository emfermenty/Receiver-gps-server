using Microsoft.Extensions.Configuration;
using Google.Protobuf.WellKnownTypes;
using Gps.Transport;
using RawPoints.Grpc;

namespace Receiver_server.Export;

public class GrpcExportChannel(
    RawPointTransfer.RawPointTransferClient client,
    Microsoft.Extensions.Configuration.IConfiguration configuration) : IExportChannel
{
    public string Name => "gRPC";

    public async Task SendAsync(
        RawPointDto point,
        CancellationToken token)
    {
        var request = new RawPoint
        {
            SourceId = point.SourceId,

            Imei = point.Imei,
            Manufacturer = point.Manufacturer,
            Model = point.Model,
            Firmware = point.Firmware,

            SessionIp = point.SessionIp,
            SessionStartedAt = ToTimestamp(point.SessionStartedAt),

            EventCode = point.EventCode,
            EventName = point.EventName,

            DeviceTime = ToTimestamp(point.DeviceTime),
            ReceivedAt = ToTimestamp(point.ReceivedAt),

            Latitude = point.Latitude,
            Longitude = point.Longitude,
            Height = point.Height,
            Speed = point.Speed,
            Course = point.Course,
            SatelliteCount = point.SatelliteCount,
            BatteryMv = point.BatteryMv
        };

        using var call = client.SendPointAsync(
            request,
            deadline: DateTime.UtcNow.AddSeconds(
                configuration.GetValue<int>("Grpc:TimeoutSeconds", 10)),
            cancellationToken: token);

        var response = await call.ResponseAsync;

        if (!response.Success)
        {
            throw new InvalidOperationException(
                $"MapSense rejected point #{point.SourceId}: {response.Error}");
        }
    }

    private static Timestamp ToTimestamp(DateTime value)
    {
        var utc = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            DateTimeKind.Unspecified =>
                DateTime.SpecifyKind(value, DateTimeKind.Utc),

            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };

        return Timestamp.FromDateTime(utc);
    }
}