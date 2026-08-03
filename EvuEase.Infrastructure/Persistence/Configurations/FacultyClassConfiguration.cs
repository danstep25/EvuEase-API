using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class FacultyClassConfiguration : IEntityTypeConfiguration<FacultyClass>
{
    public void Configure(EntityTypeBuilder<FacultyClass> builder)
    {
        builder.ToTable("tbl_faculty_class");

        builder.HasKey(e => e.id);
        builder.Property(e => e.id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(e => e.course_code).HasColumnName("course_code").HasMaxLength(32).IsRequired();
        builder.Property(e => e.class_number).HasColumnName("class_number").HasMaxLength(32).IsRequired();
        builder.Property(e => e.section).HasColumnName("section").HasMaxLength(64).IsRequired();
        builder.Property(e => e.course_title).HasColumnName("course_title").HasMaxLength(300).IsRequired();
        builder.Property(e => e.component).HasColumnName("component").HasMaxLength(64).IsRequired();
        builder.Property(e => e.academic_term).HasColumnName("academic_term").HasMaxLength(200).IsRequired();
        builder.Property(e => e.enrolled_count).HasColumnName("enrolled_count").IsRequired();
        builder.Property(e => e.program_code).HasColumnName("program_code").HasMaxLength(32).IsRequired();
        builder.Property(e => e.year_level).HasColumnName("year_level").HasMaxLength(64).IsRequired();

        builder.Property(e => e.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(e => e.created_at).HasColumnName("created_at");
        builder.Property(e => e.updated_at).HasColumnName("updated_at");
    }
}
