using FBR_DI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FBR_DI.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.NTN).IsRequired().HasMaxLength(13);
        builder.Property(c => c.CNIC).HasMaxLength(13);
        builder.Property(c => c.Province).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Address).IsRequired().HasMaxLength(500);
        builder.Property(c => c.PhoneNumber).HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.BusinessActivity).HasMaxLength(100);
        builder.Property(c => c.Sector).HasMaxLength(100);
        builder.Property(c => c.FbrBearerToken).HasMaxLength(500).HasColumnType("nvarchar(500)");
        builder.Property(c => c.SubmissionEnvironment).HasConversion<int>();

        builder.HasIndex(c => c.NTN).IsUnique();

        builder.HasMany(c => c.Invoices)
            .WithOne(i => i.Company)
            .HasForeignKey(i => i.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
