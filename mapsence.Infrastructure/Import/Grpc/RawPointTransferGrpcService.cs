using Gps.Transport;
using Grpc.Core;
using mapsence.Infrastructure.Import;
using RawPoints.Grpc;

namespace mapsence.Grpc;

public sealed class RawPointTransferGrpcService(
    GeoDataImportRunner importRunner)
    : RawPointTransfer.RawPointTransferBase
{
    public override async Task<PointResponse> SendPoint(
        RawPoint request,
        ServerCallContext context)
    {
        var point = new RawPointDto
        {
            SourceId = request.SourceId,
            Imei = request.Imei,
            Manufacturer = request.Manufacturer,
            Model = request.Model,
            Firmware = request.Firmware,
            SessionIp = request.SessionIp,

            SessionStartedAt =
                request.SessionStartedAt.ToDateTime(),

            EventCode = request.EventCode,
            EventName = request.EventName,

            DeviceTime =
                request.DeviceTime.ToDateTime(),

            ReceivedAt =
                request.ReceivedAt.ToDateTime(),

            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Height = request.Height,
            Speed = request.Speed,
            Course = request.Course,
            SatelliteCount = request.SatelliteCount,
            BatteryMv = request.BatteryMv
        };

        await importRunner.ImportAsync(
            point,
            context.CancellationToken);

        return new PointResponse
        {
            Success = true
        };
    }
}
