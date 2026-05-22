using FBR_DI.Application.DTOs.FbrApi;
using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Commands.Invoice;

public class SubmitInvoiceToFbrCommand : IRequest<Result<PostInvoiceResponseDto>>
{
    public int InvoiceId { get; set; }
}
