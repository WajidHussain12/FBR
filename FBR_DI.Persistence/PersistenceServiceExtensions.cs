using FBR_DI.Application.Interfaces;
using FBR_DI.Persistence.Context;
using FBR_DI.Persistence.Enums;
using FBR_DI.Persistence.IdentityModels;
using FBR_DI.Persistence.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FBR_DI.Persistence;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettings = configuration
            .GetSection(DatabaseSettings.SectionName)
            .Get<DatabaseSettings>()
            ?? new DatabaseSettings
            {
                Provider         = "SqlServer",
                ConnectionString = configuration.GetConnectionString("DefaultConnection")
                                   ?? "Data Source=fbr_di.db"
            };

        // Ensure connection string is populated even when section exists but
        // ConnectionString was omitted (fall back to legacy key)
        if (string.IsNullOrWhiteSpace(dbSettings.ConnectionString))
            dbSettings.ConnectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=fbr_di.db";

        services.AddDbContext<ApplicationDbContext>(options =>
            ConfigureDbProvider(options, dbSettings));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit           = true;
            options.Password.RequiredLength         = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase       = true;
            options.Password.RequireLowercase       = true;

            options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers      = true;

            options.User.RequireUniqueEmail        = true;
            options.SignIn.RequireConfirmedEmail    = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath       = "/Admin/Auth/Login";
            options.LogoutPath      = "/Admin/Auth/Logout";
            options.AccessDeniedPath = "/Admin/Auth/AccessDenied";
            options.ExpireTimeSpan  = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

        return services;
    }

    internal static void ConfigureDbProvider(
        DbContextOptionsBuilder options, DatabaseSettings dbSettings)
    {
        var migrationsAssembly = typeof(ApplicationDbContext).Assembly.FullName;

        switch (dbSettings.GetProvider())
        {
            case DatabaseProvider.MySQL:
                var serverVersion = ServerVersion.AutoDetect(dbSettings.ConnectionString);
                options.UseMySql(
                    dbSettings.ConnectionString,
                    serverVersion,
                    b => b.MigrationsAssembly(migrationsAssembly));
                break;

            case DatabaseProvider.PostgreSQL:
                options.UseNpgsql(
                    dbSettings.ConnectionString,
                    b => b.MigrationsAssembly(migrationsAssembly));
                break;

            case DatabaseProvider.SQLite:
                options.UseSqlite(
                    dbSettings.ConnectionString,
                    b => b.MigrationsAssembly(migrationsAssembly));
                break;

            case DatabaseProvider.SqlServer:
            default:
                options.UseSqlServer(
                    dbSettings.ConnectionString,
                    b => b.MigrationsAssembly(migrationsAssembly));
                break;
        }
    }
}
