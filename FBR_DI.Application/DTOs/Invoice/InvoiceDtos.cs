using FBR_DI.Domain.Enums;

namespace FBR_DI.Application.DTOs.Invoice;

public class InvoiceListDto
{
    public int Id { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public string SellerBusinessName { get; set; } = string.Empty;
    public string BuyerBusinessName { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? FbrInvoiceNumber { get; set; }
    public DateTime? FbrSubmissionDate { get; set; }
    public int ItemCount { get; set; }
}

public class InvoiceDetailDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
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
    public InvoiceStatus Status { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public List<InvoiceItemDetailDto> Items { get; set; } = new();
}

public class InvoiceItemDetailDto
{
    public int Id { get; set; }
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
    public decimal ExtraTax { get; set; }
    public decimal FurtherTax { get; set; }
    public string? SroScheduleNo { get; set; }
    public decimal FedPayable { get; set; }
    public decimal Discount { get; set; }
    public string SaleType { get; set; } = string.Empty;
    public string? SroItemSerialNo { get; set; }
    public string? FbrItemInvoiceNo { get; set; }
    public string? ItemStatus { get; set; }
    public string? ItemErrorCode { get; set; }
    public string? ItemErrorMessage { get; set; }
}

public class CreateInvoiceDto
{
    public int CompanyId { get; set; }
    public string InvoiceType { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public string SellerNTNCNIC { get; set; } = string.Empty;
    public string SellerBusinessName { get; set; } = string.Empty;
    public string SellerProvince { get; set; } = string.Empty;
    public string SellerAddress { get; set; } = string.Empty;
    public string? BuyerNTNCNIC { get; set; }
    public string BuyerBusinessName { get; set; } = string.Empty;
    public string BuyerProvince { get; set; } = string.Empty;
    public string BuyerAddress { get; set; } = string.Empty;
    public string BuyerRegistrationType { get; set; } = string.Empty;
    public string? InvoiceRefNo { get; set; }
    public string? ScenarioId { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}

public class CreateInvoiceItemDto
{
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
    public decimal ExtraTax { get; set; }
    public decimal FurtherTax { get; set; }
    public string? SroScheduleNo { get; set; }
    public decimal FedPayable { get; set; }
    public decimal Discount { get; set; }
    public string SaleType { get; set; } = string.Empty;
    public string? SroItemSerialNo { get; set; }
}

public class UpdateInvoiceDto
{
    public int Id { get; set; }
    public string InvoiceType { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public string SellerNTNCNIC { get; set; } = string.Empty;
    public string SellerBusinessName { get; set; } = string.Empty;
    public string SellerProvince { get; set; } = string.Empty;
    public string SellerAddress { get; set; } = string.Empty;
    public string? BuyerNTNCNIC { get; set; }
    public string BuyerBusinessName { get; set; } = string.Empty;
    public string BuyerProvince { get; set; } = string.Empty;
    public string BuyerAddress { get; set; } = string.Empty;
    public string BuyerRegistrationType { get; set; } = string.Empty;
    public string? InvoiceRefNo { get; set; }
    public string? ScenarioId { get; set; }
    public List<UpdateInvoiceItemDto> Items { get; set; } = new();
}

public class UpdateInvoiceItemDto : CreateInvoiceItemDto
{
    public int Id { get; set; }
}

public class InvoiceSummaryDto
{
    public int TotalInvoices { get; set; }
    public int ValidInvoices { get; set; }
    public int InvalidInvoices { get; set; }
    public int PendingInvoices { get; set; }
    public decimal TotalTaxAmount { get; set; }
    public List<MonthlyCountDto> MonthlyTrend { get; set; } = new();
}

public class MonthlyCountDto
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TaxAmount { get; set; }
}
