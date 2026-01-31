using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(s => s.id);
        builder.Property(s => s.id).HasColumnName("id").HasMaxLength(255).IsRequired();
        builder.Property(s => s.user_id).HasColumnName("user_id");
        builder.Property(s => s.ip_address).HasColumnName("ip_address").HasMaxLength(45);
        builder.Property(s => s.user_agent).HasColumnName("user_agent");
        builder.Property(s => s.payload).HasColumnName("payload").IsRequired();
        builder.Property(s => s.last_activity).HasColumnName("last_activity").IsRequired();
    }
}

