using FluentValidation;
using FBR_DI.Application.Features.Admin.Commands.Invoice;

namespace FBR_DI.Application.Validators;

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    private static readonly string[] ValidInvoiceTypes = { "SaleInvoice", "Sale Invoice", "DebitNote", "Debit Note" };
    private static readonly string[] ValidRegistrationTypes = { "Registered", "Unregistered" };

    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .GreaterThan(0).WithMessage("A valid Company must be selected.");

        RuleFor(x => x.InvoiceType)
            .NotEmpty().WithMessage("Invoice Type is required.")
            .Must(t => ValidInvoiceTypes.Contains(t))
            .WithMessage("Invoice Type must be 'Sale Invoice' or 'Debit Note'.");

        RuleFor(x => x.InvoiceDate)
            .NotEmpty().WithMessage("Invoice Date is required.")
            .Must(d => d <= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Invoice Date cannot be in the future.");

        RuleFor(x => x.SellerNTNCNIC)
            .NotEmpty().WithMessage("Seller NTN/CNIC is required.")
            .Matches(@"^\d{7}$|^\d{13}$")
            .WithMessage("Seller NTN/CNIC must be 7 or 13 digits.");

        RuleFor(x => x.SellerBusinessName)
            .NotEmpty().WithMessage("Seller Business Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.SellerProvince)
            .NotEmpty().WithMessage("Seller Province is required.")
            .MaximumLength(100);

        RuleFor(x => x.SellerAddress)
            .NotEmpty().WithMessage("Seller Address is required.")
            .MaximumLength(500);

        RuleFor(x => x.BuyerNTNCNIC)
            .Matches(@"^\d{7}$|^\d{13}$")
            .WithMessage("Buyer NTN/CNIC must be 7 or 13 digits.")
            .When(x => !string.IsNullOrWhiteSpace(x.BuyerNTNCNIC));

        RuleFor(x => x.BuyerNTNCNIC)
            .NotEmpty().WithMessage("Buyer NTN/CNIC is required for Registered buyers.")
            .When(x => x.BuyerRegistrationType == "Registered");

        RuleFor(x => x.BuyerBusinessName)
            .NotEmpty().WithMessage("Buyer Business Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.BuyerProvince)
            .NotEmpty().WithMessage("Buyer Province is required.")
            .MaximumLength(100);

        RuleFor(x => x.BuyerAddress)
            .NotEmpty().WithMessage("Buyer Address is required.")
            .MaximumLength(500);

        RuleFor(x => x.BuyerRegistrationType)
            .NotEmpty().WithMessage("Buyer Registration Type is required.")
            .Must(t => ValidRegistrationTypes.Contains(t))
            .WithMessage("Buyer Registration Type must be 'Registered' or 'Unregistered'.");

        RuleFor(x => x.InvoiceRefNo)
            .NotEmpty().WithMessage("Invoice Reference No is required for Debit Note.")
            .Must(r => r != null && (r.Length == 22 || r.Length == 28))
            .WithMessage("Invoice Reference No must be 22 (NTN) or 28 (CNIC) characters.")
            .When(x => x.InvoiceType is "DebitNote" or "Debit Note");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one invoice item is required.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateInvoiceItemDtoValidator());
    }
}

public class CreateInvoiceItemDtoValidator : AbstractValidator<DTOs.Invoice.CreateInvoiceItemDto>
{
    public CreateInvoiceItemDtoValidator()
    {
        RuleFor(x => x.HsCode)
            .NotEmpty().WithMessage("HS Code is required.")
            .Matches(@"^\d{4}\.\d{4}$").WithMessage("HS Code must be in format '####.####' (e.g. 1001.1000).");

        RuleFor(x => x.ProductDescription)
            .NotEmpty().WithMessage("Product Description is required.")
            .MaximumLength(500);

        RuleFor(x => x.Rate)
            .NotEmpty().WithMessage("Rate is required.")
            .MaximumLength(20);

        RuleFor(x => x.UoM)
            .NotEmpty().WithMessage("Unit of Measure is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.ValueSalesExcludingST)
            .GreaterThanOrEqualTo(0).WithMessage("Value Sales Excluding ST cannot be negative.");

        RuleFor(x => x.SalesTaxApplicable)
            .GreaterThanOrEqualTo(0).WithMessage("Sales Tax Applicable cannot be negative.");

        RuleFor(x => x.SaleType)
            .NotEmpty().WithMessage("Sale Type is required.");
    }
}
