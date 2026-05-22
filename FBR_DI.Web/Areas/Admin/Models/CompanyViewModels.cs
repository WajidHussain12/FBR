using System.ComponentModel.DataAnnotations;
using FBR_DI.Domain.Enums;

namespace FBR_DI.Web.Areas.Admin.Models;

public class CreateCompanyViewModel
{
    [Required(ErrorMessage = "Company Name is required.")]
    [StringLength(200)]
    [Display(Name = "Company Name")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "NTN is required.")]
    [StringLength(13)]
    [RegularExpression(@"^\d{7}$|^\d{13}$", ErrorMessage = "NTN must be 7 or 13 digits.")]
    public string NTN { get; set; } = string.Empty;

    [StringLength(13)]
    [RegularExpression(@"^\d{13}$", ErrorMessage = "CNIC must be 13 digits.")]
    public string? CNIC { get; set; }

    [Required(ErrorMessage = "Province is required.")]
    [StringLength(100)]
    public string Province { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(100)]
    [Display(Name = "Business Activity")]
    public string? BusinessActivity { get; set; }

    [StringLength(100)]
    public string? Sector { get; set; }

    [Display(Name = "FBR Bearer Token")]
    public string? FbrBearerToken { get; set; }

    [Display(Name = "Submission Environment")]
    public SubmissionEnvironment SubmissionEnvironment { get; set; } = SubmissionEnvironment.Sandbox;
}

public class UpdateCompanyViewModel : CreateCompanyViewModel
{
    public int Id { get; set; }
}
