using FBR_DI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FBR_DI.Persistence.Configurations;

public class FbrReferenceDataConfiguration : IEntityTypeConfiguration<FbrReferenceData>
{
    public void Configure(EntityTypeBuilder<FbrReferenceData> builder)
    {
        builder.ToTable("FbrReferenceDatas");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReferenceType).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Code).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Description).IsRequired().HasMaxLength(500);
        builder.Property(r => r.ParentCode).HasMaxLength(50);

        builder.HasIndex(r => new { r.ReferenceType, r.Code });
    }
}
