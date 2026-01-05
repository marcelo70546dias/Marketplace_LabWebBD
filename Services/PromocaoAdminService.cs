using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_LabWebBD.Services
{
    public class PromocaoAdminService : IPromocaoAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PromocaoAdminService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> CreatePedidoAsync(int userId, string tipoUtilizador, string justificacao)
        {
            try
            {
                // Verificar se já tem pedido pendente
                if (await HasPedidoPendenteAsync(userId))
                    return false;

                // Verificar se já é admin
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return false;

                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (isAdmin)
                    return false;

                var pedido = new PedidoPromocaoAdmin
                {
                    ID_Utilizador = userId,
                    Tipo_Utilizador_Atual = tipoUtilizador,
                    Justificacao = justificacao,
                    Data_Pedido = DateTime.Now,
                    Estado = "Pendente"
                };

                _context.PedidoPromocaoAdmins.Add(pedido);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasPedidoPendenteAsync(int userId)
        {
            return await _context.PedidoPromocaoAdmins
                .AnyAsync(p => p.ID_Utilizador == userId && p.Estado == "Pendente");
        }

        public async Task<List<PedidoPromocaoAdminViewModel>> GetPedidosPendentesAsync()
        {
            var pedidos = await _context.PedidoPromocaoAdmins
                .Include(p => p.ID_UtilizadorNavigation)
                .Where(p => p.Estado == "Pendente")
                .OrderBy(p => p.Data_Pedido)
                .ToListAsync();

            return pedidos.Select(p => MapToViewModel(p)).ToList();
        }

        public async Task<List<PedidoPromocaoAdminViewModel>> GetAllPedidosAsync()
        {
            var pedidos = await _context.PedidoPromocaoAdmins
                .Include(p => p.ID_UtilizadorNavigation)
                .Include(p => p.ID_Admin_RespostaNavigation)
                .OrderByDescending(p => p.Data_Pedido)
                .ToListAsync();

            return pedidos.Select(p => MapToViewModel(p)).ToList();
        }

        public async Task<PedidoPromocaoAdminViewModel?> GetPedidoByIdAsync(int pedidoId)
        {
            var pedido = await _context.PedidoPromocaoAdmins
                .Include(p => p.ID_UtilizadorNavigation)
                .Include(p => p.ID_Admin_RespostaNavigation)
                .FirstOrDefaultAsync(p => p.ID_Pedido == pedidoId);

            return pedido != null ? MapToViewModel(pedido) : null;
        }

        public async Task<bool> AprovarPedidoAsync(int pedidoId, int adminId, string? observacoes)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pedido = await _context.PedidoPromocaoAdmins
                    .FirstOrDefaultAsync(p => p.ID_Pedido == pedidoId && p.Estado == "Pendente");

                if (pedido == null)
                    return false;

                var user = await _userManager.FindByIdAsync(pedido.ID_Utilizador.ToString());
                if (user == null)
                    return false;

                // Adicionar role Admin
                var result = await _userManager.AddToRoleAsync(user, "Admin");
                if (!result.Succeeded)
                    return false;

                // Criar registo na tabela Admin
                var admin = new Admin
                {
                    ID_Utilizador = pedido.ID_Utilizador
                };

                _context.Admins.Add(admin);

                // Atualizar pedido
                pedido.Estado = "Aprovado";
                pedido.Data_Resposta = DateTime.Now;
                pedido.ID_Admin_Resposta = adminId;
                pedido.Observacoes_Admin = observacoes;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> RejeitarPedidoAsync(int pedidoId, int adminId, string? observacoes)
        {
            try
            {
                var pedido = await _context.PedidoPromocaoAdmins
                    .FirstOrDefaultAsync(p => p.ID_Pedido == pedidoId && p.Estado == "Pendente");

                if (pedido == null)
                    return false;

                pedido.Estado = "Rejeitado";
                pedido.Data_Resposta = DateTime.Now;
                pedido.ID_Admin_Resposta = adminId;
                pedido.Observacoes_Admin = observacoes;

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetNumPedidosPendentesAsync()
        {
            return await _context.PedidoPromocaoAdmins
                .CountAsync(p => p.Estado == "Pendente");
        }

        private PedidoPromocaoAdminViewModel MapToViewModel(PedidoPromocaoAdmin pedido)
        {
            return new PedidoPromocaoAdminViewModel
            {
                ID_Pedido = pedido.ID_Pedido,
                ID_Utilizador = pedido.ID_Utilizador,
                NomeUtilizador = pedido.ID_UtilizadorNavigation?.Nome ?? "N/A",
                EmailUtilizador = pedido.ID_UtilizadorNavigation?.Email ?? "N/A",
                Tipo_Utilizador_Atual = pedido.Tipo_Utilizador_Atual,
                Data_Pedido = pedido.Data_Pedido,
                Estado = pedido.Estado,
                Data_Resposta = pedido.Data_Resposta,
                NomeAdminResposta = pedido.ID_Admin_RespostaNavigation?.Nome,
                Justificacao = pedido.Justificacao,
                Observacoes_Admin = pedido.Observacoes_Admin
            };
        }
    }
}
