using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaPrestamos.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace SistemaPrestamos.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize] // Asegura que el usuario esté autenticado
        public IActionResult SecuredPage()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
