using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class SubjectEvaluationAuditConfiguration : IEntityTypeConfiguration<SubjectEvaluationAudit>
{
    public void Configure(EntityTypeBuilder<SubjectEvaluationAudit> builder)
    {
        builder.ToTable("tbl_subject_evaluation_audit");

        builder.HasKey(e => e.id);
        builder.Property(e => e.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.student_id).HasColumnName("student_id").IsRequired();
        builder.Property(e => e.student_number).HasColumnName("student_number").HasMaxLength(50).IsRequired();
        builder.Property(e => e.student_name).HasColumnName("student_name").HasMaxLength(255).IsRequired();
        builder.Property(e => e.program_code).HasColumnName("program_code").HasMaxLength(50).IsRequired();
        builder.Property(e => e.program_year_level).HasColumnName("program_year_level").HasMaxLength(100).IsRequired();
        builder.Property(e => e.school_year).HasColumnName("school_year").HasMaxLength(50).IsRequired();
        builder.Property(e => e.semester).HasColumnName("semester").HasMaxLength(50).IsRequired();
        builder.Property(e => e.school_year_term).HasColumnName("school_year_term").HasMaxLength(200).IsRequired();
        builder.Property(e => e.total_units_selected).HasColumnName("total_units_selected").IsRequired();
        builder.Property(e => e.evaluated_by).HasColumnName("evaluated_by").HasMaxLength(128).IsRequired();
        builder.Property(e => e.evaluated_at).HasColumnName("evaluated_at").IsRequired();
        builder.Property(e => e.evaluation_payload).HasColumnName("evaluation_payload").HasColumnType("nvarchar(max)").IsRequired();

        builder.Property(e => e.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(e => e.created_at).HasColumnName("created_at");
        builder.Property(e => e.updated_at).HasColumnName("updated_at");
        builder.Property(e => e.deleted_at).HasColumnName("deleted_at");
        builder.Property(e => e.deleted_by).HasColumnName("deleted_by").HasMaxLength(128);

        builder.HasIndex(e => e.evaluated_at).HasDatabaseName("ix_subject_evaluation_audit_evaluated_at");
        builder.HasIndex(e => e.student_id).HasDatabaseName("ix_subject_evaluation_audit_student_id");
    }
}
