using FBR_DI.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Hosting;

namespace FBR_DI.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SetupController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SetupController> _logger;

    public SetupController(
        IWebHostEnvironment env,
        IConfiguration configuration,
        ILogger<SetupController> logger)
    {
        _env           = env;
        _configuration = configuration;
        _logger        = logger;
    }

    [HttpGet]
    public IActionResult Database()
    {
        var model = new DatabaseSetupViewModel
        {
            Provider         = _configuration["Database:Provider"] ?? "SqlServer",
            ConnectionString = _configuration["Database:ConnectionString"]
                               ?? _configuration.GetConnectionString("DefaultConnection")
                               ?? string.Empty
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Database(DatabaseSetupViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var testResult = await TestConnectionInternalAsync(model.Provider, model.ConnectionString);
        if (!testResult.Success)
        {
            ModelState.AddModelError(string.Empty, testResult.Error);
            return View(model);
        }

        await WriteSettingsAsync(model.Provider, model.ConnectionString);

        TempData["Success"] = "Database configuration saved successfully. Restart the application to apply changes.";
        return RedirectToAction(nameof(Database));
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> TestConnection([FromBody] DatabaseSetupViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Provider) ||
            string.IsNullOrWhiteSpace(model.ConnectionString))
            return Json(new ConnectionTestResult { Success = false, Error = "Provider and connection string are required." });

        var result = await TestConnectionInternalAsync(model.Provider, model.ConnectionString);
        return Json(result);
    }

    // ── GET: /Admin/Setup/Ping ───────────────────────────────────────────────
    /// <summary>
    /// Lightweight liveness probe used by the client-side restart poller.
    /// Returns 200 as soon as the app is back up after a restart.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Ping() => Ok("pong");

    // ── POST: /Admin/Setup/RestartApp ────────────────────────────────────────
    /// <summary>
    /// Gracefully stops the application process. The host process manager
    /// (IIS App Pool, systemd Restart=always, Docker restart policy, PM2)
    /// is responsible for bringing it back up automatically.
    /// A 2-second delay lets the HTTP response flush before shutdown begins.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RestartApp([FromServices] IHostApplicationLifetime lifetime)
    {
        _logger.LogWarning("Application restart requested by user {User}.", User.Identity?.Name);

        // Fire-and-forget: wait for response to flush, then trigger graceful stop.
        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            lifetime.StopApplication();
        });

        return Json(new { success = true });
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static async Task<ConnectionTestResult> TestConnectionInternalAsync(
        string provider, string connectionString)
    {
        try
        {
            switch (provider.Trim().ToLower())
            {
                case "sqlserver" or "mssql":
                    await using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
                        await conn.OpenAsync();
                    break;

                case "mysql" or "mariadb":
                    await using (var conn = new MySqlConnector.MySqlConnection(connectionString))
                        await conn.OpenAsync();
                    break;

                case "postgresql" or "postgres" or "pgsql":
                    await using (var conn = new Npgsql.NpgsqlConnection(connectionString))
                        await conn.OpenAsync();
                    break;

                case "sqlite":
                    if (!connectionString.Contains("Data Source", StringComparison.OrdinalIgnoreCase))
                        return new ConnectionTestResult
                        {
                            Success = false,
                            Error   = "SQLite connection string must contain 'Data Source=path/to/file.db'."
                        };
                    // SQLite file is created on demand — no open test needed
                    break;

                default:
                    return new ConnectionTestResult
                    {
                        Success = false,
                        Error   = $"Unknown provider '{provider}'."
                    };
            }

            return new ConnectionTestResult { Success = true };
        }
        catch (Exception ex)
        {
            return new ConnectionTestResult { Success = false, Error = ex.Message };
        }
    }

    private async Task WriteSettingsAsync(string provider, string connectionString)
    {
        var path = Path.Combine(_env.ContentRootPath, "appsettings.json");
        var raw  = await System.IO.File.ReadAllTextAsync(path);
        var root = JsonNode.Parse(raw)!.AsObject();

        root["Database"] = JsonNode.Parse(JsonSerializer.Serialize(new
        {
            Provider         = provider,
            ConnectionString = connectionString
        }));

        // Also keep legacy ConnectionStrings in sync
        var legacyCs = root["ConnectionStrings"]?.AsObject();
        if (legacyCs is not null)
            legacyCs["DefaultConnection"] = JsonValue.Create(connectionString);

        var options = new JsonSerializerOptions { WriteIndented = true };
        await System.IO.File.WriteAllTextAsync(path, root.ToJsonString(options));
    }
}
