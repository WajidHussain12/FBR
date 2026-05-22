using FBR_DI.Application.DTOs.Company;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Application.Wrappers;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Queries.Company;

public class GetAllCompaniesQuery : IRequest<Result<PagedResult<CompanyListDto>>>
{
    public PaginationParams Params { get; set; } = new();
}
