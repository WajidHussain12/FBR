/*
 * ══════════════════════════════════════════════════════════════════════════════
 * HOW TO SWITCH FROM MOCK FBR API TO THE REAL PRAL API
 * ══════════════════════════════════════════════════════════════════════════════
 *
 * STEP 1 — Receive your PRAL Bearer Token from FBR.
 *
 * STEP 2 — Navigate to Admin → Settings.
 *           In the "FBR API Configuration" section, paste your token into
 *           the "Test Token" field and click "Switch to Live API".
 *           The system will call the FBR Provinces endpoint to validate the
 *           token. On success, appsettings.json is updated automatically:
 *             "FbrApi": { "UseMockService": false }
 *
 * STEP 3 — Navigate to Admin → Companies → [Your Company] → FBR Settings.
 *           Enter the same PRAL Bearer Token for that specific company.
 *           Choose Environment: Sandbox (testing) or Production (live).
 *           Click "Update Token".
 *
 * STEP 4 — Still in FBR Settings, click "Sync All Reference Data".
 *           This downloads REAL data from FBR and populates the database:
 *             • Provinces, UOM codes, HS Codes, Transaction Types, Tax Rates.
 *
 * STEP 5 — Admin → Invoices → Create Invoice.
 *           Dropdowns now show real FBR data.
 *           Submit hits the REAL FBR API and returns a genuine invoice number.
 *           A QR code is generated from that invoice number.
 *
 * TO REVERT TO MOCK:  Admin → Settings → Switch to Mock Mode.
 * ══════════════════════════════════════════════════════════════════════════════
 */

