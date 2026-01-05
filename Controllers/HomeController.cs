using System.Diagnostics;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_LabWebBD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? categoria,
            int? marcaId,
            int? modeloId,
            int? anoMin,
            int? anoMax,
            decimal? precoMin,
            decimal? precoMax,
            int? quilometragemMax,
            int? combustivelId,
            string? caixa,
            string? localizacao,
            string? ordenacao)
        {
            // Query base - apenas anúncios ativos
            var query = _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_CombustivelNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                        .ThenInclude(m => m.ID_MarcaNavigation)
                .Where(a => a.Estado_Anuncio == "Ativo")
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(a => a.ID_CarroNavigation.Categoria == categoria);
            }

            if (marcaId.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.ID_ModeloNavigation.ID_Marca == marcaId.Value);
            }

            if (modeloId.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.ID_Modelo == modeloId.Value);
            }

            if (anoMin.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.Ano >= anoMin.Value);
            }

            if (anoMax.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.Ano <= anoMax.Value);
            }

            if (precoMin.HasValue)
            {
                query = query.Where(a => a.Preco >= precoMin.Value);
            }

            if (precoMax.HasValue)
            {
                query = query.Where(a => a.Preco <= precoMax.Value);
            }

            if (quilometragemMax.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.Quilometragem <= quilometragemMax.Value);
            }

            if (combustivelId.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.ID_Combustivel == combustivelId.Value);
            }

            if (!string.IsNullOrEmpty(caixa))
            {
                query = query.Where(a => a.ID_CarroNavigation.Caixa == caixa);
            }

            if (!string.IsNullOrEmpty(localizacao))
            {
                query = query.Where(a => a.Localizacao != null && a.Localizacao.Contains(localizacao));
            }

            // Aplicar ordenação
            query = ordenacao switch
            {
                "preco_asc" => query.OrderBy(a => a.Preco),
                "preco_desc" => query.OrderByDescending(a => a.Preco),
                "quilometragem_asc" => query.OrderBy(a => a.ID_CarroNavigation.Quilometragem),
                "quilometragem_desc" => query.OrderByDescending(a => a.ID_CarroNavigation.Quilometragem),
                "ano_desc" => query.OrderByDescending(a => a.ID_CarroNavigation.Ano),
                "ano_asc" => query.OrderBy(a => a.ID_CarroNavigation.Ano),
                _ => query.OrderByDescending(a => a.Data_Publicacao) // Mais recentes por padrão
            };

            var anuncios = await query.ToListAsync();

            // Passar dados para dropdowns de filtros
            ViewBag.Marcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync();
            ViewBag.Combustiveis = await _context.Combustivels.OrderBy(c => c.Tipo).ToListAsync();
            ViewBag.Categorias = await _context.Carros
                .Where(c => c.Categoria != null)
                .Select(c => c.Categoria)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
            ViewBag.Caixas = new List<string> { "Manual", "Automática" };

            // Manter valores dos filtros
            ViewBag.FiltroCategoria = categoria;
            ViewBag.FiltroMarcaId = marcaId;
            ViewBag.FiltroModeloId = modeloId;
            ViewBag.FiltroAnoMin = anoMin;
            ViewBag.FiltroAnoMax = anoMax;
            ViewBag.FiltroPrecoMin = precoMin;
            ViewBag.FiltroPrecoMax = precoMax;
            ViewBag.FiltroQuilometragemMax = quilometragemMax;
            ViewBag.FiltroCombustivelId = combustivelId;
            ViewBag.FiltroCaixa = caixa;
            ViewBag.FiltroLocalizacao = localizacao;
            ViewBag.Ordenacao = ordenacao;

            return View(anuncios);
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
