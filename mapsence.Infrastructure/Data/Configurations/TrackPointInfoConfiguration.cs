
using mapsence.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mapsence.Infrastructure.Configurations;

public class TrackPointInfoConfiguration : IEntityTypeConfiguration<TrackPointInfo>
{
    public void Configure(EntityTypeBuilder<TrackPointInfo> builder)
    {
        builder.ToView("v_track_info", "zxc");
        builder.HasKey(e => e.Id);
            
        builder.Property(e => e.Id)
            .HasColumnName("id");
            
        builder.Property(e => e.Latitude)
            .HasColumnName("latitude");
            
        builder.Property(e => e.Longitude)
            .HasColumnName("longitude");
            
        builder.Property(e => e.Height)
            .HasColumnName("height");
            
        builder.Property(e => e.Speed)
            .HasColumnName("speed");
            
        builder.Property(e => e.DateTime)
            .HasColumnName("date_time");
            
        builder.Property(e => e.SensorId)
            .HasColumnName("sensor_id");
            
        builder.Property(e => e.TrackId)
            .HasColumnName("track_id");
    }
}