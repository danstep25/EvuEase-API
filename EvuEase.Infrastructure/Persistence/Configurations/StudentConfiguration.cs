using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("tbl_students");

        builder.HasKey(s => s.id);
        builder.Property(s => s.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(s => s.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(s => s.created_at).HasColumnName("created_at");
        builder.Property(s => s.updated_at).HasColumnName("updated_at");
    }
}

