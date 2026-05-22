using FBR_DI.Application.Interfaces;
using FBR_DI.Persistence.Context;
using FBR_DI.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace FBR_DI.Persistence.HelperClasses;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Try to read from the Web project's appsettings first
        IConfiguration? config = null;
        var webAppSettings = Path.Combine(
            Directory.GetCurrentDirectory(), "../FBR_DI.Web", "appsettings.json");

        if (File.Exists(webAppSettings))
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FBR_DI.Web"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        // Resolve DatabaseSettings (fall back to SQLite so migrations work
        // on any machine without a running DB server)
        DatabaseSettings dbSettings;
        if (config is not null)
        {
            dbSettings = config
                .GetSection(DatabaseSettings.SectionName)
                .Get<DatabaseSettings>()
                ?? new DatabaseSettings
                {
                    Provider         = "SqlServer",
                    ConnectionString = config.GetConnectionString("DefaultConnection")
                                       ?? "Data Source=fbr_di_design.db"
                };

            if (string.IsNullOrWhiteSpace(dbSettings.ConnectionString))
                dbSettings.ConnectionString =
                    config.GetConnectionString("DefaultConnection")
                    ?? "Data Source=fbr_di_design.db";
        }
        else
        {
            // No config found — safe SQLite fallback for CI / fresh machines
            dbSettings = new DatabaseSettings
            {
                Provider         = "SQLite",
                ConnectionString = "Data Source=fbr_di_design.db"
            };
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        PersistenceServiceExtensions.ConfigureDbProvider(optionsBuilder, dbSettings);

        return new ApplicationDbContext(optionsBuilder.Options, new DesignTimeCurrentUserService());
    }
}

internal class DesignTimeCurrentUserService : ICurrentUserService
{
    public string? UserId        => "design-time";
    public string? Email         => "design-time@system.com";
    public string? FullName      => "Design Time";
    public bool    IsAuthenticated => false;
    public IEnumerable<string> Roles => Enumerable.Empty<string>();
}
