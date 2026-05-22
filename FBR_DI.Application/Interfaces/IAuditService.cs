namespace FBR_DI.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string entityName, string? entityId,
                  object? oldValues, object? newValues, CancellationToken ct);
}
