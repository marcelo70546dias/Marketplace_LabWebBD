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
        private readonly IFiltroFavoritoService _filtroFavoritoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CompradorController(
            ISearchService searchService,
            IAnuncioService anuncioService,
            IReservaService reservaService,
            IVisitaService visitaService,
            ICompraService compraService,
            IPromocaoAdminService promocaoAdminService,
            IFiltroFavoritoService filtroFavoritoService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _searchService = searchService;
            _anuncioService = anuncioService;
            _reservaService = reservaService;
            _visitaService = visitaService;
            _compraService = compraService;
            _promocaoAdminService = promocaoAdminService;
            _filtroFavoritoService = filtroFavoritoService;
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

            // Check if anuncio is available (Ativo ou Reservado)
            if (anuncio.Estado_Anuncio != "Ativo" && anuncio.Estado_Anuncio != "Reservado")
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
            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var canReserve = await _reservaService.CanReserveAsync(id, comprador.ID_Comprador);

            if (!canReserve)
            {
                TempData["ErrorMessage"] = "Este veículo não está disponível para reserva ou já tem um pedido de reserva pendente/aprovado.";
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
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var result = await _reservaService.CreateReservaAsync(id, comprador.ID_Comprador);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Pedido de reserva enviado com sucesso! O vendedor será notificado e deverá aprovar o seu pedido.";
                return RedirectToAction("MinhasReservas");
            }

            TempData["ErrorMessage"] = $"Não foi possível enviar o pedido de reserva: {result.ErrorMessage}";
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

            var success = await _reservaService.CancelReservaAsync(id, user!.Id, "Comprador");

            if (success)
            {
                TempData["SuccessMessage"] = "Reserva/pedido cancelado com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível cancelar a reserva/pedido.";
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

        // POST: /Comprador/AgendarVisita
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgendarVisita(CreateVisitaViewModel model)
        {
            // Validar data no servidor
            if (model.Data_Hora <= DateTime.Now.AddMinutes(30))
            {
                ModelState.AddModelError("Data_Hora", "A data deve ser pelo menos 30 minutos no futuro.");
            }

            if (!ModelState.IsValid)
            {
                // Recarregar dados do anúncio se o modelo falhou
                if (model.ID_Anuncio > 0)
                {
                    var anuncioDetails = await _visitaService.GetVisitaDetailsAsync(model.ID_Anuncio);
                    if (anuncioDetails != null)
                    {
                        model.Titulo = anuncioDetails.Titulo;
                        model.MarcaModelo = anuncioDetails.MarcaModelo;
                        model.Preco = anuncioDetails.Preco;
                        model.FotoPrincipal = anuncioDetails.FotoPrincipal;
                    }
                }
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Sessão expirada. Por favor faça login novamente.";
                return RedirectToAction("Login", "Account");
            }

            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado. Por favor contacte o suporte.";
                return RedirectToAction("Index");
            }

            var success = await _visitaService.CreateVisitaAsync(model, comprador.ID_Comprador);

            if (success)
            {
                TempData["SuccessMessage"] = "Pedido de visita enviado com sucesso! O vendedor será notificado e deverá aprovar.";
                return RedirectToAction("MinhasVisitas");
            }

            // Verificar possíveis razões de falha
            var anuncio = await _context.Anuncios.FindAsync(model.ID_Anuncio);
            if (anuncio == null)
            {
                TempData["ErrorMessage"] = "O anúncio não foi encontrado ou já não está disponível.";
                return RedirectToAction("Search");
            }

            if (anuncio.Estado_Anuncio == "Vendido")
            {
                TempData["ErrorMessage"] = "Este veículo já foi vendido.";
                return RedirectToAction("Search");
            }

            if (anuncio.Estado_Anuncio != "Ativo" && anuncio.Estado_Anuncio != "Reservado")
            {
                TempData["ErrorMessage"] = $"Este veículo não está disponível para visitas (estado: {anuncio.Estado_Anuncio}).";
                return RedirectToAction("Search");
            }

            // Verificar se já tem visita pendente ou confirmada
            var visitaExistente = await _context.Visita
                .AnyAsync(v => v.ID_Comprador == comprador.ID_Comprador
                          && v.ID_Anuncio == model.ID_Anuncio
                          && (v.Estado == "Pendente" || v.Estado == "Confirmada"));

            if (visitaExistente)
            {
                TempData["ErrorMessage"] = "Já tem um pedido de visita pendente ou confirmado para este veículo.";
                return RedirectToAction("MinhasVisitas");
            }

            // Fallback para erros não identificados
            TempData["ErrorMessage"] = "Não foi possível agendar a visita. Verifique se a data é válida (pelo menos 30 minutos no futuro) e tente novamente.";
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
            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            // Verificar se pode comprar (veículo ativo ou reservado por este comprador)
            var canBuy = await _reservaService.CanBuyAsync(id, comprador.ID_Comprador);
            if (!canBuy)
            {
                TempData["ErrorMessage"] = "Este veículo está reservado por outro comprador e não pode ser comprado neste momento.";
                return RedirectToAction("AnuncioDetails", new { id });
            }

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

            // Verificar se pode comprar (veículo ativo ou reservado por este comprador)
            var canBuy = await _reservaService.CanBuyAsync(model.ID_Anuncio, comprador.ID_Comprador);
            if (!canBuy)
            {
                TempData["ErrorMessage"] = "Este veículo está reservado por outro comprador e não pode ser comprado neste momento.";
                return RedirectToAction("AnuncioDetails", new { id = model.ID_Anuncio });
            }

            var result = await _compraService.CreateCompraAsync(model, comprador.ID_Comprador);

            if (result.CompraId.HasValue)
            {
                TempData["SuccessMessage"] = "Compra realizada com sucesso! Aguarde a confirmação de pagamento.";
                return RedirectToAction("OrderConfirmation", new { id = result.CompraId.Value });
            }

            TempData["ErrorMessage"] = $"Não foi possível concluir a compra: {result.ErrorMessage}";
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

        // ========== FILTROS FAVORITOS ==========

        // POST: /Comprador/SaveFilter (AJAX)
        [HttpPost]
        public async Task<IActionResult> SaveFilter(FiltroFavoritoViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Nome_Filtro))
            {
                return Json(new { success = false, message = "O nome do filtro é obrigatório." });
            }

            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                return Json(new { success = false, message = "Perfil de comprador não encontrado." });
            }

            var success = await _filtroFavoritoService.SaveFilterAsync(model, comprador.ID_Comprador);

            if (success)
            {
                return Json(new { success = true, message = "Filtro guardado com sucesso!" });
            }

            return Json(new { success = false, message = "Não foi possível guardar o filtro." });
        }

        // GET: /Comprador/MeusFiltros
        [HttpGet]
        public async Task<IActionResult> MeusFiltros()
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var filtros = await _filtroFavoritoService.GetFiltersByCompradorAsync(comprador.ID_Comprador);

            return View(filtros);
        }

        // GET: /Comprador/ApplyFilter/5
        [HttpGet]
        public async Task<IActionResult> ApplyFilter(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            try
            {
                var filters = await _filtroFavoritoService.ApplyFilterAsync(id, comprador.ID_Comprador);
                return RedirectToAction("Search", filters);
            }
            catch
            {
                TempData["ErrorMessage"] = "Filtro não encontrado.";
                return RedirectToAction("MeusFiltros");
            }
        }

        // POST: /Comprador/DeleteFilter/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFilter(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == user!.Id);

            if (comprador == null)
            {
                TempData["ErrorMessage"] = "Perfil de comprador não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _filtroFavoritoService.DeleteFilterAsync(id, comprador.ID_Comprador);

            if (success)
            {
                TempData["SuccessMessage"] = "Filtro eliminado com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível eliminar o filtro.";
            }

            return RedirectToAction("MeusFiltros");
        }
    }
}
