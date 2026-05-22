using System.Text.Json;
using FBR_DI.Application.DTOs.FbrApi;
using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Commands.Invoice;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using FBR_DI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Features.Admin.Handlers.CommandHandlers;

public class SubmitInvoiceToFbrCommandHandler : IRequestHandler<SubmitInvoiceToFbrCommand, Result<PostInvoiceResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFbrApiService _fbrApiService;
    private readonly IQrCodeService _qrCodeService;
    private readonly IAuditService _auditService;

    public SubmitInvoiceToFbrCommandHandler(
        IApplicationDbContext context,
        IFbrApiService fbrApiService,
        IQrCodeService qrCodeService,
        IAuditService auditService)
    {
        _context = context;
        _fbrApiService = fbrApiService;
        _qrCodeService = qrCodeService;
        _auditService = auditService;
    }

    public async Task<Result<PostInvoiceResponseDto>> Handle(
        SubmitInvoiceToFbrCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Company)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice is null)
            throw new NotFoundException(nameof(Invoice), request.InvoiceId);

        var company = invoice.Company;
        if (string.IsNullOrWhiteSpace(company.FbrBearerToken))
            return Result<PostInvoiceResponseDto>.Failure("Company does not have an FBR Bearer Token configured.");

        var isSandbox = company.SubmissionEnvironment == SubmissionEnvironment.Sandbox;

        var requestDto = MapToRequestDto(invoice, isSandbox);

        invoice.Status = InvoiceStatus.PendingSubmission;
        await _context.SaveChangesAsync(cancellationToken);

        var apiResult = await _fbrApiService.PostInvoiceAsync(
            requestDto, company.FbrBearerToken, isSandbox, cancellationToken);

        if (apiResult.IsFailure)
        {
            invoice.Status = InvoiceStatus.Failed;
            invoice.FbrErrorMessage = apiResult.Message;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<PostInvoiceResponseDto>.Failure(apiResult.Message ?? "FBR submission failed.");
        }

        var response = apiResult.Data!;
        invoice.FbrRawResponse = JsonSerializer.Serialize(response);

        var statusCode = response.ValidationResponse?.StatusCode ?? "";

        if (statusCode == "00")
        {
            invoice.Status = InvoiceStatus.Valid;
            invoice.FbrInvoiceNumber = response.InvoiceNumber;
            invoice.FbrSubmissionDate = DateTime.UtcNow;
            invoice.FbrErrorCode = null;
            invoice.FbrErrorMessage = null;

            if (!string.IsNullOrWhiteSpace(response.InvoiceNumber))
                invoice.QrCodeBase64 = _qrCodeService.GenerateQrCodeBase64(response.InvoiceNumber);

            var statuses = response.ValidationResponse?.InvoiceStatuses ?? new List<InvoiceStatusDto>();
            foreach (var item in invoice.Items)
            {
                var itemStatus = statuses.FirstOrDefault(s => s.ItemSNo == item.ItemSerialNo.ToString());
                if (itemStatus != null)
                {
                    item.FbrItemInvoiceNo = itemStatus.InvoiceNo;
                    item.ItemStatus = itemStatus.Status;
                    item.ItemErrorCode = itemStatus.ErrorCode;
                    item.ItemErrorMessage = itemStatus.Error;
                }
            }
        }
        else
        {
            invoice.Status = InvoiceStatus.Invalid;
            invoice.FbrErrorCode = response.ValidationResponse?.ErrorCode;
            invoice.FbrErrorMessage = response.ValidationResponse?.Error;

            var statuses = response.ValidationResponse?.InvoiceStatuses ?? new List<InvoiceStatusDto>();
            foreach (var item in invoice.Items)
            {
                var itemStatus = statuses.FirstOrDefault(s => s.ItemSNo == item.ItemSerialNo.ToString());
                if (itemStatus != null)
                {
                    item.ItemStatus = itemStatus.Status;
                    item.ItemErrorCode = itemStatus.ErrorCode;
                    item.ItemErrorMessage = itemStatus.Error;
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("SubmitToFbr", nameof(Invoice), invoice.Id.ToString(),
            null, new { invoice.Status, invoice.FbrInvoiceNumber }, cancellationToken);

        return Result<PostInvoiceResponseDto>.Success(response, $"Invoice submission {invoice.Status}.");
    }

    private static PostInvoiceRequestDto MapToRequestDto(Invoice invoice, bool isSandbox)
    {
        var dto = new PostInvoiceRequestDto
        {
            InvoiceType = invoice.InvoiceType == InvoiceType.SaleInvoice ? "Sale Invoice" : "Debit Note",
            InvoiceDate = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
            SellerNTNCNIC = invoice.SellerNTNCNIC,
            SellerBusinessName = invoice.SellerBusinessName,
            SellerProvince = invoice.SellerProvince,
            SellerAddress = invoice.SellerAddress,
            BuyerNTNCNIC = invoice.BuyerNTNCNIC,
            BuyerBusinessName = invoice.BuyerBusinessName,
            BuyerProvince = invoice.BuyerProvince,
            BuyerAddress = invoice.BuyerAddress,
            BuyerRegistrationType = invoice.BuyerRegistrationType == BuyerRegistrationType.Registered
                ? "Registered" : "Unregistered",
            InvoiceRefNo = invoice.InvoiceRefNo,
            ScenarioId = isSandbox ? invoice.ScenarioId : null,
            Items = invoice.Items.Select(i => new InvoiceItemRequestDto
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
        return dto;
    }
}
