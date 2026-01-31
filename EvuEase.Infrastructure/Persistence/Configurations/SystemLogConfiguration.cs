using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
{
    public void Configure(EntityTypeBuilder<SystemLog> builder)
    {
        builder.ToTable("system_logs");

        builder.HasKey(sl => sl.log_id);
        builder.Property(sl => sl.log_id).HasColumnName("log_id").ValueGeneratedOnAdd();
        builder.Property(sl => sl.user).HasColumnName("user").HasMaxLength(50).IsRequired();
        builder.Property(sl => sl.role).HasColumnName("role").HasMaxLength(50).IsRequired();
        builder.Property(sl => sl.action).HasColumnName("action").HasMaxLength(50).IsRequired();
        builder.Property(sl => sl.timestamp).HasColumnName("timestamp").IsRequired();
        builder.Property(sl => sl.module).HasColumnName("module").HasMaxLength(50).IsRequired();
        builder.Property(sl => sl.details).HasColumnName("details").HasMaxLength(50).IsRequired();
        builder.Property(sl => sl.ip_address).HasColumnName("ip_address").HasMaxLength(50);
    }
}

