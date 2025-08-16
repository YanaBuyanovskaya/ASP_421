using ASP_421.Models;
using ASP_421.Services.Random;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_421.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IRandomService _randomService;
        public HomeController(ILogger<HomeController> logger, IRandomService randomService)
        {
            _logger = logger;
            _randomService = randomService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Razor()
        {
            return View();
        }

        public IActionResult IoC()
        {
            ViewData["otp"] = _randomService.Otp(6);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
