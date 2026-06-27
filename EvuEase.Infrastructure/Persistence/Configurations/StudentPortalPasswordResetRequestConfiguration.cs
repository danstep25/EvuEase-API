using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvuEase.Infrastructure.Persistence.Configurations;

public class StudentPortalPasswordResetRequestConfiguration : IEntityTypeConfiguration<StudentPortalPasswordResetRequest>
{
    public void Configure(EntityTypeBuilder<StudentPortalPasswordResetRequest> builder)
    {
        builder.ToTable("tbl_student_portal_password_reset_requests");

        builder.HasKey(r => r.id);
        builder.Property(r => r.id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(r => r.student_id).HasColumnName("student_id").IsRequired();
        builder.Property(r => r.student_number).HasColumnName("student_number").HasMaxLength(50).IsRequired();
        builder.Property(r => r.reason).HasColumnName("reason").HasMaxLength(500);
        builder.Property(r => r.status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(r => r.registrar_notes).HasColumnName("registrar_notes").HasMaxLength(500);
        builder.Property(r => r.resolved_by).HasColumnName("resolved_by").HasMaxLength(200);
        builder.Property(r => r.requested_at).HasColumnName("requested_at");
        builder.Property(r => r.resolved_at).HasColumnName("resolved_at");
    }
}
