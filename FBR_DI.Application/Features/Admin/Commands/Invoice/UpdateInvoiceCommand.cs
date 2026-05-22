using FBR_DI.Application.DTOs.Invoice;
using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Commands.Invoice;

public class UpdateInvoiceCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
    public string InvoiceType { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public string SellerNTNCNIC { get; set; } = string.Empty;
    public string SellerBusinessName { get; set; } = string.Empty;
    public string SellerProvince { get; set; } = string.Empty;
    public string SellerAddress { get; set; } = string.Empty;
    public string? BuyerNTNCNIC { get; set; }
    public string BuyerBusinessName { get; set; } = string.Empty;
    public string BuyerProvince { get; set; } = string.Empty;
    public string BuyerAddress { get; set; } = string.Empty;
    public string BuyerRegistrationType { get; set; } = string.Empty;
    public string? InvoiceRefNo { get; set; }
    public string? ScenarioId { get; set; }
    public List<UpdateInvoiceItemDto> Items { get; set; } = new();
}
