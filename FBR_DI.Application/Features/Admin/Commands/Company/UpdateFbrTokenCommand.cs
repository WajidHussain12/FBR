using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Enums;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Commands.Company;

public class UpdateFbrTokenCommand : IRequest<Result<bool>>
{
    public int CompanyId { get; set; }
    public string BearerToken { get; set; } = string.Empty;
    public SubmissionEnvironment Environment { get; set; }
}
