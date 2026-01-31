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
        builder.Property(osf => osf.created_at).HasColumnName("created_at");
        builder.Property(osf => osf.updated_at).HasColumnName("updated_at");
    }
}

