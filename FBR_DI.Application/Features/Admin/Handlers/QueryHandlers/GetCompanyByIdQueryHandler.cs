using FBR_DI.Application.DTOs.Company;
using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Queries.Company;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using FBR_DI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Features.Admin.Handlers.QueryHandlers;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCompanyByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CompanyDetailDto>> Handle(
        GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (company is null)
            throw new NotFoundException(nameof(Company), request.Id);

        var totalInvoices = await _context.Invoices
            .CountAsync(i => i.CompanyId == request.Id, cancellationToken);

        var submittedInvoices = await _context.Invoices
            .CountAsync(i => i.CompanyId == request.Id && i.Status == InvoiceStatus.Valid, cancellationToken);

        var dto = new CompanyDetailDto
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            NTN = company.NTN,
            CNIC = company.CNIC,
            Province = company.Province,
            Address = company.Address,
            PhoneNumber = company.PhoneNumber,
            Email = company.Email,
            BusinessActivity = company.BusinessActivity,
            Sector = company.Sector,
            SubmissionEnvironment = company.SubmissionEnvironment,
            IsIntegrated = company.IsIntegrated,
            IsActive = company.IsActive,
            CreatedAt = company.CreatedAt,
            TotalInvoices = totalInvoices,
            SubmittedInvoices = submittedInvoices
        };

        return Result<CompanyDetailDto>.Success(dto);
    }
}
