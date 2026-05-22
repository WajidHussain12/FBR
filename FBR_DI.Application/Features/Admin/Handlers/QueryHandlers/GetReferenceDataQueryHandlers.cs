using FBR_DI.Application.DTOs.FbrApi;
using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Queries.ReferenceData;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FBR_DI.Application.Features.Admin.Handlers.QueryHandlers;

public class GetProvincesQueryHandler : IRequestHandler<GetProvincesQuery, Result<List<ProvinceDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFbrApiService _fbrApiService;
    private readonly IMemoryCache _cache;

    public GetProvincesQueryHandler(IApplicationDbContext context, IFbrApiService fbrApiService, IMemoryCache cache)
    {
        _context = context;
        _fbrApiService = fbrApiService;
        _cache = cache;
    }

    public async Task<Result<List<ProvinceDto>>> Handle(GetProvincesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"fbr_Province_{request.CompanyId}";
        if (_cache.TryGetValue(cacheKey, out List<ProvinceDto>? cached) && cached != null)
            return Result<List<ProvinceDto>>.Success(cached);

        var dbRecords = await _context.FbrReferenceDatas
            .AsNoTracking()
            .Where(r => r.ReferenceType == "Province")
            .ToListAsync(cancellationToken);

        if (dbRecords.Any())
        {
            var dtos = dbRecords.Select(r => new ProvinceDto
            {
                StateProvinceCode = int.TryParse(r.Code, out var c) ? c : 0,
                StateProvinceDesc = r.Description
            }).ToList();
            _cache.Set(cacheKey, dtos, TimeSpan.FromHours(1));
            return Result<List<ProvinceDto>>.Success(dtos);
        }

        var company = await _context.Companies.FindAsync(new object[] { request.CompanyId }, cancellationToken);
        if (company is null) throw new NotFoundException(nameof(Company), request.CompanyId);
        if (string.IsNullOrWhiteSpace(company.FbrBearerToken))
            return Result<List<ProvinceDto>>.Failure("No FBR token configured.");

        var result = await _fbrApiService.GetProvincesAsync(company.FbrBearerToken, cancellationToken);
        if (result.IsSuccess && result.Data != null)
            _cache.Set(cacheKey, result.Data, TimeSpan.FromHours(1));

        return result;
    }
}

public class GetUomListQueryHandler : IRequestHandler<GetUomListQuery, Result<List<UomDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFbrApiService _fbrApiService;
    private readonly IMemoryCache _cache;

    public GetUomListQueryHandler(IApplicationDbContext context, IFbrApiService fbrApiService, IMemoryCache cache)
    {
        _context = context;
        _fbrApiService = fbrApiService;
        _cache = cache;
    }

    public async Task<Result<List<UomDto>>> Handle(GetUomListQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"fbr_UOM_{request.CompanyId}";
        if (_cache.TryGetValue(cacheKey, out List<UomDto>? cached) && cached != null)
            return Result<List<UomDto>>.Success(cached);

        var dbRecords = await _context.FbrReferenceDatas
            .AsNoTracking()
            .Where(r => r.ReferenceType == "UOM")
            .ToListAsync(cancellationToken);

        if (dbRecords.Any())
        {
            var dtos = dbRecords.Select(r => new UomDto
            {
                UoMId = int.TryParse(r.Code, out var c) ? c : 0,
                Description = r.Description
            }).ToList();
            _cache.Set(cacheKey, dtos, TimeSpan.FromHours(1));
            return Result<List<UomDto>>.Success(dtos);
        }

        var company = await _context.Companies.FindAsync(new object[] { request.CompanyId }, cancellationToken);
        if (company is null) throw new NotFoundException(nameof(Company), request.CompanyId);
        if (string.IsNullOrWhiteSpace(company.FbrBearerToken))
            return Result<List<UomDto>>.Failure("No FBR token configured.");

        var result = await _fbrApiService.GetUomListAsync(company.FbrBearerToken, cancellationToken);
        if (result.IsSuccess && result.Data != null)
            _cache.Set(cacheKey, result.Data, TimeSpan.FromHours(1));

        return result;
    }
}

public class GetTaxRatesQueryHandler : IRequestHandler<GetTaxRatesQuery, Result<List<TaxRateDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFbrApiService _fbrApiService;
    private readonly IMemoryCache _cache;

    public GetTaxRatesQueryHandler(IApplicationDbContext context, IFbrApiService fbrApiService, IMemoryCache cache)
    {
        _context = context;
        _fbrApiService = fbrApiService;
        _cache = cache;
    }

    public async Task<Result<List<TaxRateDto>>> Handle(GetTaxRatesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"fbr_TaxRate_{request.CompanyId}_{request.Date}_{request.TransTypeId}_{request.OriginationSupplier}";
        if (_cache.TryGetValue(cacheKey, out List<TaxRateDto>? cached) && cached != null)
            return Result<List<TaxRateDto>>.Success(cached);

        var company = await _context.Companies.FindAsync(new object[] { request.CompanyId }, cancellationToken);
        if (company is null) throw new NotFoundException(nameof(Company), request.CompanyId);
        if (string.IsNullOrWhiteSpace(company.FbrBearerToken))
            return Result<List<TaxRateDto>>.Failure("No FBR token configured.");

        var result = await _fbrApiService.GetTaxRatesAsync(company.FbrBearerToken,
            request.Date, request.TransTypeId, request.OriginationSupplier, cancellationToken);

        if (result.IsSuccess && result.Data != null)
            _cache.Set(cacheKey, result.Data, TimeSpan.FromHours(1));

        return result;
    }
}

public class GetHsCodesQueryHandler : IRequestHandler<GetHsCodesQuery, Result<List<HsCodeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFbrApiService _fbrApiService;
    private readonly IMemoryCache _cache;

    public GetHsCodesQueryHandler(IApplicationDbContext context, IFbrApiService fbrApiService, IMemoryCache cache)
    {
        _context = context;
        _fbrApiService = fbrApiService;
        _cache = cache;
    }

    public async Task<Result<List<HsCodeDto>>> Handle(GetHsCodesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"fbr_HSCode_{request.CompanyId}";
        if (_cache.TryGetValue(cacheKey, out List<HsCodeDto>? cached) && cached != null)
            return Result<List<HsCodeDto>>.Success(cached);

        var dbRecords = await _context.FbrReferenceDatas
            .AsNoTracking()
            .Where(r => r.ReferenceType == "HSCode")
            .ToListAsync(cancellationToken);

        if (dbRecords.Any())
        {
            var dtos = dbRecords.Select(r => new HsCodeDto
            {
                HsCode = r.Code,
                Description = r.Description
            }).ToList();
            _cache.Set(cacheKey, dtos, TimeSpan.FromHours(1));
            return Result<List<HsCodeDto>>.Success(dtos);
        }

        var company = await _context.Companies.FindAsync(new object[] { request.CompanyId }, cancellationToken);
        if (company is null) throw new NotFoundException(nameof(Company), request.CompanyId);
        if (string.IsNullOrWhiteSpace(company.FbrBearerToken))
            return Result<List<HsCodeDto>>.Failure("No FBR token configured.");

        var result = await _fbrApiService.GetHsCodesAsync(company.FbrBearerToken, cancellationToken);
        if (result.IsSuccess && result.Data != null)
            _cache.Set(cacheKey, result.Data, TimeSpan.FromHours(1));

        return result;
    }
}
