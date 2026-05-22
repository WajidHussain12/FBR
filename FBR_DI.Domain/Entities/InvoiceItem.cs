using FBR_DI.Domain.Common;

namespace FBR_DI.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public int ItemSerialNo { get; set; }
    public string HsCode { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
    public string Rate { get; set; } = string.Empty;
    public string UoM { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal TotalValues { get; set; }
    public decimal ValueSalesExcludingST { get; set; }
    public decimal FixedNotifiedValueOrRetailPrice { get; set; }
    public decimal SalesTaxApplicable { get; set; }
    public decimal SalesTaxWithheldAtSource { get; set; }
    public decimal ExtraTax { get; set; } = 0;
    public decimal FurtherTax { get; set; } = 0;
    public string? SroScheduleNo { get; set; }
    public decimal FedPayable { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public string SaleType { get; set; } = string.Empty;
    public string? SroItemSerialNo { get; set; }
    public string? FbrItemInvoiceNo { get; set; }
    public string? ItemStatus { get; set; }
    public string? ItemErrorCode { get; set; }
    public string? ItemErrorMessage { get; set; }

    public Invoice Invoice { get; set; } = null!;
}
