using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class StudentCurriculumHistoryConfiguration : IEntityTypeConfiguration<StudentCurriculumHistory>
{
    public void Configure(EntityTypeBuilder<StudentCurriculumHistory> builder)
    {
        builder.ToTable("tbl_student_curriculum_history");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.student_id).HasColumnName("student_id").IsRequired();
        builder.Property(x => x.curriculum_code).HasColumnName("curriculum_code").HasMaxLength(50).IsRequired();
        builder.Property(x => x.effective_school_year).HasColumnName("effective_school_year").HasMaxLength(64);
        builder.Property(x => x.reason).HasColumnName("reason").HasMaxLength(500);
        builder.Property(x => x.notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(x => x.migrated_by).HasColumnName("migrated_by").HasMaxLength(200);
        builder.Property(x => x.created_at).HasColumnName("created_at");
    }
}
