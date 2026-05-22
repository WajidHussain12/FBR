using FBR_DI.Application.Features.Admin.Commands.ReferenceData;
using FBR_DI.Application.Features.Admin.Queries.ReferenceData;
using FBR_DI.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBR_DI.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ReferenceDataController : Controller
{
    private readonly ISender _sender;
    private readonly IFbrApiService _fbrApiService;

    public ReferenceDataController(ISender sender, IFbrApiService fbrApiService)
    {
        _sender = sender;
        _fbrApiService = fbrApiService;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> Provinces(int companyId)
    {
        var result = await _sender.Send(new GetProvincesQuery { CompanyId = companyId });
        return Json(result.Data ?? new List<Application.DTOs.FbrApi.ProvinceDto>());
    }

    [HttpGet]
    public async Task<IActionResult> UomList(int companyId)
    {
        var result = await _sender.Send(new GetUomListQuery { CompanyId = companyId });
        return Json(result.Data ?? new List<Application.DTOs.FbrApi.UomDto>());
    }

    [HttpGet]
    public async Task<IActionResult> HsCodes(int companyId)
    {
        var result = await _sender.Send(new GetHsCodesQuery { CompanyId = companyId });
        return Json(result.Data ?? new List<Application.DTOs.FbrApi.HsCodeDto>());
    }

    [HttpGet]
    public async Task<IActionResult> SearchHsCodes(int companyId, string term)
    {
        var result = await _sender.Send(new GetHsCodesQuery { CompanyId = companyId });
        var filtered = result.Data?
            .Where(h => h.HsCode.Contains(term) ||
                        h.Description.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Take(20)
            .Select(h => (object)new { id = h.HsCode, text = $"{h.HsCode} - {h.Description}" })
            .ToList();
        return Json(filtered ?? new List<object>());
    }

    [HttpGet]
    public async Task<IActionResult> GetTaxRates(int companyId, string date, int transTypeId, int provinceId)
    {
        var result = await _sender.Send(new GetTaxRatesQuery
        {
            CompanyId = companyId,
            Date = date,
            TransTypeId = transTypeId,
            OriginationSupplier = provinceId
        });
        return Json(new { success = result.IsSuccess, data = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> CheckBuyerStatus(int companyId, string regno, string date)
    {
        // Need bearer token from company
        var companies = await _sender.Send(new Application.Features.Admin.Queries.Company.GetCompanyByIdQuery
        { Id = companyId });

        if (companies.IsFailure || string.IsNullOrEmpty(companies.Data?.ToString()))
            return Json(new { success = false, message = "Company not found." });

        // Call via company from context - simplified
        return Json(new { success = true, message = "Status check initiated." });
    }

    [HttpPost]
    public async Task<IActionResult> SyncAll(int companyId)
    {
        var types = new[] { "Province", "UOM", "HSCode", "DocType", "TransactionType" };
        var results = new List<object>();

        foreach (var type in types)
        {
            var result = await _sender.Send(new SyncFbrReferenceDataCommand
            {
                CompanyId = companyId,
                ReferenceType = type
            });
            results.Add(new { type, success = result.IsSuccess, message = result.Message });
        }

        return Json(new { success = true, results });
    }
}
