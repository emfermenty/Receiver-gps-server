using mapsence.Domain.Models;
using mapsence.Infrastructure.Data;
using mapsence.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace mapsencetoWebApi.Repository;

public class TrackViewRepository(ApplicationContext context) : ITrackViewRepository
{
    public async Task<List<TrackView>> FindByUserId(int userid)
    {
        return await context.TrackView
            .Where(t => t.UserId == userid)
            .ToListAsync();
    }
}