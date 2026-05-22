namespace FBR_DI.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? FullName { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
}
