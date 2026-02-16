using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class ProgramConfiguration : IEntityTypeConfiguration<Program>
{
    public void Configure(EntityTypeBuilder<Program> builder)
    {
        builder.ToTable("tbl_program");

        builder.HasKey(p => p.program_id);
        builder.Property(p => p.program_id).HasColumnName("program_id").ValueGeneratedOnAdd();
        builder.Property(p => p.program_code).HasColumnName("program_code").HasMaxLength(50).IsRequired();
        builder.Property(p => p.program_title).HasColumnName("program_title").HasMaxLength(50).IsRequired();
        builder.Property(p => p.program_completionyears).HasColumnName("program_completionyears").IsRequired();
        builder.Property(p => p.program_totalunits).HasColumnName("program_totalunits");
        builder.Property(p => p.program_status).HasColumnName("program_status").HasMaxLength(50).IsRequired().HasDefaultValue("active");
        builder.Property(p => p.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(p => p.created_at).HasColumnName("created_at");
        builder.Property(p => p.updated_at).HasColumnName("updated_at");
    }
}

