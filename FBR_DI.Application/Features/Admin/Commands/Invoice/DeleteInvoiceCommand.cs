using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Commands.Invoice;

public class DeleteInvoiceCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
}
