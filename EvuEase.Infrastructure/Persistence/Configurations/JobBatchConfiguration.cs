using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class JobBatchConfiguration : IEntityTypeConfiguration<JobBatch>
{
    public void Configure(EntityTypeBuilder<JobBatch> builder)
    {
        builder.ToTable("job_batches");

        builder.HasKey(jb => jb.id);
        builder.Property(jb => jb.id).HasColumnName("id").HasMaxLength(255).IsRequired();
        builder.Property(jb => jb.name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(jb => jb.total_jobs).HasColumnName("total_jobs").IsRequired();
        builder.Property(jb => jb.pending_jobs).HasColumnName("pending_jobs").IsRequired();
        builder.Property(jb => jb.failed_jobs).HasColumnName("failed_jobs").IsRequired();
        builder.Property(jb => jb.failed_job_ids).HasColumnName("failed_job_ids").IsRequired();
        builder.Property(jb => jb.options).HasColumnName("options");
        builder.Property(jb => jb.cancelled_at).HasColumnName("cancelled_at");
        builder.Property(jb => jb.created_at).HasColumnName("created_at").IsRequired();
        builder.Property(jb => jb.finished_at).HasColumnName("finished_at");
    }
}

