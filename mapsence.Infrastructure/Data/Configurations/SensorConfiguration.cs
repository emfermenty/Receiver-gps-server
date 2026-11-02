using mapsence.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mapsence.Infrastructure.Configurations;

public class SensorConfiguration : IEntityTypeConfiguration<SensorEntity>
{
    public void Configure(EntityTypeBuilder<SensorEntity> builder)
    {
        builder.ToTable("sensor", "zxc");
        builder.Metadata.SetIsTableExcludedFromMigrations(false);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(e => e.UserId)
            .HasColumnName("user_id");
        builder.Property(e => e.ModelId)
            .HasColumnName("model_id");
        builder.Property(e => e.Imei)
            .HasColumnName("imei")
            .HasMaxLength(15);
        builder.HasIndex(e => e.Imei)
            .IsUnique();
        builder.HasOne(s => s.User)
            .WithMany(u => u.Sensors)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(s => s.Model)
            .WithMany(m => m.Sensors)
            .HasForeignKey(s => s.ModelId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
