using System.ComponentModel.DataAnnotations;

namespace FBR_DI.Web.Areas.Admin.Models;

public class CreateInvoiceViewModel
{
    [Required]
    [Display(Name = "Company")]
    public int CompanyId { get; set; }

    [Required]
    [Display(Name = "Invoice Type")]
    public string InvoiceType { get; set; } = "SaleInvoice";

    [Required]
    [Display(Name = "Invoice Date")]
    public DateOnly InvoiceDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Display(Name = "Invoice Reference No")]
    public string? InvoiceRefNo { get; set; }

    [Display(Name = "Scenario ID")]
    public string? ScenarioId { get; set; }

    // Seller
    [Required]
    [Display(Name = "Seller NTN/CNIC")]
    public string SellerNTNCNIC { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Seller Business Name")]
    public string SellerBusinessName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Seller Province")]
    public string SellerProvince { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Seller Address")]
    public string SellerAddress { get; set; } = string.Empty;

    // Buyer
    [Display(Name = "Buyer NTN/CNIC")]
    public string? BuyerNTNCNIC { get; set; }

    [Required]
    [Display(Name = "Buyer Business Name")]
    public string BuyerBusinessName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Buyer Province")]
    public string BuyerProvince { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Buyer Address")]
    public string BuyerAddress { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Buyer Registration Type")]
    public string BuyerRegistrationType { get; set; } = "Unregistered";

    public List<CreateInvoiceItemViewModel> Items { get; set; } = new();
}

public class CreateInvoiceItemViewModel
{
    [Required]
    [Display(Name = "HS Code")]
    public string HsCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Product Description")]
    public string ProductDescription { get; set; } = string.Empty;

    [Required]
    public string Rate { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Unit of Measure")]
    public string UoM { get; set; } = string.Empty;

    [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public decimal Quantity { get; set; }

    [Display(Name = "Total Values")]
    public decimal TotalValues { get; set; }

    [Display(Name = "Value (Excl. ST)")]
    public decimal ValueSalesExcludingST { get; set; }

    [Display(Name = "Fixed/Notified Price")]
    public decimal FixedNotifiedValueOrRetailPrice { get; set; }

    [Display(Name = "Sales Tax Applicable")]
    public decimal SalesTaxApplicable { get; set; }

    [Display(Name = "ST Withheld at Source")]
    public decimal SalesTaxWithheldAtSource { get; set; }

    [Display(Name = "Extra Tax")]
    public decimal ExtraTax { get; set; }

    [Display(Name = "Further Tax")]
    public decimal FurtherTax { get; set; }

    [Display(Name = "SRO Schedule No")]
    public string? SroScheduleNo { get; set; }

    [Display(Name = "FED Payable")]
    public decimal FedPayable { get; set; }

    public decimal Discount { get; set; }

    [Required]
    [Display(Name = "Sale Type")]
    public string SaleType { get; set; } = string.Empty;

    [Display(Name = "SRO Item Serial No")]
    public string? SroItemSerialNo { get; set; }
}

public class UserListViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}
