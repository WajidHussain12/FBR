using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Commands.Company;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Handlers.CommandHandlers;

public class UpdateFbrTokenCommandHandler : IRequestHandler<UpdateFbrTokenCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UpdateFbrTokenCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<Result<bool>> Handle(UpdateFbrTokenCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FindAsync(new object[] { request.CompanyId }, cancellationToken);
        if (company is null)
            throw new NotFoundException(nameof(Company), request.CompanyId);

        company.FbrBearerToken = request.BearerToken;
        company.SubmissionEnvironment = request.Environment;
        company.IsIntegrated = !string.IsNullOrWhiteSpace(request.BearerToken);

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("UpdateFbrToken", nameof(Company), company.Id.ToString(),
            null, new { company.SubmissionEnvironment, company.IsIntegrated }, cancellationToken);

        return Result<bool>.Success(true, "FBR token updated successfully.");
    }
}
