using mapsence.Domain.Models;
using mapsence.Infrastructure.Data;
using mapsenceto.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace mapsence.Infrastructure.Repositories;

public class TrackPointInfoRepository(ApplicationContext context) : ITrackPointInfoRepository
{
    public async Task<List<TrackPointInfo>> FindByTrackIdOrderByDateTime(int trackId)
    {
        return await context.TrackPointInfo
            .Where(t => t.TrackId == trackId)
            .OrderBy(t => t.DateTime)
            .ToListAsync();
    }
}