using FBR_DI.Domain.Common;
using FBR_DI.Domain.Enums;

namespace FBR_DI.Domain.Entities;

public class Company : AuditableEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string NTN { get; set; } = string.Empty;
    public string? CNIC { get; set; }
    public string Province { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? BusinessActivity { get; set; }
    public string? Sector { get; set; }
    public string? FbrBearerToken { get; set; }
    public SubmissionEnvironment SubmissionEnvironment { get; set; } = SubmissionEnvironment.Sandbox;
    public bool IsIntegrated { get; set; } = false;

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
