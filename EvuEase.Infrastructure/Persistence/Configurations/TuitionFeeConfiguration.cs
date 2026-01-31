using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvuEase.Domain.Entities;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class TuitionFeeConfiguration : IEntityTypeConfiguration<TuitionFee>
{
    public void Configure(EntityTypeBuilder<TuitionFee> builder)
    {
        builder.ToTable("tbl_tuition_fees");

        builder.HasKey(tf => tf.id);
        builder.Property(tf => tf.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(tf => tf.created_at).HasColumnName("created_at");
        builder.Property(tf => tf.updated_at).HasColumnName("updated_at");
    }
}

