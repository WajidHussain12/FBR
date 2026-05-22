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

public class ValidateInvoiceCommandHandler : IRequestHandler<ValidateInvoiceCommand, Result<PostInvoiceResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFbrApiService _fbrApiService;

    public ValidateInvoiceCommandHandler(IApplicationDbContext context, IFbrApiService fbrApiService)
    {
        _context = context;
        _fbrApiService = fbrApiService;
    }

    public async Task<Result<PostInvoiceResponseDto>> Handle(
        ValidateInvoiceCommand request, CancellationToken cancellationToken)
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

        var apiResult = await _fbrApiService.ValidateInvoiceAsync(
            requestDto, company.FbrBearerToken, isSandbox, cancellationToken);

        if (apiResult.IsFailure)
            return Result<PostInvoiceResponseDto>.Failure(apiResult.Message ?? "FBR validation failed.");

        return Result<PostInvoiceResponseDto>.Success(apiResult.Data!, "Validation complete.");
    }

    private static PostInvoiceRequestDto MapToRequestDto(Invoice invoice, bool isSandbox)
        => new()
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
}
