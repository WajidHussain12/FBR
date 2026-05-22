using FBR_DI.Domain.Enums;

namespace FBR_DI.Application.DTOs.Company;

public class CompanyListDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string NTN { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public bool IsIntegrated { get; set; }
    public SubmissionEnvironment Environment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CompanyDetailDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string NTN { get; set; } = string.Empty;
    public string? CNIC { get; set; }
    public string Province { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? BusinessActivity { get; set; }
    public string? Sector { get; set; }
    public SubmissionEnvironment SubmissionEnvironment { get; set; }
    public bool IsIntegrated { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalInvoices { get; set; }
    public int SubmittedInvoices { get; set; }
}

public class CreateCompanyDto
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
}

public class UpdateCompanyDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string NTN { get; set; } = string.Empty;
    public string? CNIC { get; set; }
    public string Province { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? BusinessActivity { get; set; }
    public string? Sector { get; set; }
    public SubmissionEnvironment SubmissionEnvironment { get; set; }
}
