using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.id);
        builder.Property(u => u.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(u => u.name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(u => u.email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.HasIndex(u => u.email).IsUnique();
        builder.Property(u => u.email_verified_at).HasColumnName("email_verified_at");
        builder.Property(u => u.password).HasColumnName("password").HasMaxLength(255).IsRequired();
        builder.Property(u => u.remember_token).HasColumnName("remember_token").HasMaxLength(100);
        builder.Property(u => u.created_at).HasColumnName("created_at");
        builder.Property(u => u.updated_at).HasColumnName("updated_at");
        builder.Property(u => u.role).HasColumnName("role").HasMaxLength(255).IsRequired().HasDefaultValue("evaluator");
    }
}

