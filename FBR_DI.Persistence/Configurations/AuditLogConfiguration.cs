using FBR_DI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FBR_DI.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId).IsRequired().HasMaxLength(450);
        builder.Property(a => a.UserEmail).IsRequired().HasMaxLength(256);
        builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntityName).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntityId).HasMaxLength(50);
        builder.Property(a => a.IpAddress).HasMaxLength(50);

        builder.HasIndex(a => new { a.UserId, a.Timestamp });
    }
}
