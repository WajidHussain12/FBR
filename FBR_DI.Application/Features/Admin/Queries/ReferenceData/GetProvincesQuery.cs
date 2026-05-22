using FBR_DI.Application.DTOs.FbrApi;
using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Queries.ReferenceData;

public class GetProvincesQuery : IRequest<Result<List<ProvinceDto>>>
{
    public int CompanyId { get; set; }
}

public class GetUomListQuery : IRequest<Result<List<UomDto>>>
{
    public int CompanyId { get; set; }
}

public class GetTaxRatesQuery : IRequest<Result<List<TaxRateDto>>>
{
    public int CompanyId { get; set; }
    public string Date { get; set; } = string.Empty;
    public int TransTypeId { get; set; }
    public int OriginationSupplier { get; set; }
}

public class GetHsCodesQuery : IRequest<Result<List<HsCodeDto>>>
{
    public int CompanyId { get; set; }
}
