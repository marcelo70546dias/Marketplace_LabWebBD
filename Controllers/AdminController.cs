using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.Services;
using Marketplace_LabWebBD.Extensions;
using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAdminLogService _logService;
        private readonly IEmailService _emailService;
        private readonly IPromocaoAdminService _promocaoAdminService;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAdminLogService logService,
            IEmailService emailService,
            IPromocaoAdminService promocaoAdminService)
        {
            _context = context;
            _userManager = userManager;
            _logService = logService;
            _emailService = emailService;
            _promocaoAdminService = promocaoAdminService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.NumPedidosPromocao = await _promocaoAdminService.GetNumPedidosPendentesAsync();
            return View();
        }

        // Página de aprovação de administradores
        public async Task<IActionResult> PendentesAprovacao()
        {
            var pendentes = await _userManager.Users
                .Where(u => !string.IsNullOrEmpty(u.RolePendente))
                .ToListAsync();

            return View(pendentes);
        }

        // Aprovar administrador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprovarAdmin(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null && user.RolePendente == "Administrador")
            {
                // Adicionar role de Administrador
                await _userManager.AddToRoleAsync(user, "Administrador");

                // Limpar role pendente
                user.RolePendente = null;
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = $"Administrador {user.Nome} aprovado com sucesso!";
            }

            return RedirectToAction(nameof(PendentesAprovacao));
        }

        // Rejeitar administrador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejeitarAdmin(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null && user.RolePendente == "Administrador")
            {
                // Adicionar role de Comprador como fallback
                await _userManager.AddToRoleAsync(user, "Comprador");

                // Limpar role pendente
                user.RolePendente = null;
                await _userManager.UpdateAsync(user);

                TempData["InfoMessage"] = $"Pedido de {user.Nome} rejeitado. Conta convertida para Comprador.";
            }

            return RedirectToAction(nameof(PendentesAprovacao));
        }

        // Página de aprovação de vendedores
        public async Task<IActionResult> PendentesVendedor()
        {
            var pendentes = await _userManager.Users
                .Where(u => u.VendorApprovalStatus == "Pending")
                .OrderBy(u => u.Data_Registo)
                .ToListAsync();

            return View(pendentes);
        }

        // Aprovar vendedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprovarVendedor(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null && user.VendorApprovalStatus == "Pending")
            {
                var currentAdmin = await _userManager.GetUserAsync(User);

                // Atualizar status de aprovação
                user.VendorApprovalStatus = "Approved";
                user.ID_Aprovacao_Vendedor = currentAdmin.Id;
                user.Data_Aprovacao_Vendedor = DateOnly.FromDateTime(DateTime.Now);
                await _userManager.UpdateAsync(user);

                // Atribuir role Vendedor
                await _userManager.AddToRoleAsync(user, "Vendedor");

                // Registar ação no log
                await _logService.LogVendorApprovedAsync(
                    currentAdmin.Id,
                    user.Id,
                    HttpContext.GetClientIPAddress());

                // Enviar email de aprovação
                await _emailService.SendVendorApprovalNotificationAsync(
                    user.Email,
                    user.Nome,
                    approved: true);

                TempData["SuccessMessage"] = $"Vendedor {user.Nome} aprovado com sucesso! O utilizador pode agora completar o seu perfil.";
            }

            return RedirectToAction(nameof(PendentesVendedor));
        }

        // Rejeitar vendedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejeitarVendedor(int id, string motivo)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null && user.VendorApprovalStatus == "Pending")
            {
                var currentAdmin = await _userManager.GetUserAsync(User);

                // Atualizar status de aprovação
                user.VendorApprovalStatus = "Rejected";
                user.Motivo_Rejeicao_Vendedor = motivo;
                await _userManager.UpdateAsync(user);

                // Converter para Comprador como fallback
                await _userManager.AddToRoleAsync(user, "Comprador");

                // Criar perfil Comprador
                var comprador = new Comprador
                {
                    ID_Utilizador = user.Id,
                    Notificacoes_Email = true,
                    Notificacoes_Push = true
                };
                _context.Compradors.Add(comprador);
                await _context.SaveChangesAsync();

                // Registar ação no log
                await _logService.LogVendorRejectedAsync(
                    currentAdmin.Id,
                    user.Id,
                    motivo,
                    HttpContext.GetClientIPAddress());

                // Enviar email de rejeição
                await _emailService.SendVendorApprovalNotificationAsync(
                    user.Email,
                    user.Nome,
                    approved: false,
                    rejectReason: motivo);

                TempData["InfoMessage"] = $"Pedido de {user.Nome} rejeitado. Conta convertida para Comprador.";
            }

            return RedirectToAction(nameof(PendentesVendedor));
        }

        // Gestão de Utilizadores - Lista com filtros e pesquisa
        public async Task<IActionResult> Users(string? searchTerm, string? roleFilter, string? blockedFilter, int page = 1)
        {
            var pageSize = 20;
            var query = _userManager.Users.AsQueryable();

            // Aplicar filtro de pesquisa
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    u.Nome.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    (u.Contacto != null && u.Contacto.Contains(searchTerm)));
            }

            // Aplicar filtro de bloqueio
            if (!string.IsNullOrWhiteSpace(blockedFilter))
            {
                if (blockedFilter == "Bloqueado")
                    query = query.Where(u => u.Bloqueado == true);
                else if (blockedFilter == "Ativo")
                    query = query.Where(u => u.Bloqueado != true);
            }

            var totalUsers = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var users = await query
                .OrderByDescending(u => u.Data_Registo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userViewModels = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Aplicar filtro de role (após carregar roles)
                if (!string.IsNullOrWhiteSpace(roleFilter) && !roles.Contains(roleFilter))
                    continue;

                userViewModels.Add(new UserListViewModel
                {
                    Id = user.Id,
                    Nome = user.Nome,
                    Email = user.Email,
                    Contacto = user.Contacto,
                    Data_Registo = user.Data_Registo,
                    Status = user.Status,
                    Bloqueado = user.Bloqueado,
                    Motivo_Bloqueio = user.Motivo_Bloqueio,
                    Roles = roles.ToList(),
                    Email_Validado = user.Email_Validado,
                    VendorApprovalStatus = user.VendorApprovalStatus
                });
            }

            var viewModel = new UserListPageViewModel
            {
                Users = userViewModels,
                SearchTerm = searchTerm,
                RoleFilter = roleFilter,
                BlockedFilter = blockedFilter,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // Detalhes completos de um utilizador
        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                TempData["ErrorMessage"] = "Utilizador não encontrado.";
                return RedirectToAction(nameof(Users));
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Carregar perfil Comprador se aplicável
            var compradorPerfil = await _context.Compradors
                .FirstOrDefaultAsync(c => c.ID_Utilizador == user.Id);

            // Carregar marcas favoritas se for comprador
            var marcasFavoritas = new List<Marca>();
            if (compradorPerfil != null)
            {
                marcasFavoritas = await _context.Preferencias
                    .Where(p => p.ID_Utilizador == user.Id)
                    .Include(p => p.ID_MarcaNavigation)
                    .Select(p => p.ID_MarcaNavigation)
                    .ToListAsync();
            }

            // Carregar perfil Vendedor se aplicável
            var vendedorPerfil = await _context.Vendedors
                .FirstOrDefaultAsync(v => v.ID_Utilizador == user.Id);

            // Carregar logs administrativos
            var logs = await _logService.GetLogsByUserAsync(user.Id);

            // Carregar nome do admin que bloqueou/aprovou
            string? nomeAdminBloqueio = null;
            if (user.ID_Admin_Bloqueio.HasValue)
            {
                var adminBloqueio = await _userManager.FindByIdAsync(user.ID_Admin_Bloqueio.Value.ToString());
                nomeAdminBloqueio = adminBloqueio?.Nome;
            }

            string? nomeAdminAprovacao = null;
            if (user.ID_Aprovacao_Vendedor.HasValue)
            {
                var adminAprovacao = await _userManager.FindByIdAsync(user.ID_Aprovacao_Vendedor.Value.ToString());
                nomeAdminAprovacao = adminAprovacao?.Nome;
            }

            var viewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                Contacto = user.Contacto,
                Morada = user.Morada,
                Data_Registo = user.Data_Registo,
                Status = user.Status,
                Email_Validado = user.Email_Validado,
                Bloqueado = user.Bloqueado,
                Motivo_Bloqueio = user.Motivo_Bloqueio,
                Data_Bloqueio = user.Data_Bloqueio,
                ID_Admin_Bloqueio = user.ID_Admin_Bloqueio,
                NomeAdminBloqueio = nomeAdminBloqueio,
                Roles = roles.ToList(),
                RolePendente = user.RolePendente,
                VendorApprovalStatus = user.VendorApprovalStatus,
                ID_Aprovacao_Vendedor = user.ID_Aprovacao_Vendedor,
                Data_Aprovacao_Vendedor = user.Data_Aprovacao_Vendedor,
                Motivo_Rejeicao_Vendedor = user.Motivo_Rejeicao_Vendedor,
                NomeAdminAprovacao = nomeAdminAprovacao,
                CompradorPerfil = compradorPerfil,
                MarcasFavoritas = marcasFavoritas,
                VendedorPerfil = vendedorPerfil,
                Logs = logs
            };

            return View(viewModel);
        }

        // Bloquear utilizador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(BlockUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dados inválidos.";
                return RedirectToAction(nameof(UserDetails), new { id = model.UserId });
            }

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());

            if (user == null)
            {
                TempData["ErrorMessage"] = "Utilizador não encontrado.";
                return RedirectToAction(nameof(Users));
            }

            if (user.Bloqueado == true)
            {
                TempData["ErrorMessage"] = "Utilizador já está bloqueado.";
                return RedirectToAction(nameof(UserDetails), new { id = model.UserId });
            }

            var currentAdmin = await _userManager.GetUserAsync(User);

            // Bloquear utilizador
            user.Bloqueado = true;
            user.Motivo_Bloqueio = model.Motivo;
            user.Data_Bloqueio = DateOnly.FromDateTime(DateTime.Now);
            user.ID_Admin_Bloqueio = currentAdmin.Id;

            // Invalidar sessão do utilizador (força logout)
            await _userManager.UpdateSecurityStampAsync(user);
            await _userManager.UpdateAsync(user);

            // Registar no log
            await _logService.LogUserBlockedAsync(
                currentAdmin.Id,
                user.Id,
                model.Motivo,
                HttpContext.GetClientIPAddress());

            // Enviar email de notificação
            await _emailService.SendAccountBlockedNotificationAsync(
                user.Email,
                user.Nome,
                model.Motivo);

            TempData["SuccessMessage"] = $"Utilizador {user.Nome} bloqueado com sucesso.";
            return RedirectToAction(nameof(UserDetails), new { id = user.Id });
        }

        // Desbloquear utilizador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                TempData["ErrorMessage"] = "Utilizador não encontrado.";
                return RedirectToAction(nameof(Users));
            }

            if (user.Bloqueado != true)
            {
                TempData["ErrorMessage"] = "Utilizador não está bloqueado.";
                return RedirectToAction(nameof(UserDetails), new { id });
            }

            var currentAdmin = await _userManager.GetUserAsync(User);

            // Desbloquear
            user.Bloqueado = false;
            user.Motivo_Bloqueio = null;
            user.Data_Bloqueio = null;
            user.ID_Admin_Bloqueio = null;
            await _userManager.UpdateAsync(user);

            // Registar no log
            await _logService.LogUserUnblockedAsync(
                currentAdmin.Id,
                user.Id,
                HttpContext.GetClientIPAddress());

            TempData["SuccessMessage"] = $"Utilizador {user.Nome} desbloqueado com sucesso.";
            return RedirectToAction(nameof(UserDetails), new { id });
        }

        // Logs administrativos
        public async Task<IActionResult> AdminLogs(string? actionFilter, int? userId, int page = 1)
        {
            var pageSize = 50;
            var query = _context.Log_Admins
                .Include(l => l.ID_AdminNavigation)
                .Include(l => l.ID_Utilizador_AfetadoNavigation)
                .AsQueryable();

            // Aplicar filtro de ação
            if (!string.IsNullOrWhiteSpace(actionFilter))
            {
                query = query.Where(l => l.Tipo_Acao == actionFilter);
            }

            // Aplicar filtro de utilizador
            if (userId.HasValue)
            {
                query = query.Where(l => l.ID_Utilizador_Afetado == userId.Value);
            }

            var totalLogs = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalLogs / (double)pageSize);

            var logs = await query
                .OrderByDescending(l => l.Data_Hora)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.ActionFilter = actionFilter;
            ViewBag.UserId = userId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(logs);
        }

        // GET: /Admin/PedidosPromocao
        public async Task<IActionResult> PedidosPromocao(string? filtroEstado)
        {
            List<PedidoPromocaoAdminViewModel> pedidos;

            if (string.IsNullOrEmpty(filtroEstado) || filtroEstado == "Pendente")
            {
                pedidos = await _promocaoAdminService.GetPedidosPendentesAsync();
            }
            else
            {
                pedidos = await _promocaoAdminService.GetAllPedidosAsync();
                if (filtroEstado != "Todos")
                {
                    pedidos = pedidos.Where(p => p.Estado == filtroEstado).ToList();
                }
            }

            ViewBag.FiltroEstado = filtroEstado ?? "Pendente";
            ViewBag.NumPendentes = await _promocaoAdminService.GetNumPedidosPendentesAsync();

            return View(pedidos);
        }

        // POST: /Admin/AprovarPromocao/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprovarPromocao(int id, string? observacoes)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utilizador não encontrado.";
                return RedirectToAction("PedidosPromocao");
            }

            var success = await _promocaoAdminService.AprovarPedidoAsync(id, user.Id, observacoes);

            if (success)
            {
                TempData["SuccessMessage"] = "Pedido aprovado com sucesso! O utilizador foi promovido a Administrador.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível aprovar o pedido.";
            }

            return RedirectToAction("PedidosPromocao");
        }

        // POST: /Admin/RejeitarPromocao/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejeitarPromocao(int id, string? observacoes)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utilizador não encontrado.";
                return RedirectToAction("PedidosPromocao");
            }

            var success = await _promocaoAdminService.RejeitarPedidoAsync(id, user.Id, observacoes);

            if (success)
            {
                TempData["SuccessMessage"] = "Pedido rejeitado.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível rejeitar o pedido.";
            }

            return RedirectToAction("PedidosPromocao");
        }
    }
}
