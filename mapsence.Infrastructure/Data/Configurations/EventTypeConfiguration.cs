using mapsence.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mapsence.Infrastructure.Configurations;

public class EventTypeConfiguration : IEntityTypeConfiguration<EventTypeEntity>
{
    public void Configure(EntityTypeBuilder<EventTypeEntity> builder)
    {
        builder.ToTable("event_type", "zxc");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Code)
            .HasColumnName("code");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(100);

        builder.HasIndex(e => e.Code)
            .IsUnique();
    }
}
