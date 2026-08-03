using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class PaymentSchemeInstallmentConfiguration : IEntityTypeConfiguration<PaymentSchemeInstallment>
{
    public void Configure(EntityTypeBuilder<PaymentSchemeInstallment> builder)
    {
        builder.ToTable("tbl_payment_scheme_installment");

        builder.HasKey(i => i.id);
        builder.Property(i => i.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(i => i.payment_scheme_id).HasColumnName("payment_scheme_id").IsRequired();
        builder.Property(i => i.installment_order).HasColumnName("installment_order").IsRequired();
        builder.Property(i => i.payment_name).HasColumnName("payment_name").HasMaxLength(200);
        builder.Property(i => i.due_date).HasColumnName("due_date").IsRequired();
        builder.Property(i => i.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(i => i.created_at).HasColumnName("created_at");
        builder.Property(i => i.updated_at).HasColumnName("updated_at");
        builder.Property(i => i.deleted_at).HasColumnName("deleted_at");
        builder.Property(i => i.deleted_by).HasColumnName("deleted_by").HasMaxLength(255);

        builder.HasIndex(i => i.payment_scheme_id);
    }
}
