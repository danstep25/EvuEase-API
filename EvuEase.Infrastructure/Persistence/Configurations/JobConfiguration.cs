using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("jobs");

        builder.HasKey(j => j.id);
        builder.Property(j => j.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(j => j.queue).HasColumnName("queue").HasMaxLength(255).IsRequired();
        builder.Property(j => j.payload).HasColumnName("payload").IsRequired();
        builder.Property(j => j.attempts).HasColumnName("attempts").IsRequired();
        builder.Property(j => j.reserved_at).HasColumnName("reserved_at");
        builder.Property(j => j.available_at).HasColumnName("available_at").IsRequired();
        builder.Property(j => j.created_at).HasColumnName("created_at").IsRequired();
    }
}

