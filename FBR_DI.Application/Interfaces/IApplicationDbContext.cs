using FBR_DI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceItem> InvoiceItems { get; }
    DbSet<FbrReferenceData> FbrReferenceDatas { get; }
    DbSet<AuditLog> AuditLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
