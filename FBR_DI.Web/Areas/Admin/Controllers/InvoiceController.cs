using FBR_DI.Application.DTOs.Invoice;
using FBR_DI.Application.Features.Admin.Commands.Invoice;
using FBR_DI.Application.Features.Admin.Queries.Invoice;
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
public class InvoiceController : Controller
{
    private readonly ISender _sender;

    public InvoiceController(ISender sender)
    {
        _sender = sender;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int draw, int start, int length,
        string? searchValue, int? companyId,
        string? status, string? fromDate, string? toDate)
    {
        InvoiceStatus? invoiceStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<InvoiceStatus>(status, out var parsedStatus))
            invoiceStatus = parsedStatus;

        DateOnly? from = DateOnly.TryParse(fromDate, out var fd) ? fd : null;
        DateOnly? to = DateOnly.TryParse(toDate, out var td) ? td : null;

        var result = await _sender.Send(new GetAllInvoicesQuery
        {
            Params = new PaginationParams
            {
                PageNumber = (start / (length > 0 ? length : 10)) + 1,
                PageSize = length > 0 ? length : 10,
                SearchTerm = searchValue
            },
            CompanyId = companyId,
            Status = invoiceStatus,
            FromDate = from,
            ToDate = to
        });

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
    public IActionResult Create(int? companyId)
    {
        PopulateViewBag(companyId);
        return View(new CreateInvoiceViewModel { CompanyId = companyId ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInvoiceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulateViewBag(model.CompanyId);
            return View(model);
        }

        var command = new CreateInvoiceCommand
        {
            CompanyId = model.CompanyId,
            InvoiceType = model.InvoiceType,
            InvoiceDate = model.InvoiceDate,
            SellerNTNCNIC = model.SellerNTNCNIC,
            SellerBusinessName = model.SellerBusinessName,
            SellerProvince = model.SellerProvince,
            SellerAddress = model.SellerAddress,
            BuyerNTNCNIC = model.BuyerNTNCNIC,
            BuyerBusinessName = model.BuyerBusinessName,
            BuyerProvince = model.BuyerProvince,
            BuyerAddress = model.BuyerAddress,
            BuyerRegistrationType = model.BuyerRegistrationType,
            InvoiceRefNo = model.InvoiceRefNo,
            ScenarioId = model.ScenarioId,
            Items = model.Items.Select(i => new CreateInvoiceItemDto
            {
                HsCode = i.HsCode,
                ProductDescription = i.ProductDescription,
                Rate = i.Rate,
                UoM = i.UoM,
                Quantity = i.Quantity,
                TotalValues = i.TotalValues,
                ValueSalesExcludingST = i.ValueSalesExcludingST,
                FixedNotifiedValueOrRetailPrice = i.FixedNotifiedValueOrRetailPrice,
                SalesTaxApplicable = i.SalesTaxApplicable,
                SalesTaxWithheldAtSource = i.SalesTaxWithheldAtSource,
                ExtraTax = i.ExtraTax,
                FurtherTax = i.FurtherTax,
                SroScheduleNo = i.SroScheduleNo,
                FedPayable = i.FedPayable,
                Discount = i.Discount,
                SaleType = i.SaleType,
                SroItemSerialNo = i.SroItemSerialNo
            }).ToList()
        };

        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            TempData["Success"] = "Invoice created successfully.";
            return RedirectToAction(nameof(Detail), new { id = result.Data });
        }

        ModelState.AddModelError(string.Empty, result.Message ?? "Failed to create invoice.");
        PopulateViewBag(model.CompanyId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _sender.Send(new GetInvoiceByIdQuery { Id = id });
        if (result.IsFailure)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        var dto = result.Data!;
        if (dto.Status != InvoiceStatus.Draft)
        {
            TempData["Error"] = "Only Draft invoices can be edited.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        var model = MapToEditViewModel(dto);
        PopulateViewBag(dto.CompanyId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateInvoiceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulateViewBag(model.CompanyId);
            return View(model);
        }

        var command = new UpdateInvoiceCommand
        {
            Id = id,
            InvoiceType = model.InvoiceType,
            InvoiceDate = model.InvoiceDate,
            SellerNTNCNIC = model.SellerNTNCNIC,
            SellerBusinessName = model.SellerBusinessName,
            SellerProvince = model.SellerProvince,
            SellerAddress = model.SellerAddress,
            BuyerNTNCNIC = model.BuyerNTNCNIC,
            BuyerBusinessName = model.BuyerBusinessName,
            BuyerProvince = model.BuyerProvince,
            BuyerAddress = model.BuyerAddress,
            BuyerRegistrationType = model.BuyerRegistrationType,
            InvoiceRefNo = model.InvoiceRefNo,
            ScenarioId = model.ScenarioId,
            Items = model.Items.Select(i => new UpdateInvoiceItemDto
            {
                HsCode = i.HsCode,
                ProductDescription = i.ProductDescription,
                Rate = i.Rate,
                UoM = i.UoM,
                Quantity = i.Quantity,
                TotalValues = i.TotalValues,
                ValueSalesExcludingST = i.ValueSalesExcludingST,
                FixedNotifiedValueOrRetailPrice = i.FixedNotifiedValueOrRetailPrice,
                SalesTaxApplicable = i.SalesTaxApplicable,
                SalesTaxWithheldAtSource = i.SalesTaxWithheldAtSource,
                ExtraTax = i.ExtraTax,
                FurtherTax = i.FurtherTax,
                SroScheduleNo = i.SroScheduleNo,
                FedPayable = i.FedPayable,
                Discount = i.Discount,
                SaleType = i.SaleType,
                SroItemSerialNo = i.SroItemSerialNo
            }).ToList()
        };

        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            TempData["Success"] = "Invoice updated successfully.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        ModelState.AddModelError(string.Empty, result.Message ?? "Failed to update invoice.");
        PopulateViewBag(model.CompanyId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var result = await _sender.Send(new GetInvoiceByIdQuery { Id = id });
        if (result.IsFailure)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Submit(int id)
    {
        var result = await _sender.Send(new SubmitInvoiceToFbrCommand { InvoiceId = id });
        return Json(new
        {
            success = result.IsSuccess,
            message = result.Message,
            invoiceNumber = result.Data?.InvoiceNumber,
            statusCode = result.Data?.ValidationResponse?.StatusCode,
            error = result.Data?.ValidationResponse?.Error
        });
    }

    [HttpPost]
    public async Task<IActionResult> Validate(int id)
    {
        var result = await _sender.Send(new ValidateInvoiceCommand { InvoiceId = id });
        return Json(new
        {
            success = result.IsSuccess,
            message = result.Message,
            statusCode = result.Data?.ValidationResponse?.StatusCode,
            status = result.Data?.ValidationResponse?.Status,
            error = result.Data?.ValidationResponse?.Error,
            invoiceStatuses = result.Data?.ValidationResponse?.InvoiceStatuses
        });
    }

    [HttpPost]
    public async Task<IActionResult> BulkSubmit([FromBody] List<int> ids)
    {
        var result = await _sender.Send(new BulkSubmitInvoicesCommand { InvoiceIds = ids });
        return Json(new
        {
            success = result.IsSuccess,
            message = result.Message,
            data = result.Data
        });
    }

    [HttpGet]
    public async Task<IActionResult> PrintInvoice(int id)
    {
        var result = await _sender.Send(new GetInvoiceByIdQuery { Id = id });
        if (result.IsFailure)
            return NotFound();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _sender.Send(new DeleteInvoiceCommand { Id = id });
        return Json(new { success = result.IsSuccess, message = result.Message });
    }

    private static CreateInvoiceViewModel MapToEditViewModel(
        FBR_DI.Application.DTOs.Invoice.InvoiceDetailDto dto)
    {
        return new CreateInvoiceViewModel
        {
            CompanyId = dto.CompanyId,
            InvoiceType = dto.InvoiceType.ToString(),
            InvoiceDate = dto.InvoiceDate,
            SellerNTNCNIC = dto.SellerNTNCNIC,
            SellerBusinessName = dto.SellerBusinessName,
            SellerProvince = dto.SellerProvince,
            SellerAddress = dto.SellerAddress,
            BuyerNTNCNIC = dto.BuyerNTNCNIC,
            BuyerBusinessName = dto.BuyerBusinessName,
            BuyerProvince = dto.BuyerProvince,
            BuyerAddress = dto.BuyerAddress,
            BuyerRegistrationType = dto.BuyerRegistrationType.ToString(),
            InvoiceRefNo = dto.InvoiceRefNo,
            ScenarioId = dto.ScenarioId,
            Items = dto.Items.Select(i => new CreateInvoiceItemViewModel
            {
                HsCode = i.HsCode,
                ProductDescription = i.ProductDescription,
                Rate = i.Rate,
                UoM = i.UoM,
                Quantity = i.Quantity,
                TotalValues = i.TotalValues,
                ValueSalesExcludingST = i.ValueSalesExcludingST,
                FixedNotifiedValueOrRetailPrice = i.FixedNotifiedValueOrRetailPrice,
                SalesTaxApplicable = i.SalesTaxApplicable,
                SalesTaxWithheldAtSource = i.SalesTaxWithheldAtSource,
                ExtraTax = i.ExtraTax,
                FurtherTax = i.FurtherTax,
                SroScheduleNo = i.SroScheduleNo,
                FedPayable = i.FedPayable,
                Discount = i.Discount,
                SaleType = i.SaleType,
                SroItemSerialNo = i.SroItemSerialNo
            }).ToList()
        };
    }

    private void PopulateViewBag(int? selectedCompanyId = null)
    {
        ViewBag.InvoiceTypes = new SelectList(new[]
        {
            new { Value = "SaleInvoice", Text = "Sale Invoice" },
            new { Value = "DebitNote",   Text = "Debit Note" }
        }, "Value", "Text");

        ViewBag.RegistrationTypes = new SelectList(new[]
        {
            new { Value = "Registered",   Text = "Registered" },
            new { Value = "Unregistered", Text = "Unregistered" }
        }, "Value", "Text");
    }
}
