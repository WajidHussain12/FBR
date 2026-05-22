using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Commands.ReferenceData;

public class SyncFbrReferenceDataCommand : IRequest<Result<bool>>
{
    public int CompanyId { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
}
