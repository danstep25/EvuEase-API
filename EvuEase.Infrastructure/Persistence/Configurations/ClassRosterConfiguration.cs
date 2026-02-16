using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class ClassRosterConfiguration : IEntityTypeConfiguration<ClassRoster>
{
    public void Configure(EntityTypeBuilder<ClassRoster> builder)
    {
        builder.ToTable("tbl_class_roster");

        builder.HasKey(cr => cr.id);
        builder.Property(cr => cr.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(cr => cr.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(cr => cr.created_at).HasColumnName("created_at");
        builder.Property(cr => cr.updated_at).HasColumnName("updated_at");
    }
}

