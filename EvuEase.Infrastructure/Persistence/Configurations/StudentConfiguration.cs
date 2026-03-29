using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("tbl_students");

        builder.HasKey(s => s.id);
        builder.Property(s => s.id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(s => s.student_number).HasColumnName("student_number").HasMaxLength(50).IsRequired();
        builder.Property(s => s.first_name).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        builder.Property(s => s.last_name).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        builder.Property(s => s.middle_name).HasColumnName("middle_name").HasMaxLength(100);
        builder.Property(s => s.program_code).HasColumnName("program_code").HasMaxLength(50).IsRequired();
        builder.Property(s => s.program_title).HasColumnName("program_title").HasMaxLength(300).IsRequired();
        builder.Property(s => s.year_level).HasColumnName("year_level").HasMaxLength(50).IsRequired();
        builder.Property(s => s.student_type).HasColumnName("student_type").HasMaxLength(50).IsRequired();
        builder.Property(s => s.enrollment_status).HasColumnName("enrollment_status").HasMaxLength(50).IsRequired();

        builder.Property(s => s.address).HasColumnName("address").HasMaxLength(500);
        builder.Property(s => s.contact_number).HasColumnName("contact_number").HasMaxLength(50);
        builder.Property(s => s.email).HasColumnName("email").HasMaxLength(200);
        builder.Property(s => s.gender).HasColumnName("gender").HasMaxLength(20);
        builder.Property(s => s.birthdate).HasColumnName("birthdate").HasColumnType("date");

        builder.Property(s => s.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(s => s.created_at).HasColumnName("created_at");
        builder.Property(s => s.updated_at).HasColumnName("updated_at");
    }
}
