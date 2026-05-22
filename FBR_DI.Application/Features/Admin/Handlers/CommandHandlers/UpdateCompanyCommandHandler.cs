using FBR_DI.Application.Exceptions;
using FBR_DI.Application.Features.Admin.Commands.Company;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Features.Admin.Handlers.CommandHandlers;

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UpdateCompanyCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<Result<bool>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FindAsync(new object[] { request.Id }, cancellationToken);
        if (company is null)
            throw new NotFoundException(nameof(Company), request.Id);

        var ntnConflict = await _context.Companies
            .AnyAsync(c => c.NTN == request.NTN && c.Id != request.Id, cancellationToken);
        if (ntnConflict)
            return Result<bool>.Failure($"NTN '{request.NTN}' is already used by another company.");

        var old = new { company.CompanyName, company.NTN, company.Province };

        company.CompanyName = request.CompanyName;
        company.NTN = request.NTN;
        company.CNIC = request.CNIC;
        company.Province = request.Province;
        company.Address = request.Address;
        company.PhoneNumber = request.PhoneNumber;
        company.Email = request.Email;
        company.BusinessActivity = request.BusinessActivity;
        company.Sector = request.Sector;
        company.SubmissionEnvironment = request.SubmissionEnvironment;

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Update", nameof(Company), company.Id.ToString(),
            old, new { company.CompanyName, company.NTN }, cancellationToken);

        return Result<bool>.Success(true, "Company updated successfully.");
    }
}
