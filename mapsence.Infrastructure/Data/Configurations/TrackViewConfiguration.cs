
using mapsence.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mapsence.Infrastructure.Configurations;

public class TrackViewConfiguration : IEntityTypeConfiguration<TrackView>
{
    public void Configure(EntityTypeBuilder<TrackView> builder)
    {
        builder.ToView("v_tracks_view", "zxc");
        // [Table] на модели добавляет ещё и маппинг на таблицу; вьюха создаётся SQL-ом в миграции
        builder.Metadata.SetIsTableExcludedFromMigrations(true);
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
        
        builder.Property(e => e.Name)
            .HasColumnName("name");
            
        builder.Property(e => e.DateStart)
            .HasColumnName("date_start");
            
        builder.Property(e => e.DateStop)
            .HasColumnName("date_stop");
            
        builder.Property(e => e.SensorId)
            .HasColumnName("sensor_id");
            
        builder.Property(e => e.UserId)
            .HasColumnName("user_id");
            
        builder.Property(e => e.Distance)
            .HasColumnName("distance");
            
        builder.Property(e => e.AvgSpeed)
            .HasColumnName("avg_speed");
            
        builder.Property(e => e.Duration)
            .HasColumnName("duration");
    }
}