using System.Diagnostics;
using Marketplace_LabWebBD.Models;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace_LabWebBD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Redirecionar a landing page diretamente para o catálogo de veículos
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Anuncios");
        }

        public IActionResult Privacy()
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
