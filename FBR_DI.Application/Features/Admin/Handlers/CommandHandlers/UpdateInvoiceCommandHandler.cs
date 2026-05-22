using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Commands.Invoice;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using FBR_DI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Features.Admin.Handlers.CommandHandlers;

public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UpdateInvoiceCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<Result<bool>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (invoice is null)
            throw new NotFoundException(nameof(Invoice), request.Id);

        if (invoice.Status != InvoiceStatus.Draft)
            return Result<bool>.Failure("Only Draft invoices can be edited.");

        var invoiceType = Enum.Parse<InvoiceType>(request.InvoiceType.Replace(" ", ""));
        var buyerRegType = Enum.Parse<BuyerRegistrationType>(request.BuyerRegistrationType.Replace(" ", ""));

        invoice.InvoiceType = invoiceType;
        invoice.InvoiceDate = request.InvoiceDate;
        invoice.SellerNTNCNIC = request.SellerNTNCNIC;
        invoice.SellerBusinessName = request.SellerBusinessName;
        invoice.SellerProvince = request.SellerProvince;
        invoice.SellerAddress = request.SellerAddress;
        invoice.BuyerNTNCNIC = request.BuyerNTNCNIC;
        invoice.BuyerBusinessName = request.BuyerBusinessName;
        invoice.BuyerProvince = request.BuyerProvince;
        invoice.BuyerAddress = request.BuyerAddress;
        invoice.BuyerRegistrationType = buyerRegType;
        invoice.InvoiceRefNo = request.InvoiceRefNo;
        invoice.ScenarioId = request.ScenarioId;

        // Remove old items, add new
        foreach (var item in invoice.Items.ToList())
            _context.InvoiceItems.Remove(item);

        var newItems = request.Items.Select((itemDto, index) => new InvoiceItem
        {
            InvoiceId = invoice.Id,
            ItemSerialNo = index + 1,
            HsCode = itemDto.HsCode,
            ProductDescription = itemDto.ProductDescription,
            Rate = itemDto.Rate,
            UoM = itemDto.UoM,
            Quantity = itemDto.Quantity,
            TotalValues = itemDto.TotalValues,
            ValueSalesExcludingST = itemDto.ValueSalesExcludingST,
            FixedNotifiedValueOrRetailPrice = itemDto.FixedNotifiedValueOrRetailPrice,
            SalesTaxApplicable = itemDto.SalesTaxApplicable,
            SalesTaxWithheldAtSource = itemDto.SalesTaxWithheldAtSource,
            ExtraTax = itemDto.ExtraTax,
            FurtherTax = itemDto.FurtherTax,
            SroScheduleNo = itemDto.SroScheduleNo,
            FedPayable = itemDto.FedPayable,
            Discount = itemDto.Discount,
            SaleType = itemDto.SaleType,
            SroItemSerialNo = itemDto.SroItemSerialNo
        }).ToList();

        invoice.Items = newItems;

        invoice.TotalValueExclST = newItems.Sum(x => x.ValueSalesExcludingST);
        invoice.TotalSalesTax = newItems.Sum(x => x.SalesTaxApplicable);
        invoice.TotalFurtherTax = newItems.Sum(x => x.FurtherTax);
        invoice.TotalExtraTax = newItems.Sum(x => x.ExtraTax);
        invoice.TotalFedPayable = newItems.Sum(x => x.FedPayable);
        invoice.TotalDiscount = newItems.Sum(x => x.Discount);
        invoice.GrandTotal = invoice.TotalValueExclST + invoice.TotalSalesTax +
                             invoice.TotalFurtherTax + invoice.TotalExtraTax +
                             invoice.TotalFedPayable - invoice.TotalDiscount;

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Update", nameof(Invoice), invoice.Id.ToString(),
            null, new { invoice.Status, invoice.GrandTotal }, cancellationToken);

        return Result<bool>.Success(true, "Invoice updated successfully.");
    }
}
