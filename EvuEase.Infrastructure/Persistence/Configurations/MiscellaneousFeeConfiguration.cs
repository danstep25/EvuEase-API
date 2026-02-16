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
        builder.Property(mf => mf.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(mf => mf.created_at).HasColumnName("created_at");
        builder.Property(mf => mf.updated_at).HasColumnName("updated_at");
    }
}

