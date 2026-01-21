using mapsence.Domain.Models;
using mapsence.Infrastructure.Configurations;
using mapsence.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace mapsence.Infrastructure.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
    : base(options)
    {
    }
    public DbSet<GeoDataEntity> GeoData => Set<GeoDataEntity>();
    public DbSet<TrackEntity> Tracks => Set<TrackEntity>();
    public DbSet<SensorEntity> Sensors => Set<SensorEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<TrackView> TrackView => Set<TrackView>();
    public DbSet<TrackPointInfo> TrackPointInfo => Set<TrackPointInfo>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new TrackConfiguration());
        modelBuilder.ApplyConfiguration(new SensorConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new GeoDataConfiguration());
        modelBuilder.ApplyConfiguration(new TrackPointInfoConfiguration());
        modelBuilder.ApplyConfiguration(new TrackViewConfiguration());
    }
}
