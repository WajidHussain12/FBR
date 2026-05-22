using FBR_DI.Domain.Common;
using FBR_DI.Domain.Enums;

namespace FBR_DI.Domain.Entities;

public class Invoice : AuditableEntity
{
    public int CompanyId { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public string SellerNTNCNIC { get; set; } = string.Empty;
    public string SellerBusinessName { get; set; } = string.Empty;
    public string SellerProvince { get; set; } = string.Empty;
    public string SellerAddress { get; set; } = string.Empty;
    public string? BuyerNTNCNIC { get; set; }
    public string BuyerBusinessName { get; set; } = string.Empty;
    public string BuyerProvince { get; set; } = string.Empty;
    public string BuyerAddress { get; set; } = string.Empty;
    public BuyerRegistrationType BuyerRegistrationType { get; set; }
    public string? InvoiceRefNo { get; set; }
    public string? ScenarioId { get; set; }
    public string? FbrInvoiceNumber { get; set; }
    public DateTime? FbrSubmissionDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string? FbrRawResponse { get; set; }
    public string? FbrErrorCode { get; set; }
    public string? FbrErrorMessage { get; set; }
    public decimal TotalValueExclST { get; set; }
    public decimal TotalSalesTax { get; set; }
    public decimal TotalFurtherTax { get; set; }
    public decimal TotalExtraTax { get; set; }
    public decimal TotalFedPayable { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal GrandTotal { get; set; }
    public string? QrCodeBase64 { get; set; }

    public Company Company { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
