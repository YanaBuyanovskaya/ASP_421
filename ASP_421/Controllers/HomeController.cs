using ASP_421.Infasctructure;
using ASP_421.Models;
using ASP_421.Services.KDF;
using ASP_421.Services.Random;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_421.Controllers
{
    [ServiceFilter(typeof(VisitLogger))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IRandomService _randomService;

        private readonly IKDFService _kdfService;

        private readonly TimeStampService _timeStampService;
      

        public HomeController(ILogger<HomeController> logger, IRandomService randomService, IKDFService kdfService, TimeStampService timeStampService)
        {
            _logger = logger;
            _randomService = randomService;
            _kdfService = kdfService;
            _timeStampService = timeStampService;

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
            ViewData["otp"] = _kdfService.DK("Admin", "B81D9191-7040-41A3-BFBD-6AF1FFB4A266");
            return View();
        }
        public IActionResult TimeStamp()
        {
            ViewBag.Seconds = _timeStampService.TimeStampSeconds();
            ViewBag.Milliseconds = _timeStampService.TimeStampMilliSeconds();
            ViewBag.Epocha = _timeStampService.EpochTime();
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
