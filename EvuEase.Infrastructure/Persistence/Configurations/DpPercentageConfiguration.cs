using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class DpPercentageConfiguration : IEntityTypeConfiguration<DpPercentage>
{
    public void Configure(EntityTypeBuilder<DpPercentage> builder)
    {
        builder.ToTable("tbl_dp_percentage");

        builder.HasKey(dpp => dpp.id);
        builder.Property(dpp => dpp.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(dpp => dpp.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(dpp => dpp.created_at).HasColumnName("created_at");
        builder.Property(dpp => dpp.updated_at).HasColumnName("updated_at");
    }
}

