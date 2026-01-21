using mapsence.Application.DTO;
using mapsence.Domain.Models;

namespace mapsenceto.Application.Services;

public interface ITrackRepository
{
    Task<Track?> FindTrackByIdAsync(int id);
    Task<int> PrcStartTrackAsync(StartTrackRequest request);
    Task PrcStopTrackAsync(int trackId, DateTime dateStop);
    Task PrcDeleteTrackAsync(int trackId);
    Task PrcEditTrackAsync(int trackId, string name);
    Task PrcComebackTrackAsync(int trackId);
}