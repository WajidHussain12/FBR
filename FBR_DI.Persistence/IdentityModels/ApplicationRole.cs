using Microsoft.AspNetCore.Identity;

namespace FBR_DI.Persistence.IdentityModels;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
}
