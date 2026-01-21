using mapsence.Domain.Models;

namespace mapsenceto.Application.Services;

public interface ITrackPointInfoRepository
{
    Task<List<TrackPointInfo>> FindByTrackIdOrderByDateTime(int trackId);
}