using FBR_DI.Application.Interfaces;
using FBR_DI.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FBR_DI.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    // Real FBR sandbox provinces endpoint — lightweight, no side effects.
    // Using sandbox (_sb suffix) so the validation works before going live.
    private const string FbrProvincesTestUrl =
        "https://gw.fbr.gov.pk/pdi/v1/GetProvinces_sb";

    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration      _configuration;
    private readonly IApplicationDbContext _context;
    private readonly IHttpClientFactory  _httpClientFactory;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        IWebHostEnvironment       env,
        IConfiguration            configuration,
        IApplicationDbContext     context,
        IHttpClientFactory        httpClientFactory,
        ILogger<SettingsController> logger)
    {
        _env               = env;
        _configuration     = configuration;
        _context           = context;
        _httpClientFactory = httpClientFactory;
        _logger            = logger;
    }

    // ── GET: /Admin/Settings ─────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Index()
    {
        var rawCs = _configuration["Database:ConnectionString"]
                    ?? _configuration.GetConnectionString("DefaultConnection")
                    ?? string.Empty;

        var model = new SettingsViewModel
        {
            IsMockMode             = _configuration.GetValue<bool>("FbrApi:UseMockService", true),
            DbProvider             = _configuration["Database:Provider"] ?? "SqlServer",
            MaskedConnectionString = MaskConnectionString(rawCs),
            AppEnvironment         = _env.EnvironmentName,
            LogsPath               = Path.Combine(_env.ContentRootPath, "Logs"),
            AppVersion             = "1.0.0"
        };

        return View(model);
    }

    // ── POST: /Admin/Settings/SwitchToRealApi ────────────────────────────────
    /// <summary>
    /// Validates the supplied PRAL Bearer Token against the FBR sandbox
    /// Provinces endpoint. If valid, writes UseMockService=false to
    /// appsettings.json so the next request uses the real FbrApiService.
    /// </summary>
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SwitchToRealApi(
        [FromBody] SwitchToRealApiRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BearerTokenTest))
            return Json(new { success = false, message = "Bearer token is required." });

        _logger.LogInformation("Attempting to validate PRAL token and switch to Live FBR API.");

        // ── Validate token against a live FBR endpoint ──────────────────────
        var (ok, errorMsg) = await ValidateTokenAsync(request.BearerTokenTest);
        if (!ok)
        {
            _logger.LogWarning("PRAL token validation failed: {Error}", errorMsg);
            return Json(new { success = false, message = $"Token validation failed: {errorMsg}" });
        }

        // ── Write to appsettings.json ────────────────────────────────────────
        try
        {
            await PatchAppSettingsAsync("FbrApi", "UseMockService", false);
            _logger.LogInformation("Switched to Live FBR API. UseMockService set to false.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write appsettings.json");
            return Json(new { success = false, message = "Token valid, but could not update appsettings.json. Set FbrApi:UseMockService=false manually." });
        }

        return Json(new
        {
            success = true,
            message = "Token validated successfully. Switched to Live FBR API. Restart the application to apply the change."
        });
    }

    // ── POST: /Admin/Settings/SwitchToMockApi ────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchToMockApi()
    {
        try
        {
            await PatchAppSettingsAsync("FbrApi", "UseMockService", true);
            _logger.LogInformation("Switched to Mock FBR API. UseMockService set to true.");
            return Json(new { success = true, message = "Switched to Mock FBR API. Restart the application to apply." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to switch to mock API");
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ── GET: /Admin/Settings/ApiStatus ───────────────────────────────────────
    /// <summary>
    /// Quick health check. In Mock mode, always returns connected.
    /// In Live mode, calls GetProvincesAsync with the first integrated
    /// company's token and reports the result.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ApiStatus(
        [FromServices] IFbrApiService fbrApiService)
    {
        var isMock = _configuration.GetValue<bool>("FbrApi:UseMockService", true);
        var now    = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        if (isMock)
        {
            return Json(new
            {
                isConnected = true,
                mode        = "Mock",
                message     = "Mock service — always connected. No real FBR calls are made.",
                lastChecked = now
            });
        }

        // Live mode — probe with first integrated company's token
        var company = await _context.Companies
            .AsNoTracking()
            .Where(c => c.IsIntegrated && !string.IsNullOrEmpty(c.FbrBearerToken))
            .FirstOrDefaultAsync();

        if (company is null)
        {
            return Json(new
            {
                isConnected = false,
                mode        = "Live",
                message     = "No integrated company found. Add a PRAL Bearer Token via Companies → FBR Settings.",
                lastChecked = now
            });
        }

        var result = await fbrApiService.GetProvincesAsync(
            company.FbrBearerToken!, CancellationToken.None);

        return Json(new
        {
            isConnected = result.IsSuccess,
            mode        = "Live",
            message     = result.IsSuccess
                              ? $"FBR API reachable. Tested with company: {company.CompanyName}."
                              : result.Message ?? "FBR API returned an error.",
            lastChecked = now
        });
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Makes a direct HTTP GET to the FBR sandbox provinces endpoint using
    /// the supplied bearer token. Returns (true, "") on success.
    /// </summary>
    private async Task<(bool Success, string Error)> ValidateTokenAsync(string bearerToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await client.GetAsync(FbrProvincesTestUrl);

            if (response.IsSuccessStatusCode)
                return (true, string.Empty);

            return (false, $"FBR returned HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
        }
        catch (HttpRequestException ex)
        {
            return (false, $"Network error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return (false, "Request timed out. Check network connectivity to FBR servers.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    /// <summary>
    /// Patches a single key inside a JSON section of appsettings.json.
    /// E.g. PatchAppSettingsAsync("FbrApi", "UseMockService", false)
    /// sets { "FbrApi": { "UseMockService": false } }.
    /// </summary>
    private async Task PatchAppSettingsAsync(string section, string key, object value)
    {
        var path = Path.Combine(_env.ContentRootPath, "appsettings.json");
        var raw  = await System.IO.File.ReadAllTextAsync(path);
        var root = JsonNode.Parse(raw)!.AsObject();

        var sectionNode = root[section]?.AsObject() ?? new JsonObject();
        sectionNode[key] = JsonValue.Create(value);
        root[section]    = sectionNode;

        var opts = new JsonSerializerOptions { WriteIndented = true };
        await System.IO.File.WriteAllTextAsync(path, root.ToJsonString(opts));
    }

    /// <summary>
    /// Masks a connection string for display — shows only the Server/Host
    /// and Database name; replaces Password with *****.
    /// </summary>
    private static string MaskConnectionString(string cs)
    {
        if (string.IsNullOrWhiteSpace(cs)) return "(not configured)";

        // Replace password/pwd values with *****
        var masked = System.Text.RegularExpressions.Regex.Replace(
            cs,
            @"(?i)(password|pwd|Password)=[^;]+",
            "$1=*****");

        // Truncate if still long
        return masked.Length > 100 ? masked[..100] + "…" : masked;
    }
}
