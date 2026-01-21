using mapsence.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mapsence.Infrastructure.Configurations;

public class GeoDataConfiguration : IEntityTypeConfiguration<GeoDataEntity>
{
    public void Configure(EntityTypeBuilder<GeoDataEntity> builder)
    {
        builder.ToTable("geo_data", "zxc");
        builder.Metadata.SetIsTableExcludedFromMigrations(false);
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
            
        builder.Property(e => e.Latitude)
            .HasColumnName("latitude")
            .HasColumnType("double precision");
            
        builder.Property(e => e.Longitude)
            .HasColumnName("longitude")
            .HasColumnType("double precision");
            
        builder.Property(e => e.Height)
            .HasColumnName("height")
            .HasColumnType("double precision");
            
        builder.Property(e => e.Speed)
            .HasColumnName("speed")
            .HasColumnType("double precision");
            
        builder.Property(e => e.DateTime)
            .HasColumnName("date_time");
            
        builder.Property(e => e.Course)
            .HasColumnName("course")
            .HasColumnType("double precision");
            
        builder.Property(e => e.BatteryVoltage)
            .HasColumnName("battery_voltage")
            .HasColumnType("double precision");
            
        builder.Property(e => e.SatelliteCount)
            .HasColumnName("satellite_count");
            
        builder.Property(e => e.SensorId)
            .HasColumnName("sensor_id");
        builder.Property(e => e.User_id)
            .HasColumnName("user_id");

        builder.Property(e => e.GeometryBytes)
            .HasColumnName("geom")
            .HasColumnType("geometry (Point, 4326)");
        builder.HasOne(g => g.Sensor)
            .WithMany(s => s.GeoData)
            .HasForeignKey(g => g.SensorId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(g => g.User)
            .WithMany(u => u.GeoData)
            .HasForeignKey(g => g.User_id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}