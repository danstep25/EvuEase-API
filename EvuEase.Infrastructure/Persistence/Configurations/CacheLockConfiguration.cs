using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class CacheLockConfiguration : IEntityTypeConfiguration<CacheLock>
{
    public void Configure(EntityTypeBuilder<CacheLock> builder)
    {
        builder.ToTable("cache_locks");

        builder.HasKey(cl => cl.key);
        builder.Property(cl => cl.key).HasColumnName("key").HasMaxLength(255).IsRequired();
        builder.Property(cl => cl.owner).HasColumnName("owner").HasMaxLength(255).IsRequired();
        builder.Property(cl => cl.expiration).HasColumnName("expiration").IsRequired();
    }
}

