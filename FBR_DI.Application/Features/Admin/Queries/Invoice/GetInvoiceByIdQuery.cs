using FBR_DI.Application.DTOs.Invoice;
using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Queries.Invoice;

public class GetInvoiceByIdQuery : IRequest<Result<InvoiceDetailDto>>
{
    public int Id { get; set; }
}
