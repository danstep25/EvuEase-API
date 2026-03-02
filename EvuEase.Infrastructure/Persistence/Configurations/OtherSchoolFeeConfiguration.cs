using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class OtherSchoolFeeConfiguration : IEntityTypeConfiguration<OtherSchoolFee>
{
    public void Configure(EntityTypeBuilder<OtherSchoolFee> builder)
    {
        builder.ToTable("tbl_other_school_fees");

        builder.HasKey(osf => osf.id);
        builder.Property(osf => osf.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(osf => osf.sy_id).HasColumnName("sy_id").HasMaxLength(50);
        builder.Property(osf => osf.batch).HasColumnName("batch").HasMaxLength(10);
        builder.Property(osf => osf.semester).HasColumnName("semester").HasMaxLength(20);
        builder.Property(osf => osf.school_fee).HasColumnName("school_fee").HasMaxLength(255);
        builder.Property(osf => osf.cash).HasColumnName("cash").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(osf => osf.low_monthly_payment).HasColumnName("low_monthly_payment").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(osf => osf.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(osf => osf.created_at).HasColumnName("created_at");
        builder.Property(osf => osf.updated_at).HasColumnName("updated_at");
    }
}

