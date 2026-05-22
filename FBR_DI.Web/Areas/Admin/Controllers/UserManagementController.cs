using FBR_DI.Persistence.IdentityModels;
using FBR_DI.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FBR_DI.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UserManagementController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UserManagementController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var userVms = new List<UserListViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userVms.Add(new UserListViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "None",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        }

        return View(userVms);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        return View(new CreateUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(model);
        }

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
            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var model = new EditUserViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Role = roles.FirstOrDefault() ?? string.Empty,
            IsActive = user.IsActive
        };

        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name", model.Role);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.IsActive = model.IsActive;

        await _userManager.UpdateAsync(user);

        // Update role
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, model.Role);

        TempData["Success"] = "User updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return Json(new { success = false });

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        return Json(new { success = true, isActive = user.IsActive });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return Json(new { success = false });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return Json(new { success = result.Succeeded, message = result.Succeeded
            ? "Password reset successfully."
            : string.Join(", ", result.Errors.Select(e => e.Description)) });
    }
}
