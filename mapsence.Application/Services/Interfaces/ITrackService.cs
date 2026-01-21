

using mapsence.Application.DTO;
using mapsence.Domain.Models;

namespace mapsence.Application.Services;

public interface ITrackService
{
    Task<Track?> FindTrackByIdAsync(int id);
    Task<List<TrackDisplay>> FindByUserId(int userId);
    Task<List<TrackPointInfo>> FindPointsByTrack(int trackId);
    Task DeleteTrack(int trackId);
    Task EditTrack(int trackId, string name);
    Task StartTrack(StartTrackRequest request);
    Task StopTrack(int trackId);
    Task ComebackTrack(int trackId);
}