using Gps.Transport;

namespace mapsence.Application.Services.Interfaces;

public interface IGeoDataImportService
{
    /// <returns>false, если строка с таким SourceId уже была импортирована</returns>
    Task<bool> ImportAsync(RawPointDto point);
}
