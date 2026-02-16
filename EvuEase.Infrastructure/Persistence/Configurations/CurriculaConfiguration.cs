using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class CurriculaConfiguration : IEntityTypeConfiguration<Curricula>
{
    public void Configure(EntityTypeBuilder<Curricula> builder)
    {
        builder.ToTable("tbl_curricula");

        builder.HasKey(c => c.id);
        builder.Property(c => c.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(c => c.curriculum_code).HasColumnName("curriculum_code").HasMaxLength(50).IsRequired();
        builder.Property(c => c.version).HasColumnName("version").HasMaxLength(50).IsRequired();
        builder.Property(c => c.program_id).HasColumnName("program_id").IsRequired();
        builder.Property(c => c.sy_id).HasColumnName("sy_id").IsRequired();
        builder.Property(c => c.effective_date).HasColumnName("effective_date").IsRequired();
        builder.Property(c => c.curriculum_status).HasColumnName("curriculum_status").HasMaxLength(50).IsRequired().HasDefaultValue("Inactive");
        builder.Property(c => c.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(c => c.created_at).HasColumnName("created_at");
        builder.Property(c => c.updated_at).HasColumnName("updated_at");
    }
}

