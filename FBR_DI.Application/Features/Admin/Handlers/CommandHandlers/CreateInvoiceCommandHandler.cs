using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Commands.Invoice;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using FBR_DI.Domain.Enums;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Handlers.CommandHandlers;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public CreateInvoiceCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<Result<int>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FindAsync(new object[] { request.CompanyId }, cancellationToken);
        if (company is null)
            throw new NotFoundException(nameof(Company), request.CompanyId);

        var invoiceType = Enum.Parse<InvoiceType>(request.InvoiceType.Replace(" ", ""));
        var buyerRegType = Enum.Parse<BuyerRegistrationType>(request.BuyerRegistrationType.Replace(" ", ""));

        var invoice = new Invoice
        {
            CompanyId = request.CompanyId,
            InvoiceType = invoiceType,
            InvoiceDate = request.InvoiceDate,
            SellerNTNCNIC = request.SellerNTNCNIC,
            SellerBusinessName = request.SellerBusinessName,
            SellerProvince = request.SellerProvince,
            SellerAddress = request.SellerAddress,
            BuyerNTNCNIC = request.BuyerNTNCNIC,
            BuyerBusinessName = request.BuyerBusinessName,
            BuyerProvince = request.BuyerProvince,
            BuyerAddress = request.BuyerAddress,
            BuyerRegistrationType = buyerRegType,
            InvoiceRefNo = request.InvoiceRefNo,
            ScenarioId = request.ScenarioId,
            Status = InvoiceStatus.Draft
        };

        var items = request.Items.Select((itemDto, index) => new InvoiceItem
        {
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

        invoice.Items = items;

        // Calculate totals
        invoice.TotalValueExclST = items.Sum(x => x.ValueSalesExcludingST);
        invoice.TotalSalesTax = items.Sum(x => x.SalesTaxApplicable);
        invoice.TotalFurtherTax = items.Sum(x => x.FurtherTax);
        invoice.TotalExtraTax = items.Sum(x => x.ExtraTax);
        invoice.TotalFedPayable = items.Sum(x => x.FedPayable);
        invoice.TotalDiscount = items.Sum(x => x.Discount);
        invoice.GrandTotal = invoice.TotalValueExclST + invoice.TotalSalesTax +
                             invoice.TotalFurtherTax + invoice.TotalExtraTax +
                             invoice.TotalFedPayable - invoice.TotalDiscount;

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("Create", nameof(Invoice), invoice.Id.ToString(),
            null, new { invoice.InvoiceType, invoice.GrandTotal }, cancellationToken);

        return Result<int>.Success(invoice.Id, "Invoice created successfully.");
    }
}
