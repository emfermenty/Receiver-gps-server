using mapsence.Domain.Models;

namespace mapsence.Application.Services;

public interface ITrackViewRepository
{
    Task<List<TrackView>> FindByUserId(int id);
}