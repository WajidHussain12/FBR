using Microsoft.AspNetCore.Identity;

namespace FBR_DI.Persistence.IdentityModels;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? ProfilePicture { get; set; }
    public int? CompanyId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
