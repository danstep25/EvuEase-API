using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class CreditRequestLineConfiguration : IEntityTypeConfiguration<CreditRequestLine>
{
    public void Configure(EntityTypeBuilder<CreditRequestLine> builder)
    {
        builder.ToTable("tbl_credit_request_line");

        builder.HasKey(e => e.id);
        builder.Property(e => e.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.credit_request_id).HasColumnName("credit_request_id").IsRequired();
        builder.Property(e => e.sort_order).HasColumnName("sort_order").IsRequired();
        builder.Property(e => e.applied_course_code).HasColumnName("applied_course_code").HasMaxLength(32);
        builder.Property(e => e.applied_course_title).HasColumnName("applied_course_title").HasMaxLength(200);
        builder.Property(e => e.applied_lec_units).HasColumnName("applied_lec_units").HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(e => e.applied_lab_units).HasColumnName("applied_lab_units").HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(e => e.grade).HasColumnName("grade").HasMaxLength(16);
        builder.Property(e => e.equivalent_course_code).HasColumnName("equivalent_course_code").HasMaxLength(32);

        builder.Property(e => e.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(e => e.created_at).HasColumnName("created_at");
        builder.Property(e => e.updated_at).HasColumnName("updated_at");
        builder.Property(e => e.deleted_at).HasColumnName("deleted_at");
        builder.Property(e => e.deleted_by).HasColumnName("deleted_by").HasMaxLength(128);

        builder.HasIndex(e => e.credit_request_id).HasDatabaseName("ix_credit_request_line_request_id");

        builder.HasOne<CreditRequest>()
            .WithMany()
            .HasForeignKey(e => e.credit_request_id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
