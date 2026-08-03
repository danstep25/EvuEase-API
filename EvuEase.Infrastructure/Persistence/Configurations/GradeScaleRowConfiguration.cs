using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class GradeScaleRowConfiguration : IEntityTypeConfiguration<GradeScaleRow>
{
    public void Configure(EntityTypeBuilder<GradeScaleRow> builder)
    {
        builder.ToTable("tbl_grade_scale_row");

        builder.HasKey(e => e.id);
        builder.Property(e => e.id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.academic_term_key)
            .HasColumnName("academic_term_key")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(e => e.mark).HasColumnName("mark").HasColumnType("decimal(9,4)").IsRequired();
        builder.Property(e => e.grade).HasColumnName("grade").HasColumnType("decimal(9,4)").IsRequired();
        builder.Property(e => e.sort_order).HasColumnName("sort_order").IsRequired();

        builder.HasIndex(e => e.academic_term_key).HasDatabaseName("ix_grade_scale_row_academic_term_key");

        builder.Property(e => e.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(e => e.created_at).HasColumnName("created_at");
        builder.Property(e => e.updated_at).HasColumnName("updated_at");
    }
}
