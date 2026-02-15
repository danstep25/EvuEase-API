using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class FailedJobConfiguration : IEntityTypeConfiguration<FailedJob>
{
    public void Configure(EntityTypeBuilder<FailedJob> builder)
    {
        builder.ToTable("failed_jobs");

        builder.HasKey(fj => fj.id);
        builder.Property(fj => fj.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(fj => fj.uuid).HasColumnName("uuid").HasMaxLength(255).IsRequired();
        builder.HasIndex(fj => fj.uuid).IsUnique();
        builder.Property(fj => fj.connection).HasColumnName("connection").IsRequired();
        builder.Property(fj => fj.queue).HasColumnName("queue").IsRequired();
        builder.Property(fj => fj.payload).HasColumnName("payload").IsRequired();
        builder.Property(fj => fj.exception).HasColumnName("exception").IsRequired();
        builder.Property(fj => fj.failed_at).HasColumnName("failed_at").IsRequired().HasDefaultValueSql("SYSDATETIME()");
    }
}

