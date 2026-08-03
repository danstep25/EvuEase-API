using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class MiscellaneousFeeConfiguration : IEntityTypeConfiguration<MiscellaneousFee>
{
    public void Configure(EntityTypeBuilder<MiscellaneousFee> builder)
    {
        builder.ToTable("tbl_miscellaneous_fees");

        builder.HasKey(mf => mf.id);
        builder.Property(mf => mf.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(mf => mf.sy_id).HasColumnName("sy_id").HasMaxLength(50);
        builder.Property(mf => mf.batch).HasColumnName("batch").HasMaxLength(10);
        builder.Property(mf => mf.semester).HasColumnName("semester").HasMaxLength(20);
        builder.Property(mf => mf.miscellaneous_fee).HasColumnName("miscellaneous_fee").HasMaxLength(255);
        builder.Property(mf => mf.cash).HasColumnName("cash").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(mf => mf.low_monthly_payment).HasColumnName("low_monthly_payment").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(mf => mf.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(mf => mf.created_at).HasColumnName("created_at");
        builder.Property(mf => mf.updated_at).HasColumnName("updated_at");
    }
}

