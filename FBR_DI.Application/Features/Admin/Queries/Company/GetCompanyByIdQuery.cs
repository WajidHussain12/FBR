using FBR_DI.Application.DTOs.Company;
using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Queries.Company;

public class GetCompanyByIdQuery : IRequest<Result<CompanyDetailDto>>
{
    public int Id { get; set; }
}
