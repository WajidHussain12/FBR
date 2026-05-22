using FBR_DI.Application.Features.Admin.Commands.Company;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using FBR_DI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Features.Admin.Handlers.CommandHandlers;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public CreateCompanyCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<Result<int>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Companies
            .AnyAsync(c => c.NTN == request.NTN, cancellationToken);

        if (exists)
            return Result<int>.Failure($"A company with NTN '{request.NTN}' already exists.");

        var company = new Company
        {
            CompanyName = request.CompanyName,
            NTN = request.NTN,
            CNIC = request.CNIC,
            Province = request.Province,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            BusinessActivity = request.BusinessActivity,
            Sector = request.Sector,
            FbrBearerToken = request.FbrBearerToken,
            SubmissionEnvironment = request.SubmissionEnvironment,
            IsIntegrated = !string.IsNullOrWhiteSpace(request.FbrBearerToken)
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("Create", nameof(Company), company.Id.ToString(),
            null, new { company.CompanyName, company.NTN }, cancellationToken);

        return Result<int>.Success(company.Id, "Company created successfully.");
    }
}
