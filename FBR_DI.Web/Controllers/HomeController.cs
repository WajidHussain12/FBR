using FBR_DI.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FBR_DI.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        public IActionResult Privacy()
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogWarning("[Home] Error page rendered — RequestId='{RequestId}' PathBase='{PathBase}' Path='{Path}'",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Request.PathBase.HasValue ? Request.PathBase.Value : string.Empty,
                Request.Path.Value ?? string.Empty);

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
