using System.Security.Claims;
using FBR_DI.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FBR_DI.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;
    public string? FullName => User?.FindFirst(ClaimTypes.Name)?.Value;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    public IEnumerable<string> Roles => User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();
}
