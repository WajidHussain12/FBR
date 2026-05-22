using FBR_DI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FBR_DI.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.SellerNTNCNIC).IsRequired().HasMaxLength(13);
        builder.Property(i => i.SellerBusinessName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.SellerProvince).IsRequired().HasMaxLength(100);
        builder.Property(i => i.SellerAddress).IsRequired().HasMaxLength(500);
        builder.Property(i => i.BuyerNTNCNIC).HasMaxLength(13);
        builder.Property(i => i.BuyerBusinessName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.BuyerProvince).IsRequired().HasMaxLength(100);
        builder.Property(i => i.BuyerAddress).IsRequired().HasMaxLength(500);
        builder.Property(i => i.InvoiceRefNo).HasMaxLength(50);
        builder.Property(i => i.ScenarioId).HasMaxLength(10);
        builder.Property(i => i.FbrInvoiceNumber).HasMaxLength(50);
        builder.Property(i => i.FbrErrorCode).HasMaxLength(10);
        builder.Property(i => i.FbrErrorMessage).HasMaxLength(500);
        builder.Property(i => i.InvoiceType).HasConversion<int>();
        builder.Property(i => i.BuyerRegistrationType).HasConversion<int>();
        builder.Property(i => i.Status).HasConversion<int>();

        builder.Property(i => i.TotalValueExclST).HasPrecision(18, 2);
        builder.Property(i => i.TotalSalesTax).HasPrecision(18, 2);
        builder.Property(i => i.TotalFurtherTax).HasPrecision(18, 2);
        builder.Property(i => i.TotalExtraTax).HasPrecision(18, 2);
        builder.Property(i => i.TotalFedPayable).HasPrecision(18, 2);
        builder.Property(i => i.TotalDiscount).HasPrecision(18, 2);
        builder.Property(i => i.GrandTotal).HasPrecision(18, 2);

        builder.HasIndex(i => i.FbrInvoiceNumber);
        builder.HasIndex(i => new { i.CompanyId, i.InvoiceDate, i.Status });

        builder.HasMany(i => i.Items)
            .WithOne(item => item.Invoice)
            .HasForeignKey(item => item.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
