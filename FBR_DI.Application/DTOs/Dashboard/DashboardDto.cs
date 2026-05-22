namespace FBR_DI.Application.DTOs.Dashboard;

public class DashboardDto
{
    public int TotalCompanies { get; set; }
    public int ActiveCompanies { get; set; }
    public int TotalInvoicesThisMonth { get; set; }
    public int ValidInvoicesThisMonth { get; set; }
    public int InvalidInvoicesThisMonth { get; set; }
    public int PendingInvoices { get; set; }
    public decimal TotalTaxCollectedThisMonth { get; set; }
    public List<RecentInvoiceDto> RecentInvoices { get; set; } = new();
    public List<CompanyStatDto> CompanyStats { get; set; } = new();
}

public class RecentInvoiceDto
{
    public int Id { get; set; }
    public string? FbrInvoiceNumber { get; set; }
    public string BuyerBusinessName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
}

public class CompanyStatDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int TotalInvoices { get; set; }
    public int ValidInvoices { get; set; }
    public decimal TotalTax { get; set; }
}
