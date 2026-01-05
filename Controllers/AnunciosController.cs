using Microsoft.AspNetCore.Mvc;
using Marketplace_LabWebBD.Services;
using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Controllers
{
    public class AnunciosController : Controller
    {
        private readonly ISearchService _searchService;
        private readonly IAnuncioService _anuncioService;

        public AnunciosController(ISearchService searchService, IAnuncioService anuncioService)
        {
            _searchService = searchService;
            _anuncioService = anuncioService;
        }

        // GET: /Anuncios (Catálogo público)
        [HttpGet]
        public async Task<IActionResult> Index(SearchViewModel filters, int page = 1)
        {
            // Load dropdowns
            filters.Marcas = await _anuncioService.GetMarcasAsync();
            filters.Marcas.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "", Text = "Todas as marcas" });

            filters.Combustiveis = await _anuncioService.GetCombustiveisAsync();
            filters.Combustiveis.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "", Text = "Todos os combustíveis" });

            filters.Cores = _anuncioService.GetCores();
            filters.Cores.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "", Text = "Todas as cores" });

            // Load modelos if marca is selected
            if (filters.ID_Marca.HasValue)
            {
                filters.Modelos = await _anuncioService.GetModelosByMarcaAsync(filters.ID_Marca.Value);
                filters.Modelos.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "", Text = "Todos os modelos" });
            }
            else
            {
                filters.Modelos.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "", Text = "Selecione a marca primeiro" });
            }

            // Perform search
            var result = await _searchService.SearchAnunciosAsync(filters, page, 12);

            return View(result);
        }

        // GET: /Anuncios/Detalhes/5
        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            var anuncio = await _searchService.GetAnuncioDetailsAsync(id);

            if (anuncio == null)
            {
                TempData["ErrorMessage"] = "Anúncio não encontrado.";
                return RedirectToAction("Index");
            }

            // Check if anuncio is active
            if (anuncio.Estado_Anuncio != "Ativo")
            {
                TempData["InfoMessage"] = "Este anúncio não está mais disponível.";
                return RedirectToAction("Index");
            }

            return View(anuncio);
        }

        // GET: /Anuncios/GetModelosByMarca (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetModelosByMarca(int marcaId)
        {
            var modelos = await _anuncioService.GetModelosByMarcaAsync(marcaId);
            return Json(modelos);
        }
    }
}
