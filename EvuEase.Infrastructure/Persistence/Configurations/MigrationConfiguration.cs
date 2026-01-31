using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class MigrationConfiguration : IEntityTypeConfiguration<Migration>
{
    public void Configure(EntityTypeBuilder<Migration> builder)
    {
        builder.ToTable("migrations");

        builder.HasKey(m => m.id);
        builder.Property(m => m.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(m => m.migration).HasColumnName("migration").HasMaxLength(255).IsRequired();
        builder.Property(m => m.batch).HasColumnName("batch").IsRequired();
    }
}

