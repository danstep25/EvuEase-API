using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class CreditRequestConfiguration : IEntityTypeConfiguration<CreditRequest>
{
    public void Configure(EntityTypeBuilder<CreditRequest> builder)
    {
        builder.ToTable("tbl_credit_request");

        builder.HasKey(e => e.id);
        builder.Property(e => e.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.credit_request_no).HasColumnName("credit_request_no").HasMaxLength(32).IsRequired();
        builder.Property(e => e.student_id).HasColumnName("student_id");
        builder.Property(e => e.student_number).HasColumnName("student_number").HasMaxLength(50).IsRequired();
        builder.Property(e => e.first_name).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.middle_name).HasColumnName("middle_name").HasMaxLength(100);
        builder.Property(e => e.last_name).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.program_id).HasColumnName("program_id").IsRequired();
        builder.Property(e => e.sy_id).HasColumnName("sy_id").IsRequired();
        builder.Property(e => e.request_status).HasColumnName("request_status").HasMaxLength(32).IsRequired();
        builder.Property(e => e.signed_pdf_file_name).HasColumnName("signed_pdf_file_name").HasMaxLength(260);
        builder.Property(e => e.signed_pdf_storage_key).HasColumnName("signed_pdf_storage_key").HasMaxLength(500);

        builder.Property(e => e.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(e => e.created_at).HasColumnName("created_at");
        builder.Property(e => e.updated_at).HasColumnName("updated_at");
        builder.Property(e => e.deleted_at).HasColumnName("deleted_at");
        builder.Property(e => e.deleted_by).HasColumnName("deleted_by").HasMaxLength(128);

        builder.HasIndex(e => e.credit_request_no).IsUnique().HasDatabaseName("ux_credit_request_no");
    }
}
