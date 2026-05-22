using FBR_DI.Application.DTOs.Invoice;
using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Queries.Invoice;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Features.Admin.Handlers.QueryHandlers;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInvoiceByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InvoiceDetailDto>> Handle(
        GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.Items)
            .Include(i => i.Company)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (invoice is null)
            throw new NotFoundException(nameof(Invoice), request.Id);

        var dto = new InvoiceDetailDto
        {
            Id = invoice.Id,
            CompanyId = invoice.CompanyId,
            CompanyName = invoice.Company.CompanyName,
            InvoiceType = invoice.InvoiceType,
            InvoiceDate = invoice.InvoiceDate,
            SellerNTNCNIC = invoice.SellerNTNCNIC,
            SellerBusinessName = invoice.SellerBusinessName,
            SellerProvince = invoice.SellerProvince,
            SellerAddress = invoice.SellerAddress,
            BuyerNTNCNIC = invoice.BuyerNTNCNIC,
            BuyerBusinessName = invoice.BuyerBusinessName,
            BuyerProvince = invoice.BuyerProvince,
            BuyerAddress = invoice.BuyerAddress,
            BuyerRegistrationType = invoice.BuyerRegistrationType,
            InvoiceRefNo = invoice.InvoiceRefNo,
            ScenarioId = invoice.ScenarioId,
            FbrInvoiceNumber = invoice.FbrInvoiceNumber,
            FbrSubmissionDate = invoice.FbrSubmissionDate,
            Status = invoice.Status,
            FbrRawResponse = invoice.FbrRawResponse,
            FbrErrorCode = invoice.FbrErrorCode,
            FbrErrorMessage = invoice.FbrErrorMessage,
            TotalValueExclST = invoice.TotalValueExclST,
            TotalSalesTax = invoice.TotalSalesTax,
            TotalFurtherTax = invoice.TotalFurtherTax,
            TotalExtraTax = invoice.TotalExtraTax,
            TotalFedPayable = invoice.TotalFedPayable,
            TotalDiscount = invoice.TotalDiscount,
            GrandTotal = invoice.GrandTotal,
            QrCodeBase64 = invoice.QrCodeBase64,
            CreatedAt = invoice.CreatedAt,
            Items = invoice.Items.OrderBy(i => i.ItemSerialNo).Select(i => new InvoiceItemDetailDto
            {
                Id = i.Id,
                ItemSerialNo = i.ItemSerialNo,
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
                SroItemSerialNo = i.SroItemSerialNo,
                FbrItemInvoiceNo = i.FbrItemInvoiceNo,
                ItemStatus = i.ItemStatus,
                ItemErrorCode = i.ItemErrorCode,
                ItemErrorMessage = i.ItemErrorMessage
            }).ToList()
        };

        return Result<InvoiceDetailDto>.Success(dto);
    }
}
