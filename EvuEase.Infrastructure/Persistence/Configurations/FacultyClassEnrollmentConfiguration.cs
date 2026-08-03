using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class FacultyClassEnrollmentConfiguration : IEntityTypeConfiguration<FacultyClassEnrollment>
{
    public void Configure(EntityTypeBuilder<FacultyClassEnrollment> builder)
    {
        builder.ToTable("tbl_faculty_class_enrollment");

        builder.HasKey(e => e.id);
        builder.Property(e => e.id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.faculty_class_id).HasColumnName("faculty_class_id").IsRequired();
        builder.Property(e => e.student_id).HasColumnName("student_id").IsRequired();

        builder.Property(e => e.official_grade).HasColumnName("official_grade").HasMaxLength(32);
        builder.Property(e => e.remarks).HasColumnName("remarks").HasMaxLength(32);

        builder.Property(e => e.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(e => e.created_at).HasColumnName("created_at");
        builder.Property(e => e.updated_at).HasColumnName("updated_at");

        builder.HasIndex(e => new { e.faculty_class_id, e.student_id })
            .IsUnique()
            .HasDatabaseName("UQ_fce_class_student");

        builder.HasIndex(e => e.faculty_class_id).HasDatabaseName("IX_fce_faculty_class_id");

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(e => e.student_id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<FacultyClass>()
            .WithMany()
            .HasForeignKey(e => e.faculty_class_id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
