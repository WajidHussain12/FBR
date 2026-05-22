using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Commands.Invoice;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using FBR_DI.Domain.Enums;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Handlers.CommandHandlers;

public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public DeleteInvoiceCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<Result<bool>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices.FindAsync(new object[] { request.Id }, cancellationToken);
        if (invoice is null)
            throw new NotFoundException(nameof(Invoice), request.Id);

        if (invoice.Status != InvoiceStatus.Draft)
            return Result<bool>.Failure("Only Draft invoices can be deleted.");

        invoice.IsDeleted = true;
        invoice.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Delete", nameof(Invoice), invoice.Id.ToString(),
            new { invoice.Status }, null, cancellationToken);

        return Result<bool>.Success(true, "Invoice deleted successfully.");
    }
}
