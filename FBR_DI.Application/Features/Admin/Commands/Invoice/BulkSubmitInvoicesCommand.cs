using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Commands.Invoice;

public class BulkSubmitInvoicesCommand : IRequest<Result<BulkSubmitResultDto>>
{
    public List<int> InvoiceIds { get; set; } = new();
}

public class BulkSubmitResultDto
{
    public int TotalRequested { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<BulkSubmitItemResult> Results { get; set; } = new();
}

public class BulkSubmitItemResult
{
    public int InvoiceId { get; set; }
    public bool Success { get; set; }
    public string? FbrInvoiceNumber { get; set; }
    public string? ErrorMessage { get; set; }
}
