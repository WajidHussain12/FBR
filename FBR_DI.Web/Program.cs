using FBR_DI.Application;
using FBR_DI.Infrastructure;
using FBR_DI.Persistence;
using FBR_DI.Persistence.Context;
using FBR_DI.Persistence.Seed;
using FBR_DI.Web.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// ── Application services ─────────────────────────────────────────────────────
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── MVC + Razor runtime compilation (dev only) ───────────────────────────────
var mvcBuilder = builder.Services.AddControllersWithViews();
if (builder.Environment.IsDevelopment())
    mvcBuilder.AddRazorRuntimeCompilation();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// ── Build pipeline ────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Database initialization: check → create → seed ───────────────────────────
await InitializeDatabaseAsync(app);

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ── Routes ────────────────────────────────────────────────────────────────────
app.MapAreaControllerRoute(
    name:     "AdminArea",
    areaName: "Admin",
    pattern:  "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ─────────────────────────────────────────────────────────────────────────────
static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger   = services.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    logger.LogInformation("[DB Init] Starting database initialization...");

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var provider = context.Database.ProviderName ?? "Unknown";

        // ── Step 1: Check database connectivity ───────────────────────────────
        logger.LogInformation("[DB Init] Step 1 — Checking database connection (Provider: {Provider})...", provider);

        var canConnect = false;
        try { canConnect = await context.Database.CanConnectAsync(); }
        catch { /* will be created in step 2 */ }

        if (canConnect)
            logger.LogInformation("[DB Init] Step 1 — Database connection OK.");
        else
            logger.LogWarning("[DB Init] Step 1 — Database not reachable. Will attempt to create in next step.");

        // ── Step 2: Create database + all tables ──────────────────────────────
        logger.LogInformation("[DB Init] Step 2 — Checking database schema (tables)...");

        var isSqlServer = provider.Contains("SqlServer", StringComparison.OrdinalIgnoreCase);

        if (isSqlServer)
        {
            // SQL Server: full migration history support
            var pending = (await context.Database.GetPendingMigrationsAsync()).ToList();
            if (pending.Count > 0)
            {
                logger.LogInformation("[DB Init] Step 2 — Applying {Count} pending migration(s): {Names}",
                    pending.Count, string.Join(", ", pending));
                await context.Database.MigrateAsync();
                logger.LogInformation("[DB Init] Step 2 — All migrations applied. Tables created/updated.");
            }
            else
            {
                await context.Database.MigrateAsync();
                logger.LogInformation("[DB Init] Step 2 — Schema is up-to-date. No new migrations needed.");
            }
        }
        else
        {
            // MySQL / PostgreSQL / SQLite:
            // Migration files were generated for SQL Server (nvarchar, datetime2, bit, SqlServer:Identity)
            // which MySQL rejects. We use EnsureCreated() instead — it builds the schema directly
            // from the current C# model using the correct MySQL types.
            //
            // But EnsureCreated() skips table creation if it finds ANY existing table in the database
            // (including __EFMigrationsHistory left behind by a failed MigrateAsync attempt).
            // So we first drop that history table to give EnsureCreated a clean slate.
            try
            {
                await context.Database.ExecuteSqlRawAsync(
                    "DROP TABLE IF EXISTS `__EFMigrationsHistory`");
                logger.LogInformation("[DB Init] Step 2 — Cleared migration history (if any).");
            }
            catch { /* database may not exist yet — that's fine */ }

            var created = await context.Database.EnsureCreatedAsync();
            if (created)
                logger.LogInformation("[DB Init] Step 2 — Database and all tables created successfully.");
            else
                logger.LogInformation("[DB Init] Step 2 — Database and tables already exist. No changes needed.");
        }

        // ── Step 3: Check and seed required data ──────────────────────────────
        logger.LogInformation("[DB Init] Step 3 — Checking required data (roles, admin user, reference data)...");
        await ApplicationDbSeeder.SeedAsync(services);
        logger.LogInformation("[DB Init] Step 3 — Seed data check complete.");

        logger.LogInformation("[DB Init] Database initialization finished successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[DB Init] Database initialization failed. Application may not work correctly.");
    }

    logger.LogInformation("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
}
