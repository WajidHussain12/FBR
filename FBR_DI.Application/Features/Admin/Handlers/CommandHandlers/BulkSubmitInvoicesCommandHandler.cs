using FBR_DI.Application.Features.Admin.Commands.Invoice;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Handlers.CommandHandlers;

public class BulkSubmitInvoicesCommandHandler : IRequestHandler<BulkSubmitInvoicesCommand, Result<BulkSubmitResultDto>>
{
    private readonly ISender _sender;

    public BulkSubmitInvoicesCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<Result<BulkSubmitResultDto>> Handle(
        BulkSubmitInvoicesCommand request, CancellationToken cancellationToken)
    {
        var result = new BulkSubmitResultDto
        {
            TotalRequested = request.InvoiceIds.Count
        };

        foreach (var invoiceId in request.InvoiceIds)
        {
            var submitResult = await _sender.Send(
                new SubmitInvoiceToFbrCommand { InvoiceId = invoiceId }, cancellationToken);

            var itemResult = new BulkSubmitItemResult
            {
                InvoiceId = invoiceId,
                Success = submitResult.IsSuccess,
                FbrInvoiceNumber = submitResult.Data?.InvoiceNumber,
                ErrorMessage = submitResult.IsFailure ? submitResult.Message : null
            };

            result.Results.Add(itemResult);

            if (submitResult.IsSuccess)
                result.SuccessCount++;
            else
                result.FailureCount++;
        }

        return Result<BulkSubmitResultDto>.Success(result,
            $"Bulk submission complete: {result.SuccessCount} succeeded, {result.FailureCount} failed.");
    }
}
