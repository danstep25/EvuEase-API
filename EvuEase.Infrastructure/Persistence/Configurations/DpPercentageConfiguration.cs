using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class DpPercentageConfiguration : IEntityTypeConfiguration<DpPercentage>
{
    public void Configure(EntityTypeBuilder<DpPercentage> builder)
    {
        builder.ToTable("tbl_dp_percentage");

        builder.HasKey(dpp => dpp.id);
        builder.Property(dpp => dpp.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(dpp => dpp.program_code).HasColumnName("program_code").HasMaxLength(32);
        builder.Property(dpp => dpp.program_title).HasColumnName("program_title").HasMaxLength(200);
        builder.Property(dpp => dpp.batch).HasColumnName("batch").HasMaxLength(32);
        builder.Property(dpp => dpp.downpayment_percent).HasColumnName("downpayment_percent").HasPrecision(5, 2);
        builder.Property(dpp => dpp.effective_school_year).HasColumnName("effective_school_year").HasMaxLength(64);
        builder.Property(dpp => dpp.created_by).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(dpp => dpp.updated_by).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(dpp => dpp.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(dpp => dpp.created_at).HasColumnName("created_at");
        builder.Property(dpp => dpp.updated_at).HasColumnName("updated_at");
    }
}
