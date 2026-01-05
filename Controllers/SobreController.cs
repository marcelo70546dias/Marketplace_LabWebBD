using Marketplace_LabWebBD.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_LabWebBD.Controllers
{
    public class SobreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SobreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Página "Sobre Nós" com informações do marketplace
        public async Task<IActionResult> Index()
        {
            var config = await _context.Configuracao_Sistemas.FirstOrDefaultAsync();
            return View(config);
        }

        // Página de Termos de Uso
        public async Task<IActionResult> Termos()
        {
            var config = await _context.Configuracao_Sistemas.FirstOrDefaultAsync();
            return View(config);
        }

        // Página de Política de Privacidade
        public async Task<IActionResult> Privacidade()
        {
            var config = await _context.Configuracao_Sistemas.FirstOrDefaultAsync();
            return View(config);
        }

        // Página de Contactos
        public async Task<IActionResult> Contactos()
        {
            var config = await _context.Configuracao_Sistemas.FirstOrDefaultAsync();
            return View(config);
        }
    }
}
