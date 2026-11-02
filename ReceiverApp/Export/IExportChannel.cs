using Gps.Transport;

namespace Receiver_server.Export;

public interface IExportChannel
{
    string Name { get; }

    /// <summary>
    /// Завершается после подтверждения доставки, при сбое бросает исключение.
    /// </summary>
    Task SendAsync(RawPointDto point, CancellationToken token);
}
