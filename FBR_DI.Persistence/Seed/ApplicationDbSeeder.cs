using FBR_DI.Domain.Entities;
using FBR_DI.Persistence.Context;
using FBR_DI.Persistence.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FBR_DI.Persistence.Seed;

public static class ApplicationDbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Create roles
            string[] roles = { "Admin", "Manager", "Operator" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = role,
                        Description = $"{role} role"
                    });
                    logger.LogInformation("Role '{Role}' created.", role);
                }
            }

            // Create default admin user
            const string adminEmail = "admin@fbr-di.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Default admin user created: {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Failed to create admin user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Seed FBR Reference Data - Provinces
            await SeedProvincesAsync(services, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private static async Task SeedProvincesAsync(IServiceProvider services, ILogger logger)
    {
        var context = services.GetRequiredService<Context.ApplicationDbContext>();

        if (await context.FbrReferenceDatas.IgnoreQueryFilters()
            .AnyAsync(r => r.ReferenceType == "Province"))
            return;

        var provinces = new List<FbrReferenceData>
        {
            new() { ReferenceType = "Province", Code = "7",  Description = "Punjab",             SortOrder = 1, LastSyncedAt = DateTime.UtcNow },
            new() { ReferenceType = "Province", Code = "8",  Description = "Sindh",              SortOrder = 2, LastSyncedAt = DateTime.UtcNow },
            new() { ReferenceType = "Province", Code = "9",  Description = "Khyber Pakhtunkhwa", SortOrder = 3, LastSyncedAt = DateTime.UtcNow },
            new() { ReferenceType = "Province", Code = "10", Description = "Balochistan",        SortOrder = 4, LastSyncedAt = DateTime.UtcNow },
            new() { ReferenceType = "Province", Code = "11", Description = "AJK",                SortOrder = 5, LastSyncedAt = DateTime.UtcNow },
            new() { ReferenceType = "Province", Code = "12", Description = "Gilgit-Baltistan",   SortOrder = 6, LastSyncedAt = DateTime.UtcNow },
            new() { ReferenceType = "Province", Code = "13", Description = "ICT (Islamabad)",    SortOrder = 7, LastSyncedAt = DateTime.UtcNow }
        };

        context.FbrReferenceDatas.AddRange(provinces);
        await context.SaveChangesAsync(default);
        logger.LogInformation("Seeded {Count} provinces.", provinces.Count);
    }
}
