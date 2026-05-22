using System.Text.Json;
using FBR_DI.Application.Interfaces;
using FBR_DI.Domain.Entities;

namespace FBR_DI.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AuditService(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task LogAsync(string action, string entityName, string? entityId,
        object? oldValues, object? newValues, CancellationToken ct)
    {
        var log = new AuditLog
        {
            UserId = _currentUser.UserId ?? "system",
            UserEmail = _currentUser.Email ?? "system",
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues is null ? null : JsonSerializer.Serialize(oldValues),
            NewValues = newValues is null ? null : JsonSerializer.Serialize(newValues),
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync(ct);
    }
}
