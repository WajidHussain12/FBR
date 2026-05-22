using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Commands.ReferenceData;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Features.Admin.Handlers.CommandHandlers;

public class SyncFbrReferenceDataCommandHandler : IRequestHandler<SyncFbrReferenceDataCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFbrApiService _fbrApiService;

    public SyncFbrReferenceDataCommandHandler(IApplicationDbContext context, IFbrApiService fbrApiService)
    {
        _context = context;
        _fbrApiService = fbrApiService;
    }

    public async Task<Result<bool>> Handle(SyncFbrReferenceDataCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FindAsync(new object[] { request.CompanyId }, cancellationToken);
        if (company is null)
            throw new NotFoundException(nameof(Company), request.CompanyId);

        if (string.IsNullOrWhiteSpace(company.FbrBearerToken))
            return Result<bool>.Failure("Company does not have an FBR Bearer Token configured.");

        var token = company.FbrBearerToken;
        var now = DateTime.UtcNow;
        var records = new List<FbrReferenceData>();

        switch (request.ReferenceType)
        {
            case "Province":
            {
                var result = await _fbrApiService.GetProvincesAsync(token, cancellationToken);
                if (result.IsFailure) return Result<bool>.Failure(result.Message ?? "Failed to fetch provinces.");
                records = result.Data!.Select(p => new FbrReferenceData
                {
                    ReferenceType = "Province",
                    Code = p.StateProvinceCode.ToString(),
                    Description = p.StateProvinceDesc,
                    LastSyncedAt = now
                }).ToList();
                break;
            }
            case "UOM":
            {
                var result = await _fbrApiService.GetUomListAsync(token, cancellationToken);
                if (result.IsFailure) return Result<bool>.Failure(result.Message ?? "Failed to fetch UOM list.");
                records = result.Data!.Select(u => new FbrReferenceData
                {
                    ReferenceType = "UOM",
                    Code = u.UoMId.ToString(),
                    Description = u.Description,
                    LastSyncedAt = now
                }).ToList();
                break;
            }
            case "HSCode":
            {
                var result = await _fbrApiService.GetHsCodesAsync(token, cancellationToken);
                if (result.IsFailure) return Result<bool>.Failure(result.Message ?? "Failed to fetch HS Codes.");
                records = result.Data!.Select(h => new FbrReferenceData
                {
                    ReferenceType = "HSCode",
                    Code = h.HsCode,
                    Description = h.Description,
                    LastSyncedAt = now
                }).ToList();
                break;
            }
            case "DocType":
            {
                var result = await _fbrApiService.GetDocTypesAsync(token, cancellationToken);
                if (result.IsFailure) return Result<bool>.Failure(result.Message ?? "Failed to fetch doc types.");
                records = result.Data!.Select(d => new FbrReferenceData
                {
                    ReferenceType = "DocType",
                    Code = d.DocTypeId.ToString(),
                    Description = d.DocDescription,
                    LastSyncedAt = now
                }).ToList();
                break;
            }
            case "TransactionType":
            {
                var result = await _fbrApiService.GetTransactionTypesAsync(token, cancellationToken);
                if (result.IsFailure) return Result<bool>.Failure(result.Message ?? "Failed to fetch transaction types.");
                records = result.Data!.Select(t => new FbrReferenceData
                {
                    ReferenceType = "TransactionType",
                    Code = t.TransactionTypeId.ToString(),
                    Description = t.TransactionDesc,
                    LastSyncedAt = now
                }).ToList();
                break;
            }
            default:
                return Result<bool>.Failure($"Unknown reference type: {request.ReferenceType}");
        }

        // Delete existing records of this type
        var existing = await _context.FbrReferenceDatas
            .Where(r => r.ReferenceType == request.ReferenceType)
            .ToListAsync(cancellationToken);

        foreach (var e in existing)
            _context.FbrReferenceDatas.Remove(e);

        foreach (var r in records)
            _context.FbrReferenceDatas.Add(r);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, $"Synced {records.Count} {request.ReferenceType} records.");
    }
}
