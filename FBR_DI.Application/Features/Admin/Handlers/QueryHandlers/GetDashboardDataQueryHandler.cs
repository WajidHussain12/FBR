using FBR_DI.Application.DTOs.Dashboard;
using FBR_DI.Application.Features.Admin.Queries.Dashboard;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Features.Admin.Handlers.QueryHandlers;

public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, Result<DashboardDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardDataQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DashboardDto>> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalCompanies = await _context.Companies.CountAsync(cancellationToken);
        var activeCompanies = await _context.Companies.CountAsync(c => c.IsActive, cancellationToken);

        var thisMonthInvoices = await _context.Invoices
            .Where(i => i.CreatedAt >= startOfMonth)
            .ToListAsync(cancellationToken);

        var totalThisMonth = thisMonthInvoices.Count;
        var validThisMonth = thisMonthInvoices.Count(i => i.Status == InvoiceStatus.Valid);
        var invalidThisMonth = thisMonthInvoices.Count(i => i.Status == InvoiceStatus.Invalid);
        var pending = await _context.Invoices
            .CountAsync(i => i.Status == InvoiceStatus.Draft || i.Status == InvoiceStatus.PendingSubmission,
                cancellationToken);

        var totalTaxThisMonth = thisMonthInvoices
            .Where(i => i.Status == InvoiceStatus.Valid)
            .Sum(i => i.TotalSalesTax);

        var recentInvoices = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.Company)
            .OrderByDescending(i => i.CreatedAt)
            .Take(10)
            .Select(i => new RecentInvoiceDto
            {
                Id = i.Id,
                FbrInvoiceNumber = i.FbrInvoiceNumber,
                BuyerBusinessName = i.BuyerBusinessName,
                CompanyName = i.Company.CompanyName,
                GrandTotal = i.GrandTotal,
                Status = i.Status.ToString(),
                InvoiceDate = i.InvoiceDate
            })
            .ToListAsync(cancellationToken);

        var companyStats = await _context.Companies
            .AsNoTracking()
            .Select(c => new CompanyStatDto
            {
                CompanyId = c.Id,
                CompanyName = c.CompanyName,
                TotalInvoices = c.Invoices.Count,
                ValidInvoices = c.Invoices.Count(i => i.Status == InvoiceStatus.Valid),
                TotalTax = c.Invoices
                    .Where(i => i.Status == InvoiceStatus.Valid)
                    .Sum(i => i.TotalSalesTax)
            })
            .ToListAsync(cancellationToken);

        var dashboard = new DashboardDto
        {
            TotalCompanies = totalCompanies,
            ActiveCompanies = activeCompanies,
            TotalInvoicesThisMonth = totalThisMonth,
            ValidInvoicesThisMonth = validThisMonth,
            InvalidInvoicesThisMonth = invalidThisMonth,
            PendingInvoices = pending,
            TotalTaxCollectedThisMonth = totalTaxThisMonth,
            RecentInvoices = recentInvoices,
            CompanyStats = companyStats
        };

        return Result<DashboardDto>.Success(dashboard);
    }
}
