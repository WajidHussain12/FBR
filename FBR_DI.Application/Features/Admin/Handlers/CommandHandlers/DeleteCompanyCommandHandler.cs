using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Commands.Company;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Handlers.CommandHandlers;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public DeleteCompanyCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<Result<bool>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FindAsync(new object[] { request.Id }, cancellationToken);
        if (company is null)
            throw new NotFoundException(nameof(Company), request.Id);

        company.IsDeleted = true;
        company.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Delete", nameof(Company), company.Id.ToString(),
            new { company.CompanyName }, null, cancellationToken);

        return Result<bool>.Success(true, "Company deleted successfully.");
    }
}
