using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.Services;
using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Controllers
{
    [Authorize(Roles = "Comprador")]
    public class CompradorController : Controller
    {
        private readonly ISearchService _searchService;
        private readonly IAnuncioService _anuncioService;
        private readonly IReservaService _reservaService;
        private readonly IVisitaService _visitaService;
        private readonly ICompraService _compraService;
        private readonly IPromocaoAdminService _promocaoAdminService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CompradorController(
            ISearchService searchService,
            IAnuncioService anuncioService,
            IReservaService reservaService,
            IVisitaService visitaService,
            ICompraService compraService,
            IPromocaoAdminService promocaoAdminService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _searchService = searchService;
            _anuncioService = anuncioService;
            _reservaService = reservaService;
            _visitaService = visitaService;
            _compraService = compraService;
            _promocaoAdminService = promocaoAdminService;
            _userManager = userManager;
            _context = context;
        }

        // GET: /Comprador/Index (Dashboard)
        public async Task<IActionResult> Index()
        {
            // Get recent and featured anuncios for dashboard
            var model = new BuyerDashboardViewModel
            {
                AnunciosRecentes = await _searchService.GetRecentAnunciosAsync(6),
                AnunciosDestaque = await _searchService.GetFeaturedAnunciosAsync(6)
            };

            return View(model);
        }

        // GET: /Comprador/Search
        [HttpGet]
        public async Task<IActionResult> Search(SearchViewModel filters, int page = 1)
        {
            // Load dropdowns
            filters.Marcas = await _anuncioService.GetMarcasAsync();
            filters.Marcas.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "", Text = "Todas as marcas" });

            filters.Combustiveis = await _anuncioService.GetCombustiveisAsync();
            filters.Combustiveis.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "", Text = "Todos os combustíveis" });

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

        // GET: /Comprador/AnuncioDetails/5
        [HttpGet]
        public async Task<IActionResult> AnuncioDetails(int id)
        {
            var anuncio = await _searchService.GetAnuncioDetailsAsync(id);

            if (anuncio == null)
            {
                TempData["ErrorMessage"] = "Anúncio não encontrado.";
                return RedirectToAction("Search");
            }

            // Check if anuncio is active
            if (anuncio.Estado_Anuncio != "Ativo")
            {
                TempData["InfoMessage"] = "Este anúncio não está mais disponível.";
                return RedirectToAction("Search");
            }

            return View(anuncio);
        }

        // GET: /Comprador/GetModelosByMarca (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetModelosByMarca(int marcaId)
        {
            var modelos = await _anuncioService.GetModelosByMarcaAsync(marcaId);
            return Json(modelos);
        }

        // GET: /Comprador/ReservarVeiculo/5
        [HttpGet]
        public async Task<IActionResult> ReservarVeiculo(int id)
        {
            var canReserve = await _reservaService.CanReserveAsync(id);

            if (!canReserve)
            {
                TempData["ErrorMessage"] = "Este veículo não está disponível para reserva.";
                return RedirectToAction("AnuncioDetails", new { id });
            }

            var model = await _reservaService.GetReservaDetailsAsync(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Anúncio não encontrado.";
                return RedirectToAction("Search");
            }

            return View(model);
        }

        // POST: /Comprador/ReservarVeiculo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservarVeiculo(int id, string confirm)
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = _context.Compradors.FirstOrDefault(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _reservaService.CreateReservaAsync(id, comprador.ID_Comprador);

            if (success)
            {
                TempData["SuccessMessage"] = "Veículo reservado com sucesso! A reserva expira na data indicada.";
                return RedirectToAction("MinhasReservas");
            }

            TempData["ErrorMessage"] = "Não foi possível reservar o veículo. Pode já ter sido reservado por outro utilizador.";
            return RedirectToAction("AnuncioDetails", new { id });
        }

        // GET: /Comprador/MinhasReservas
        [HttpGet]
        public async Task<IActionResult> MinhasReservas()
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = _context.Compradors.FirstOrDefault(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var reservas = await _reservaService.GetReservasByCompradorAsync(comprador.ID_Comprador);

            return View(reservas);
        }

        // POST: /Comprador/CancelarReserva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = _context.Compradors.FirstOrDefault(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _reservaService.CancelReservaAsync(id, comprador.ID_Comprador);

            if (success)
            {
                TempData["SuccessMessage"] = "Reserva cancelada com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível cancelar a reserva.";
            }

            return RedirectToAction("MinhasReservas");
        }

        // ========== VISITAS ==========

        // GET: /Comprador/AgendarVisita/5
        [HttpGet]
        public async Task<IActionResult> AgendarVisita(int id)
        {
            var model = await _visitaService.GetVisitaDetailsAsync(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Veículo não disponível para agendamento de visitas.";
                return RedirectToAction("Search");
            }

            return View(model);
        }

        // POST: /Comprador/AgendarVisita/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgendarVisita(CreateVisitaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _visitaService.CreateVisitaAsync(model, comprador.ID_Comprador);

            if (success)
            {
                TempData["SuccessMessage"] = "Visita agendada com sucesso!";
                return RedirectToAction("MinhasVisitas");
            }

            TempData["ErrorMessage"] = "Não foi possível agendar a visita. Verifique se a data é futura.";
            return View(model);
        }

        // GET: /Comprador/MinhasVisitas
        [HttpGet]
        public async Task<IActionResult> MinhasVisitas()
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var visitas = await _visitaService.GetVisitasByCompradorAsync(comprador.ID_Comprador);

            return View(visitas);
        }

        // POST: /Comprador/CancelarVisita/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarVisita(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var success = await _visitaService.CancelVisitaAsync(id, user!.Id, "Comprador");

            if (success)
            {
                TempData["SuccessMessage"] = "Visita cancelada com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível cancelar a visita.";
            }

            return RedirectToAction("MinhasVisitas");
        }

        // ========== COMPRAS ==========

        // GET: /Comprador/Checkout/5
        [HttpGet]
        public async Task<IActionResult> Checkout(int id)
        {
            var model = await _compraService.GetCheckoutDetailsAsync(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Veículo não disponível para compra.";
                return RedirectToAction("Search");
            }

            return View(model);
        }

        // POST: /Comprador/Checkout/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var compraId = await _compraService.CreateCompraAsync(model, comprador.ID_Comprador);

            if (compraId.HasValue)
            {
                TempData["SuccessMessage"] = "Compra realizada com sucesso! Aguarde a confirmação de pagamento.";
                return RedirectToAction("OrderConfirmation", new { id = compraId.Value });
            }

            TempData["ErrorMessage"] = "Não foi possível concluir a compra. O veículo pode já ter sido vendido.";
            return View(model);
        }

        // GET: /Comprador/OrderConfirmation/5
        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var model = await _compraService.GetOrderConfirmationAsync(id, comprador.ID_Comprador);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Compra não encontrada.";
                return RedirectToAction("MinhasCompras");
            }

            return View(model);
        }

        // POST: /Comprador/SimulatePayment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SimulatePayment(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _compraService.SimulatePaymentAsync(id, comprador.ID_Comprador);

            if (success)
            {
                TempData["SuccessMessage"] = "Pagamento simulado com sucesso! A compra foi marcada como paga.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível simular o pagamento.";
            }

            return RedirectToAction("MinhasCompras");
        }

        // GET: /Comprador/MinhasCompras
        [HttpGet]
        public async Task<IActionResult> MinhasCompras()
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var compras = await _compraService.GetComprasByCompradorAsync(comprador.ID_Comprador);

            return View(compras);
        }

        // POST: /Comprador/CancelarCompra/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarCompra(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _compraService.CancelCompraAsync(id, comprador.ID_Comprador);

            if (success)
            {
                TempData["SuccessMessage"] = "Compra cancelada com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível cancelar a compra. Apenas compras pendentes podem ser canceladas.";
            }

            return RedirectToAction("MinhasCompras");
        }

        // GET: /Comprador/SolicitarPromocao
        public async Task<IActionResult> SolicitarPromocao()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index");

            // Verificar se já tem pedido pendente
            var hasPedidoPendente = await _promocaoAdminService.HasPedidoPendenteAsync(user.Id);
            if (hasPedidoPendente)
            {
                TempData["ErrorMessage"] = "Já tem um pedido de promoção pendente.";
                return RedirectToAction("Index");
            }

            // Verificar se já é admin
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                TempData["ErrorMessage"] = "Já é administrador.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // POST: /Comprador/SolicitarPromocao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarPromocao(PedidoPromocaoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utilizador não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _promocaoAdminService.CreatePedidoAsync(user.Id, "Comprador", model.Justificacao);

            if (success)
            {
                TempData["SuccessMessage"] = "Pedido de promoção enviado com sucesso! Aguarde a aprovação do administrador.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível enviar o pedido de promoção.";
            }

            return RedirectToAction("Index");
        }
    }
}
