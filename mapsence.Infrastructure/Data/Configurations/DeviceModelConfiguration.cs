using mapsence.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mapsence.Infrastructure.Configurations;

public class DeviceModelConfiguration : IEntityTypeConfiguration<DeviceModelEntity>
{
    public void Configure(EntityTypeBuilder<DeviceModelEntity> builder)
    {
        builder.ToTable("device_model", "zxc");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Manufacturer)
            .HasColumnName("manufacturer")
            .HasMaxLength(100);

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(100);

        builder.HasIndex(e => new { e.Manufacturer, e.Name })
            .IsUnique();
    }
}
