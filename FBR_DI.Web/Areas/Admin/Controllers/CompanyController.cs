using FBR_DI.Application.Features.Admin.Commands.Company;
using FBR_DI.Application.Features.Admin.Commands.ReferenceData;
using FBR_DI.Application.Features.Admin.Queries.Company;
using FBR_DI.Application.Wrappers;
using FBR_DI.Domain.Enums;
using FBR_DI.Web.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBR_DI.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class CompanyController : Controller
{
    private readonly ISender _sender;

    public CompanyController(ISender sender)
    {
        _sender = sender;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int draw, int start, int length, string? searchValue,
        string? sortColumn, string? sortDir)
    {
        var paginationParams = new PaginationParams
        {
            PageNumber = (start / length) + 1,
            PageSize = length > 0 ? length : 10,
            SearchTerm = searchValue,
            SortColumn = sortColumn,
            SortDirection = sortDir
        };

        var result = await _sender.Send(new GetAllCompaniesQuery { Params = paginationParams });
        if (result.IsFailure)
            return Json(new { draw, recordsTotal = 0, recordsFiltered = 0, data = Array.Empty<object>() });

        var paged = result.Data!;
        return Json(new
        {
            draw,
            recordsTotal = paged.TotalCount,
            recordsFiltered = paged.TotalCount,
            data = paged.Items
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllForDropdown()
    {
        var result = await _sender.Send(new GetAllCompaniesQuery
        {
            Params = new PaginationParams { PageNumber = 1, PageSize = 100 }
        });

        if (result.IsFailure)
            return Json(new List<object>());

        return Json(result.Data!.Items.Select(c => new
        {
            value        = c.Id,
            text         = $"{c.CompanyName} (NTN: {c.NTN})",
            isIntegrated = c.IsIntegrated,
            environment  = c.Environment.ToString()
        }));
    }

    [HttpGet]
    public IActionResult Create()
    {
        PopulateViewBag();
        return View(new CreateCompanyViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCompanyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulateViewBag();
            return View(model);
        }

        var command = new CreateCompanyCommand
        {
            CompanyName = model.CompanyName,
            NTN = model.NTN,
            CNIC = model.CNIC,
            Province = model.Province,
            Address = model.Address,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            BusinessActivity = model.BusinessActivity,
            Sector = model.Sector,
            FbrBearerToken = model.FbrBearerToken,
            SubmissionEnvironment = model.SubmissionEnvironment
        };

        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            TempData["Success"] = "Company created successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Message ?? "Failed to create company.");
        PopulateViewBag();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _sender.Send(new GetCompanyByIdQuery { Id = id });
        if (result.IsFailure)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        var dto = result.Data!;
        var model = new UpdateCompanyViewModel
        {
            Id = dto.Id,
            CompanyName = dto.CompanyName,
            NTN = dto.NTN,
            CNIC = dto.CNIC,
            Province = dto.Province,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            BusinessActivity = dto.BusinessActivity,
            Sector = dto.Sector,
            SubmissionEnvironment = dto.SubmissionEnvironment
        };

        PopulateViewBag();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateCompanyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulateViewBag();
            return View(model);
        }

        var command = new UpdateCompanyCommand
        {
            Id = id,
            CompanyName = model.CompanyName,
            NTN = model.NTN,
            CNIC = model.CNIC,
            Province = model.Province,
            Address = model.Address,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            BusinessActivity = model.BusinessActivity,
            Sector = model.Sector,
            SubmissionEnvironment = model.SubmissionEnvironment
        };

        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            TempData["Success"] = "Company updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Message ?? "Failed to update company.");
        PopulateViewBag();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _sender.Send(new DeleteCompanyCommand { Id = id });
        return Json(new { success = result.IsSuccess, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> FbrSettings(int id)
    {
        var result = await _sender.Send(new GetCompanyByIdQuery { Id = id });
        if (result.IsFailure)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateFbrToken(int id, string token, string environment)
    {
        var env = Enum.Parse<SubmissionEnvironment>(environment);
        var result = await _sender.Send(new UpdateFbrTokenCommand
        {
            CompanyId = id,
            BearerToken = token,
            Environment = env
        });

        if (result.IsSuccess)
            TempData["Success"] = "FBR Token updated successfully.";
        else
            TempData["Error"] = result.Message;

        return RedirectToAction(nameof(FbrSettings), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> SyncReferenceData(int companyId, string referenceType)
    {
        var result = await _sender.Send(new SyncFbrReferenceDataCommand
        {
            CompanyId = companyId,
            ReferenceType = referenceType
        });

        return Json(new { success = result.IsSuccess, message = result.Message });
    }

    private void PopulateViewBag()
    {
        ViewBag.Environments = new SelectList(
            Enum.GetValues<SubmissionEnvironment>()
                .Select(e => new { Value = (int)e, Text = e.ToString() }),
            "Value", "Text");
    }
}
