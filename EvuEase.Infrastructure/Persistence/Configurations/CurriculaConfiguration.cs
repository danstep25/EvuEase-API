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
        builder.Property(c => c.created_at).HasColumnName("created_at");
        builder.Property(c => c.updated_at).HasColumnName("updated_at");
    }
}

