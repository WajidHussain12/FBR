using System.ComponentModel.DataAnnotations;

namespace FBR_DI.Web.Areas.Admin.Models;

public class DatabaseSetupViewModel
{
    [Required]
    public string Provider { get; set; } = "SqlServer";

    [Required(ErrorMessage = "Connection string is required.")]
    public string ConnectionString { get; set; } = string.Empty;
}

public class ConnectionTestResult
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
}

// ── Settings page ────────────────────────────────────────────────────────────

public class SettingsViewModel
{
    public bool   IsMockMode            { get; set; }
    public string DbProvider            { get; set; } = "SqlServer";
    public string MaskedConnectionString { get; set; } = string.Empty;
    public string AppEnvironment        { get; set; } = "Development";
    public string LogsPath              { get; set; } = "Logs/";
    public string AppVersion            { get; set; } = "1.0.0";
}

public class SwitchToRealApiRequest
{
    public string BearerTokenTest { get; set; } = string.Empty;
}
