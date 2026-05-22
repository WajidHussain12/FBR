using FBR_DI.Application.Features.Admin.Queries.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBR_DI.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class DashboardController : Controller
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _sender.Send(new GetDashboardDataQuery());
        if (result.IsFailure)
            return View("Error");

        return View(result.Data);
    }
}
