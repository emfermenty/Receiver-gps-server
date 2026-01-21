using mapsence.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mapsence.Infrastructure.Configurations;

public class TrackConfiguration : IEntityTypeConfiguration<TrackEntity>
{
    public void Configure(EntityTypeBuilder<TrackEntity> builder)
    {
        builder.ToTable(name: "track", schema: "zxc");
        builder.Metadata.SetIsTableExcludedFromMigrations(false);
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.DateStart)
            .HasColumnName("date_start")
            .HasColumnType("timestamp without time zone")
            .HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified), 
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified)  
            );

        builder.Property(e => e.DateStop)
            .HasColumnName("date_stop")
            .HasColumnType("timestamp without time zone");;

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(255);
        builder.Property(e => e.SensorId)
            .HasColumnName("sensor_id");
        builder.Property(e => e.UserId)
            .HasColumnName("user_id");
        builder.HasOne(t => t.Sensor)
            .WithMany(u => u.Tracks)
            .HasForeignKey(t => t.SensorId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.User)
            .WithMany(u => u.Tracks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}