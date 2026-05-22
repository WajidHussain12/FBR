using FBR_DI.Application.DTOs.Invoice;
using FBR_DI.Application.Features.Admin.Queries.Invoice;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Features.Admin.Handlers.QueryHandlers;

public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, Result<PagedResult<InvoiceListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllInvoicesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<InvoiceListDto>>> Handle(
        GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Invoices.AsNoTracking();

        if (request.CompanyId.HasValue)
            query = query.Where(i => i.CompanyId == request.CompanyId.Value);

        if (request.Status.HasValue)
            query = query.Where(i => i.Status == request.Status.Value);

        if (request.FromDate.HasValue)
            query = query.Where(i => i.InvoiceDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(i => i.InvoiceDate <= request.ToDate.Value);

        if (!string.IsNullOrWhiteSpace(request.Params.SearchTerm))
        {
            var term = request.Params.SearchTerm.ToLower();
            query = query.Where(i =>
                i.BuyerBusinessName.ToLower().Contains(term) ||
                i.SellerBusinessName.ToLower().Contains(term) ||
                (i.FbrInvoiceNumber != null && i.FbrInvoiceNumber.Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = query.OrderByDescending(i => i.CreatedAt);

        var items = await query
            .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
            .Take(request.Params.PageSize)
            .Select(i => new InvoiceListDto
            {
                Id = i.Id,
                InvoiceType = i.InvoiceType,
                InvoiceDate = i.InvoiceDate,
                SellerBusinessName = i.SellerBusinessName,
                BuyerBusinessName = i.BuyerBusinessName,
                GrandTotal = i.GrandTotal,
                Status = i.Status,
                FbrInvoiceNumber = i.FbrInvoiceNumber,
                FbrSubmissionDate = i.FbrSubmissionDate,
                ItemCount = i.Items.Count
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<InvoiceListDto>>.Success(
            PagedResult<InvoiceListDto>.Create(items, totalCount,
                request.Params.PageNumber, request.Params.PageSize));
    }
}
