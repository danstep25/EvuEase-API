using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class GradingSchemeBasisConfiguration : IEntityTypeConfiguration<GradingSchemeBasis>
{
    public void Configure(EntityTypeBuilder<GradingSchemeBasis> builder)
    {
        builder.ToTable("tbl_grading_scheme_basis");

        builder.HasKey(e => e.id);
        builder.Property(e => e.id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.academic_term_key)
            .HasColumnName("academic_term_key")
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(e => e.academic_term_key)
            .IsUnique()
            .HasDatabaseName("uq_grading_scheme_basis_academic_term_key");

        builder.Property(e => e.grading_scheme_code)
            .HasColumnName("grading_scheme_code")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(e => e.grading_scheme_description)
            .HasColumnName("grading_scheme_description")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.grading_basis_code)
            .HasColumnName("grading_basis_code")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(e => e.grading_basis_description)
            .HasColumnName("grading_basis_description")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(e => e.created_at).HasColumnName("created_at");
        builder.Property(e => e.updated_at).HasColumnName("updated_at");
    }
}
