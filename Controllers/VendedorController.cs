using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.ViewModels;
using Marketplace_LabWebBD.Services;

namespace Marketplace_LabWebBD.Controllers
{
    [Authorize(Roles = "Vendedor")]
    public class VendedorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAnuncioService _anuncioService;
        private readonly IReservaService _reservaService;
        private readonly IVisitaService _visitaService;
        private readonly ICompraService _compraService;
        private readonly IPromocaoAdminService _promocaoAdminService;

        public VendedorController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IAnuncioService anuncioService,
            IReservaService reservaService,
            IVisitaService visitaService,
            ICompraService compraService,
            IPromocaoAdminService promocaoAdminService)
        {
            _userManager = userManager;
            _context = context;
            _anuncioService = anuncioService;
            _reservaService = reservaService;
            _visitaService = visitaService;
            _compraService = compraService;
            _promocaoAdminService = promocaoAdminService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            // Verificar se perfil de vendedor está completo
            var vendedor = await _context.Vendedors
                .FirstOrDefaultAsync(v => v.ID_Utilizador == user.Id);

            if (vendedor == null)
            {
                // Perfil não completo, redirecionar
                TempData["InfoMessage"] = "Por favor complete o seu perfil de vendedor antes de continuar.";
                return RedirectToAction("CompleteProfile");
            }

            // Obter estatísticas
            ViewBag.AnunciosAtivos = await _anuncioService.GetAnunciosAtivosByVendedorAsync(vendedor.ID_Utilizador);
            ViewBag.AnunciosReservados = await _anuncioService.GetAnunciosReservadosByVendedorAsync(vendedor.ID_Utilizador);
            ViewBag.AnunciosVendidos = await _anuncioService.GetAnunciosVendidosByVendedorAsync(vendedor.ID_Utilizador);

            return View();
        }

        // GET: /Vendedor/CompleteProfile
        [HttpGet]
        public async Task<IActionResult> CompleteProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Utilizador não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            // Verificar se perfil já está completo
            var vendedor = await _context.Vendedors
                .FirstOrDefaultAsync(v => v.ID_Utilizador == user.Id);

            if (vendedor != null)
            {
                // Perfil já completo, redirecionar para dashboard
                TempData["InfoMessage"] = "Perfil já está completo.";
                return RedirectToAction("Index");
            }

            // Debug info
            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.Nome;
            ViewBag.ApprovalStatus = user.VendorApprovalStatus;
            ViewBag.ApprovalId = user.ID_Aprovacao_Vendedor;
            ViewBag.ApprovalDate = user.Data_Aprovacao_Vendedor;

            return View();
        }

        // POST: /Vendedor/CompleteProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(CompleteVendedorProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "Utilizador não encontrado.";
                        return RedirectToAction("Index", "Home");
                    }

                    // Verificar se já tem perfil
                    var existingVendedor = await _context.Vendedors
                        .FirstOrDefaultAsync(v => v.ID_Utilizador == user.Id);

                    if (existingVendedor != null)
                    {
                        TempData["ErrorMessage"] = "Perfil já foi completado.";
                        return RedirectToAction("Index");
                    }

                    // Verificar se foi aprovado
                    if (user.VendorApprovalStatus != "Approved")
                    {
                        TempData["ErrorMessage"] = "O seu pedido de vendedor ainda não foi aprovado.";
                        return RedirectToAction("Index", "Home");
                    }

                    // Verificar se o admin que aprovou existe na tabela Admin
                    int? idAprovacao = null;
                    if (user.ID_Aprovacao_Vendedor != null)
                    {
                        var adminRecord = await _context.Admins
                            .FirstOrDefaultAsync(a => a.ID_Utilizador == user.ID_Aprovacao_Vendedor);

                        idAprovacao = adminRecord?.ID_Utilizador;
                    }

                    // Criar entrada Vendedor
                    var vendedor = new Vendedor
                    {
                        ID_Utilizador = user.Id,
                        Tipo = model.Tipo,
                        NIF = model.NIF,
                        Dados_Faturacao = model.Dados_Faturacao,
                        ID_Aprovacao = idAprovacao,
                        Data_Aprovacao = user.Data_Aprovacao_Vendedor
                    };

                    _context.Vendedors.Add(vendedor);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Perfil de vendedor completado com sucesso! Pode agora criar anúncios.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Erro ao completar perfil: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        TempData["ErrorMessage"] += $" | Detalhes: {ex.InnerException.Message}";
                    }
                    return View(model);
                }
            }

            return View(model);
        }

        // ==================== GESTÃO DE ANÚNCIOS ====================

        // GET: /Vendedor/MeusAnuncios
        [HttpGet]
        public async Task<IActionResult> MeusAnuncios()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
                return RedirectToAction("CompleteProfile");

            var anuncios = await _anuncioService.GetAnunciosByVendedorAsync(vendedor.ID_Utilizador);
            return View(anuncios);
        }

        // GET: /Vendedor/CreateAnuncio
        [HttpGet]
        public async Task<IActionResult> CreateAnuncio()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
                return RedirectToAction("CompleteProfile");

            var model = new CreateAnuncioViewModel
            {
                Categorias = GetCategoriasSelectList(),
                Marcas = await _anuncioService.GetMarcasAsync(),
                Modelos = new List<SelectListItem>(), // Preenchido via AJAX
                Transmissoes = GetTransmissoesSelectList(),
                Combustiveis = await _anuncioService.GetCombustiveisAsync(),
                Cores = _anuncioService.GetCores(),
                Distritos = GetDistritosSelectList()
            };

            return View(model);
        }

        // POST: /Vendedor/CreateAnuncio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnuncio(CreateAnuncioViewModel model, List<IFormFile>? Fotos)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

                if (vendedor == null)
                    return RedirectToAction("CompleteProfile");

                var anuncioId = await _anuncioService.CreateAnuncioAsync(model, vendedor.ID_Utilizador);

                // Upload fotos se fornecidas
                if (Fotos != null && Fotos.Count > 0)
                {
                    await _anuncioService.AddFotosAsync(anuncioId, Fotos, vendedor.ID_Utilizador);
                }

                TempData["SuccessMessage"] = "Anúncio criado com sucesso!";
                return RedirectToAction("MeusAnuncios");
            }

            // Recarregar dropdowns se validação falhar
            model.Categorias = GetCategoriasSelectList();
            model.Marcas = await _anuncioService.GetMarcasAsync();
            model.Modelos = await _anuncioService.GetModelosByMarcaAsync(model.ID_Marca);
            model.Transmissoes = GetTransmissoesSelectList();
            model.Combustiveis = await _anuncioService.GetCombustiveisAsync();
            model.Cores = _anuncioService.GetCores();
            model.Distritos = GetDistritosSelectList();

            return View(model);
        }

        // GET: /Vendedor/EditAnuncio/5
        [HttpGet]
        public async Task<IActionResult> EditAnuncio(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
                return RedirectToAction("CompleteProfile");

            var model = await _anuncioService.GetAnuncioForEditAsync(id, vendedor.ID_Utilizador);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Anúncio não encontrado ou não pode ser editado.";
                return RedirectToAction("MeusAnuncios");
            }

            // Popular dropdowns que não vêm do service
            model.Categorias = GetCategoriasSelectList();
            model.Transmissoes = GetTransmissoesSelectList();
            model.Distritos = GetDistritosSelectList();

            return View(model);
        }

        // POST: /Vendedor/EditAnuncio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnuncio(EditAnuncioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

                if (vendedor == null)
                    return RedirectToAction("CompleteProfile");

                var success = await _anuncioService.UpdateAnuncioAsync(model, vendedor.ID_Utilizador);

                if (success)
                {
                    TempData["SuccessMessage"] = "Anúncio atualizado com sucesso!";
                    return RedirectToAction("MeusAnuncios");
                }
                else
                {
                    TempData["ErrorMessage"] = "Não foi possível atualizar o anúncio.";
                }
            }

            // Recarregar dados se falhar
            model.Categorias = GetCategoriasSelectList();
            model.Marcas = await _anuncioService.GetMarcasAsync();
            model.Modelos = await _anuncioService.GetModelosByMarcaAsync(model.ID_Marca);
            model.Transmissoes = GetTransmissoesSelectList();
            model.Combustiveis = await _anuncioService.GetCombustiveisAsync();
            model.Distritos = GetDistritosSelectList();

            return View(model);
        }

        // POST: /Vendedor/DeleteAnuncio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnuncio(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
                return RedirectToAction("CompleteProfile");

            var success = await _anuncioService.DeleteAnuncioAsync(id, vendedor.ID_Utilizador);

            if (success)
            {
                TempData["SuccessMessage"] = "Anúncio eliminado com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível eliminar o anúncio. Veículos reservados ou vendidos não podem ser eliminados.";
            }

            return RedirectToAction("MeusAnuncios");
        }

        // POST: /Vendedor/ChangeEstado
        [HttpPost]
        public async Task<IActionResult> ChangeEstado(int id, string estado)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
                return Json(new { success = false, message = "Perfil não encontrado" });

            var success = await _anuncioService.ChangeEstadoAsync(id, estado, vendedor.ID_Utilizador);

            if (success)
            {
                return Json(new { success = true, message = $"Estado alterado para {estado}" });
            }

            return Json(new { success = false, message = "Não foi possível alterar o estado" });
        }

        // GET: /Vendedor/GetModelosByMarca (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetModelosByMarca(int marcaId)
        {
            var modelos = await _anuncioService.GetModelosByMarcaAsync(marcaId);
            return Json(modelos);
        }

        // POST: /Vendedor/UploadFotos (AJAX)
        [HttpPost]
        public async Task<IActionResult> UploadFotos(int anuncioId, List<IFormFile> fotos)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
                return Json(new { success = false, message = "Perfil não encontrado" });

            if (fotos == null || fotos.Count == 0)
                return Json(new { success = false, message = "Nenhuma foto selecionada" });

            var success = await _anuncioService.AddFotosAsync(anuncioId, fotos, vendedor.ID_Utilizador);

            if (success)
            {
                return Json(new { success = true, message = $"{fotos.Count} foto(s) adicionada(s) com sucesso" });
            }

            return Json(new { success = false, message = "Não foi possível adicionar as fotos" });
        }

        // POST: /Vendedor/DeleteFoto (AJAX)
        [HttpPost]
        public async Task<IActionResult> DeleteFoto(int fotoId)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
                return Json(new { success = false, message = "Perfil não encontrado" });

            var success = await _anuncioService.DeleteFotoAsync(fotoId, vendedor.ID_Utilizador);

            if (success)
            {
                return Json(new { success = true, message = "Foto eliminada com sucesso" });
            }

            return Json(new { success = false, message = "Não foi possível eliminar a foto" });
        }

        // GET: /Vendedor/AnunciosReservados
        [HttpGet]
        public async Task<IActionResult> AnunciosReservados()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
            {
                TempData["ErrorMessage"] = "Perfil de vendedor não encontrado.";
                return RedirectToAction("Index");
            }

            var reservas = await _reservaService.GetReservasByVendedorAsync(vendedor.ID_Utilizador);

            return View(reservas);
        }

        // POST: /Vendedor/AprovarReserva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprovarReserva(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
            {
                TempData["ErrorMessage"] = "Perfil de vendedor não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _reservaService.AprovarReservaAsync(id, vendedor.ID_Utilizador);

            if (success)
            {
                TempData["SuccessMessage"] = "Reserva aprovada com sucesso! O veículo está agora reservado para este comprador.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível aprovar a reserva. O veículo pode já estar reservado.";
            }

            return RedirectToAction("AnunciosReservados");
        }

        // POST: /Vendedor/RecusarReserva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecusarReserva(int id, string? motivo)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
            {
                TempData["ErrorMessage"] = "Perfil de vendedor não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _reservaService.RecusarReservaAsync(id, vendedor.ID_Utilizador, motivo);

            if (success)
            {
                TempData["SuccessMessage"] = "Pedido de reserva recusado.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível recusar o pedido de reserva.";
            }

            return RedirectToAction("AnunciosReservados");
        }

        // POST: /Vendedor/CancelarReserva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var success = await _reservaService.CancelReservaAsync(id, user!.Id, "Vendedor");

            if (success)
            {
                TempData["SuccessMessage"] = "Reserva cancelada com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível cancelar a reserva.";
            }

            return RedirectToAction("AnunciosReservados");
        }

        // ========== VISITAS ==========

        // GET: /Vendedor/VisitasAgendadas
        [HttpGet]
        public async Task<IActionResult> VisitasAgendadas()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
            {
                TempData["ErrorMessage"] = "Perfil de vendedor não encontrado.";
                return RedirectToAction("Index");
            }

            var visitas = await _visitaService.GetVisitasByVendedorAsync(vendedor.ID_Utilizador);

            return View(visitas);
        }

        // POST: /Vendedor/MarcarVisitaRealizada/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarVisitaRealizada(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
            {
                TempData["ErrorMessage"] = "Perfil de vendedor não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _visitaService.MarkVisitaRealizadaAsync(id, vendedor.ID_Utilizador);

            if (success)
            {
                TempData["SuccessMessage"] = "Visita marcada como realizada.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível marcar a visita como realizada.";
            }

            return RedirectToAction("VisitasAgendadas");
        }

        // POST: /Vendedor/CancelarVisita/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarVisita(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var success = await _visitaService.CancelVisitaAsync(id, user!.Id, "Vendedor");

            if (success)
            {
                TempData["SuccessMessage"] = "Visita cancelada com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível cancelar a visita.";
            }

            return RedirectToAction("VisitasAgendadas");
        }

        // POST: /Vendedor/AprovarVisita/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprovarVisita(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
            {
                TempData["ErrorMessage"] = "Perfil de vendedor não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _visitaService.AprovarVisitaAsync(id, vendedor.ID_Utilizador);

            if (success)
            {
                TempData["SuccessMessage"] = "Visita aprovada com sucesso! O comprador será notificado.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível aprovar a visita. Verifique se ainda está pendente e dentro do prazo.";
            }

            return RedirectToAction("VisitasAgendadas");
        }

        // POST: /Vendedor/RecusarVisita/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecusarVisita(int id, string? motivo)
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
            {
                TempData["ErrorMessage"] = "Perfil de vendedor não encontrado.";
                return RedirectToAction("Index");
            }

            var success = await _visitaService.RecusarVisitaAsync(id, vendedor.ID_Utilizador, motivo);

            if (success)
            {
                TempData["SuccessMessage"] = "Visita recusada.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível recusar a visita.";
            }

            return RedirectToAction("VisitasAgendadas");
        }

        // ========== VENDAS ==========

        // GET: /Vendedor/VeiculosVendidos
        [HttpGet]
        public async Task<IActionResult> VeiculosVendidos()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == user!.Id);

            if (vendedor == null)
            {
                TempData["ErrorMessage"] = "Perfil de vendedor não encontrado.";
                return RedirectToAction("Index");
            }

            var vendas = await _compraService.GetComprasByVendedorAsync(vendedor.ID_Utilizador);

            return View(vendas);
        }

        // ========== MÉTODOS AUXILIARES ==========

        // Métodos privados para listas fixas
        private List<SelectListItem> GetCategoriasSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Selecione a categoria...", Disabled = true, Selected = true },
                new SelectListItem { Value = "SUV", Text = "SUV" },
                new SelectListItem { Value = "Coupe", Text = "Coupe" },
                new SelectListItem { Value = "Hatchback", Text = "Hatchback" },
                new SelectListItem { Value = "Saloon", Text = "Saloon" },
                new SelectListItem { Value = "Executive", Text = "Executive" },
                new SelectListItem { Value = "Sports", Text = "Sports" }
            };
        }

        // GET: /Vendedor/SolicitarPromocao
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

        // POST: /Vendedor/SolicitarPromocao
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

            var success = await _promocaoAdminService.CreatePedidoAsync(user.Id, "Vendedor", model.Justificacao);

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

        private List<SelectListItem> GetTransmissoesSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Selecione a transmissão...", Disabled = true, Selected = true },
                new SelectListItem { Value = "Manual", Text = "Manual" },
                new SelectListItem { Value = "Automática", Text = "Automática" }
            };
        }

        private List<SelectListItem> GetDistritosSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Selecione o distrito...", Disabled = true, Selected = true },
                new SelectListItem { Value = "Aveiro", Text = "Aveiro" },
                new SelectListItem { Value = "Beja", Text = "Beja" },
                new SelectListItem { Value = "Braga", Text = "Braga" },
                new SelectListItem { Value = "Bragança", Text = "Bragança" },
                new SelectListItem { Value = "Castelo Branco", Text = "Castelo Branco" },
                new SelectListItem { Value = "Coimbra", Text = "Coimbra" },
                new SelectListItem { Value = "Évora", Text = "Évora" },
                new SelectListItem { Value = "Faro", Text = "Faro" },
                new SelectListItem { Value = "Guarda", Text = "Guarda" },
                new SelectListItem { Value = "Leiria", Text = "Leiria" },
                new SelectListItem { Value = "Lisboa", Text = "Lisboa" },
                new SelectListItem { Value = "Portalegre", Text = "Portalegre" },
                new SelectListItem { Value = "Porto", Text = "Porto" },
                new SelectListItem { Value = "Santarém", Text = "Santarém" },
                new SelectListItem { Value = "Setúbal", Text = "Setúbal" },
                new SelectListItem { Value = "Viana do Castelo", Text = "Viana do Castelo" },
                new SelectListItem { Value = "Vila Real", Text = "Vila Real" },
                new SelectListItem { Value = "Viseu", Text = "Viseu" },
                new SelectListItem { Value = "Açores", Text = "Açores" },
                new SelectListItem { Value = "Madeira", Text = "Madeira" }
            };
        }
    }
}
