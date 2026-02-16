using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class SyTermConfiguration : IEntityTypeConfiguration<SyTerm>
{
    public void Configure(EntityTypeBuilder<SyTerm> builder)
    {
        builder.ToTable("tbl_sy_term", t => t.HasCheckConstraint("chk_sy_status", "sy_status IN ('Active','Inactive')"));

        builder.HasKey(st => st.sy_id);
        builder.Property(st => st.sy_id).HasColumnName("sy_id").ValueGeneratedOnAdd();
        builder.Property(st => st.sy_code).HasColumnName("sy_code").HasMaxLength(255).IsRequired();
        builder.Property(st => st.sy_year).HasColumnName("sy_year").HasMaxLength(255).IsRequired();
        builder.Property(st => st.sy_semester).HasColumnName("sy_semester").HasMaxLength(255).IsRequired();
        builder.Property(st => st.sy_startdate).HasColumnName("sy_startdate").IsRequired();
        builder.Property(st => st.sy_enddate).HasColumnName("sy_enddate").IsRequired();
        builder.Property(st => st.sy_enrollmentstart).HasColumnName("sy_enrollmentstart").IsRequired();
        builder.Property(st => st.sy_enrollmentend).HasColumnName("sy_enrollmentend").IsRequired();
        builder.Property(st => st.sy_status).HasColumnName("sy_status").HasMaxLength(10).IsRequired().HasDefaultValue("Inactive");
        builder.Property(st => st.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(st => st.created_at).HasColumnName("created_at");
        builder.Property(st => st.updated_at).HasColumnName("updated_at");
    }
}

