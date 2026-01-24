using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class CacheConfiguration : IEntityTypeConfiguration<Cache>
{
    public void Configure(EntityTypeBuilder<Cache> builder)
    {
        builder.ToTable("cache");

        builder.HasKey(c => c.key);
        builder.Property(c => c.key).HasColumnName("key").HasMaxLength(255).IsRequired();
        builder.Property(c => c.value).HasColumnName("value").IsRequired();
        builder.Property(c => c.expiration).HasColumnName("expiration").IsRequired();
    }
}

