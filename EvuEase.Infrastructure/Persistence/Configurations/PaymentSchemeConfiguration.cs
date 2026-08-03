using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class PaymentSchemeConfiguration : IEntityTypeConfiguration<PaymentScheme>
{
    public void Configure(EntityTypeBuilder<PaymentScheme> builder)
    {
        builder.ToTable("tbl_payment_scheme");

        builder.HasKey(s => s.id);
        builder.Property(s => s.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(s => s.school_year).HasColumnName("school_year").HasMaxLength(64);
        builder.Property(s => s.semester).HasColumnName("semester").HasMaxLength(32);
        builder.Property(s => s.description).HasColumnName("description").HasMaxLength(500);
        builder.Property(s => s.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(s => s.created_at).HasColumnName("created_at");
        builder.Property(s => s.updated_at).HasColumnName("updated_at");
        builder.Property(s => s.deleted_at).HasColumnName("deleted_at");
        builder.Property(s => s.deleted_by).HasColumnName("deleted_by").HasMaxLength(255);
    }
}
