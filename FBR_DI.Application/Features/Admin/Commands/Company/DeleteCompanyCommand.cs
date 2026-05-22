using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Commands.Company;

public class DeleteCompanyCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
}
