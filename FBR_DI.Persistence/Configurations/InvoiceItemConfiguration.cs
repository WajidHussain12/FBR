using FBR_DI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FBR_DI.Persistence.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.HsCode).IsRequired().HasMaxLength(20);
        builder.Property(i => i.ProductDescription).IsRequired().HasMaxLength(500);
        builder.Property(i => i.Rate).IsRequired().HasMaxLength(20);
        builder.Property(i => i.UoM).IsRequired().HasMaxLength(100);
        builder.Property(i => i.SaleType).IsRequired().HasMaxLength(100);
        builder.Property(i => i.SroScheduleNo).HasMaxLength(50);
        builder.Property(i => i.SroItemSerialNo).HasMaxLength(20);
        builder.Property(i => i.FbrItemInvoiceNo).HasMaxLength(60);
        builder.Property(i => i.ItemStatus).HasMaxLength(20);
        builder.Property(i => i.ItemErrorCode).HasMaxLength(10);
        builder.Property(i => i.ItemErrorMessage).HasMaxLength(500);

        builder.Property(i => i.Quantity).HasPrecision(18, 4);
        builder.Property(i => i.TotalValues).HasPrecision(18, 2);
        builder.Property(i => i.ValueSalesExcludingST).HasPrecision(18, 2);
        builder.Property(i => i.FixedNotifiedValueOrRetailPrice).HasPrecision(18, 2);
        builder.Property(i => i.SalesTaxApplicable).HasPrecision(18, 2);
        builder.Property(i => i.SalesTaxWithheldAtSource).HasPrecision(18, 2);
        builder.Property(i => i.ExtraTax).HasPrecision(18, 2);
        builder.Property(i => i.FurtherTax).HasPrecision(18, 2);
        builder.Property(i => i.FedPayable).HasPrecision(18, 2);
        builder.Property(i => i.Discount).HasPrecision(18, 2);
    }
}
