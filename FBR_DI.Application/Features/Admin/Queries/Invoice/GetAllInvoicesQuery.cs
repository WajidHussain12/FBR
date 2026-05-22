using FBR_DI.Application.DTOs.Invoice;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Application.Wrappers;
using FBR_DI.Domain.Enums;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Queries.Invoice;

public class GetAllInvoicesQuery : IRequest<Result<PagedResult<InvoiceListDto>>>
{
    public PaginationParams Params { get; set; } = new();
    public int? CompanyId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
}
