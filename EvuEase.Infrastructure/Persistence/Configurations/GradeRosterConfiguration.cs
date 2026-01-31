using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class GradeRosterConfiguration : IEntityTypeConfiguration<GradeRoster>
{
    public void Configure(EntityTypeBuilder<GradeRoster> builder)
    {
        builder.ToTable("tbl_grade_roster");

        builder.HasKey(gr => gr.id);
        builder.Property(gr => gr.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(gr => gr.created_at).HasColumnName("created_at");
        builder.Property(gr => gr.updated_at).HasColumnName("updated_at");
    }
}

