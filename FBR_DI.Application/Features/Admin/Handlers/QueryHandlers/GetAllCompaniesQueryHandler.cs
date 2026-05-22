using FBR_DI.Application.DTOs.Company;
using FBR_DI.Application.Features.Admin.Queries.Company;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Features.Admin.Handlers.QueryHandlers;

public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, Result<PagedResult<CompanyListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllCompaniesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<CompanyListDto>>> Handle(
        GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Companies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Params.SearchTerm))
        {
            var term = request.Params.SearchTerm.ToLower();
            query = query.Where(c =>
                c.CompanyName.ToLower().Contains(term) ||
                c.NTN.Contains(term) ||
                c.Province.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var sortColumn = request.Params.SortColumn?.ToLower() ?? "createdat";
        var sortDir = request.Params.SortDirection?.ToLower() ?? "desc";

        query = (sortColumn, sortDir) switch
        {
            ("companyname", "asc") => query.OrderBy(c => c.CompanyName),
            ("companyname", _) => query.OrderByDescending(c => c.CompanyName),
            ("ntn", "asc") => query.OrderBy(c => c.NTN),
            ("ntn", _) => query.OrderByDescending(c => c.NTN),
            (_, "asc") => query.OrderBy(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        var items = await query
            .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
            .Take(request.Params.PageSize)
            .Select(c => new CompanyListDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                NTN = c.NTN,
                Province = c.Province,
                IsIntegrated = c.IsIntegrated,
                Environment = c.SubmissionEnvironment,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var paged = PagedResult<CompanyListDto>.Create(items, totalCount,
            request.Params.PageNumber, request.Params.PageSize);

        return Result<PagedResult<CompanyListDto>>.Success(paged);
    }
}
