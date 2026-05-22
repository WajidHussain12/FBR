using FBR_DI.Persistence.IdentityModels;
using FBR_DI.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBR_DI.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToReturnUrlOrDashboard(returnUrl);

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return RedirectToReturnUrlOrDashboard(returnUrl);

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Account is locked. Try again after 15 minutes.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        var loginUrl = $"{GetEffectivePathBase()}/Admin/Auth/Login";
        _logger.LogInformation("[Auth] Logout redirect — FinalRedirectUrl='{FinalRedirectUrl}'", loginUrl);
        return Redirect(loginUrl);
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Register() => View();

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, model.Role);
            TempData["Success"] = $"User '{model.Email}' registered successfully.";
            return RedirectToAction("Index", "UserManagement");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    private string GetEffectivePathBase()
    {
        var requestPathBase = Request.PathBase.HasValue ? Request.PathBase.Value : string.Empty;
        var configuredPathBase = _configuration["PathBase"] ?? string.Empty;

        var pathBase = !string.IsNullOrWhiteSpace(requestPathBase)
            ? requestPathBase
            : configuredPathBase;

        if (string.IsNullOrWhiteSpace(pathBase))
            return string.Empty;

        pathBase = pathBase.Trim();
        pathBase = pathBase.StartsWith('/') ? pathBase : "/" + pathBase;
        return pathBase.TrimEnd('/');
    }

    private IActionResult RedirectToReturnUrlOrDashboard(string? returnUrl)
    {
        var configuredPathBase = _configuration["PathBase"] ?? string.Empty;
        var pathBase = GetEffectivePathBase();
        var finalRedirectUrl = BuildReturnUrlOrDashboard(returnUrl, pathBase);

        _logger.LogInformation(
            "[Auth] Redirect decision — Request.PathBase='{RequestPathBase}' Configured.PathBase='{ConfiguredPathBase}' Effective.PathBase='{EffectivePathBase}' ReturnUrl='{ReturnUrl}' IsAuthenticated={IsAuthenticated} FinalRedirectUrl='{FinalRedirectUrl}'",
            Request.PathBase.HasValue ? Request.PathBase.Value : string.Empty,
            configuredPathBase,
            pathBase,
            returnUrl,
            User.Identity?.IsAuthenticated ?? false,
            finalRedirectUrl);

        return Redirect(finalRedirectUrl);
    }

    private string BuildReturnUrlOrDashboard(string? returnUrl, string pathBase)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            if (!string.IsNullOrWhiteSpace(pathBase))
            {
                if (returnUrl.StartsWith(pathBase + "/", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(returnUrl, pathBase, StringComparison.OrdinalIgnoreCase))
                {
                    return returnUrl;
                }

                if (returnUrl.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return pathBase + returnUrl;
                }
            }

            return returnUrl;
        }

        return $"{pathBase}/Admin/Dashboard/Index";
    }
}
