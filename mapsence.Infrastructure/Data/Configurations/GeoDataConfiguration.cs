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

        builder.Property(e => e.SourceId)
            .HasColumnName("source_id");

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

        // UTC без часового пояса, как date_start/date_stop у track: вьюхи сравнивают их напрямую
        builder.Property(e => e.DateTime)
            .HasColumnName("date_time")
            .HasColumnType("timestamp without time zone")
            .HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified)
            );

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
        builder.Property(e => e.EventTypeId)
            .HasColumnName("event_type_id");

        builder.Property(e => e.GeometryBytes)
            .HasColumnName("geom")
            .HasColumnType("geometry (Point, 4326)");
        builder.HasIndex(e => e.SourceId)
            .IsUnique();
        builder.HasOne(g => g.Sensor)
            .WithMany(s => s.GeoData)
            .HasForeignKey(g => g.SensorId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(g => g.EventType)
            .WithMany(e => e.GeoData)
            .HasForeignKey(g => g.EventTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
