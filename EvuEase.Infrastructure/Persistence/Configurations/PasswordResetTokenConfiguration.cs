using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("password_reset_tokens");

        builder.HasKey(prt => prt.email);
        builder.Property(prt => prt.email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(prt => prt.token).HasColumnName("token").HasMaxLength(255).IsRequired();
        builder.Property(prt => prt.created_at).HasColumnName("created_at");
    }
}

