using mapsence.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mapsence.Infrastructure.Configurations;

public class SensorSessionConfiguration : IEntityTypeConfiguration<SensorSessionEntity>
{
    public void Configure(EntityTypeBuilder<SensorSessionEntity> builder)
    {
        builder.ToTable("sensor_session", "zxc");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.SensorId)
            .HasColumnName("sensor_id");

        builder.Property(e => e.Ip)
            .HasColumnName("ip")
            .HasMaxLength(45);

        builder.Property(e => e.Firmware)
            .HasColumnName("firmware")
            .HasMaxLength(50);

        builder.Property(e => e.StartedAt)
            .HasColumnName("started_at")
            .HasColumnType("timestamp without time zone")
            .HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified)
            );

        builder.Property(e => e.LastSeenAt)
            .HasColumnName("last_seen_at")
            .HasColumnType("timestamp without time zone")
            .HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified)
            );

        builder.HasIndex(e => new { e.SensorId, e.StartedAt })
            .IsUnique();

        builder.HasOne(e => e.Sensor)
            .WithMany(s => s.Sessions)
            .HasForeignKey(e => e.SensorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
