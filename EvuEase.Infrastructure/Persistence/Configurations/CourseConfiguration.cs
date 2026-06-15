using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("tbl_course");

        builder.HasKey(c => c.course_code);
        builder.Property(c => c.course_code).HasColumnName("course_code").HasMaxLength(20).IsRequired();
        builder.Property(c => c.curriculum_id)
            .HasColumnName("curriculum_id")
            .IsRequired()
            .HasConversion<int>(
                v => (int)v,
                v => (long)v);
        builder.Property(c => c.program_id)
            .HasColumnName("program_id")
            .IsRequired()
            .HasConversion<int>(
                v => (int)v,
                v => (long)v);
        builder.Property(c => c.course_title).HasColumnName("course_title").HasMaxLength(50).IsRequired();
        builder.Property(c => c.course_lec_units).HasColumnName("course_lec_units").IsRequired();
        builder.Property(c => c.course_lab_units).HasColumnName("course_lab_units").IsRequired();
        builder.Property(c => c.course_total_units).HasColumnName("course_total_units").IsRequired();
        builder.Property(c => c.course_yearlevel).HasColumnName("course_yearlevel").HasMaxLength(20).IsRequired();
        builder.Property(c => c.course_semester).HasColumnName("course_semester").HasMaxLength(20).IsRequired();
        builder.Property(c => c.course_component).HasColumnName("course_component").HasMaxLength(100);
        builder.Property(c => c.prerequisites).HasColumnName("prerequisites").HasMaxLength(200);
        builder.Property(c => c.description).HasColumnName("description");
        builder.Property(c => c.course_has_prerequities).HasColumnName("course_has_prerequities").IsRequired().HasDefaultValue(0);
        builder.Property(c => c.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(c => c.created_at).HasColumnName("created_at");
        builder.Property(c => c.updated_at).HasColumnName("updated_at");
    }
}

