using Gps.Transport;

namespace mapsence.Application.Services.Interfaces;

public interface IGeoDataImportService
{
    Task<bool> ImportAsync(RawPointDto point);
}
