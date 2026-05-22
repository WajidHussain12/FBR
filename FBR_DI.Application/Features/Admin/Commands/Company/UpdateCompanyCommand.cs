using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Enums;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Commands.Company;

public class UpdateCompanyCommand : IRequest<Result<bool>>
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
