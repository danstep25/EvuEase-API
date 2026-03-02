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
        builder.Property(tf => tf.sy_id).HasColumnName("sy_id").HasMaxLength(50);
        builder.Property(tf => tf.batch).HasColumnName("batch").HasMaxLength(10);
        builder.Property(tf => tf.semester).HasColumnName("semester").HasMaxLength(20);
        builder.Property(tf => tf.course_code).HasColumnName("course_code").HasMaxLength(20);
        builder.Property(tf => tf.course_title).HasColumnName("course_title").HasMaxLength(255);
        builder.Property(tf => tf.component).HasColumnName("component").HasMaxLength(50);
        builder.Property(tf => tf.units).HasColumnName("units").HasColumnType("decimal(5,2)");
        builder.Property(tf => tf.cash).HasColumnName("cash").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(tf => tf.low_monthly_payment).HasColumnName("low_monthly_payment").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(tf => tf.status).HasColumnName("status").IsRequired().HasDefaultValue(true);
        builder.Property(tf => tf.created_at).HasColumnName("created_at");
        builder.Property(tf => tf.updated_at).HasColumnName("updated_at");
    }
}

