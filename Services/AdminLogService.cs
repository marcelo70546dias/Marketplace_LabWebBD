using Microsoft.EntityFrameworkCore;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;

namespace Marketplace_LabWebBD.Services
{
    public class AdminLogService : IAdminLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminLogService> _logger;

        public AdminLogService(ApplicationDbContext context, ILogger<AdminLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogUserBlockedAsync(int adminId, int userId, string motivo, string ipAddress)
        {
            try
            {
                var log = new Log_Admin
                {
                    ID_Admin = adminId,
                    Tipo_Acao = "BLOQUEIO_UTILIZADOR",
                    Descricao = $"Utilizador bloqueado. Motivo: {motivo}",
                    ID_Utilizador_Afetado = userId,
                    Data_Hora = DateTime.Now,
                    IP_Address = ipAddress
                };

                _context.Log_Admins.Add(log);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Log criado: Admin {adminId} bloqueou utilizador {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar log de bloqueio de utilizador {userId}");
            }
        }

        public async Task LogUserUnblockedAsync(int adminId, int userId, string ipAddress)
        {
            try
            {
                var log = new Log_Admin
                {
                    ID_Admin = adminId,
                    Tipo_Acao = "DESBLOQUEIO_UTILIZADOR",
                    Descricao = "Utilizador desbloqueado",
                    ID_Utilizador_Afetado = userId,
                    Data_Hora = DateTime.Now,
                    IP_Address = ipAddress
                };

                _context.Log_Admins.Add(log);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Log criado: Admin {adminId} desbloqueou utilizador {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar log de desbloqueio de utilizador {userId}");
            }
        }

        public async Task LogVendorApprovedAsync(int adminId, int userId, string ipAddress)
        {
            try
            {
                var log = new Log_Admin
                {
                    ID_Admin = adminId,
                    Tipo_Acao = "APROVACAO_VENDEDOR",
                    Descricao = "Pedido de vendedor aprovado",
                    ID_Utilizador_Afetado = userId,
                    Data_Hora = DateTime.Now,
                    IP_Address = ipAddress
                };

                _context.Log_Admins.Add(log);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Log criado: Admin {adminId} aprovou vendedor {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar log de aprovação de vendedor {userId}");
            }
        }

        public async Task LogVendorRejectedAsync(int adminId, int userId, string motivo, string ipAddress)
        {
            try
            {
                var log = new Log_Admin
                {
                    ID_Admin = adminId,
                    Tipo_Acao = "REJEICAO_VENDEDOR",
                    Descricao = $"Pedido de vendedor rejeitado. Motivo: {motivo}",
                    ID_Utilizador_Afetado = userId,
                    Data_Hora = DateTime.Now,
                    IP_Address = ipAddress
                };

                _context.Log_Admins.Add(log);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Log criado: Admin {adminId} rejeitou vendedor {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar log de rejeição de vendedor {userId}");
            }
        }

        public async Task LogAdminApprovedAsync(int adminId, int userId, string ipAddress)
        {
            try
            {
                var log = new Log_Admin
                {
                    ID_Admin = adminId,
                    Tipo_Acao = "APROVACAO_ADMIN",
                    Descricao = "Pedido de administrador aprovado",
                    ID_Utilizador_Afetado = userId,
                    Data_Hora = DateTime.Now,
                    IP_Address = ipAddress
                };

                _context.Log_Admins.Add(log);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Log criado: Admin {adminId} aprovou admin {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar log de aprovação de admin {userId}");
            }
        }

        public async Task LogAdminRejectedAsync(int adminId, int userId, string ipAddress)
        {
            try
            {
                var log = new Log_Admin
                {
                    ID_Admin = adminId,
                    Tipo_Acao = "REJEICAO_ADMIN",
                    Descricao = "Pedido de administrador rejeitado",
                    ID_Utilizador_Afetado = userId,
                    Data_Hora = DateTime.Now,
                    IP_Address = ipAddress
                };

                _context.Log_Admins.Add(log);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Log criado: Admin {adminId} rejeitou admin {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar log de rejeição de admin {userId}");
            }
        }

        public async Task<List<Log_Admin>> GetLogsByUserAsync(int userId)
        {
            return await _context.Log_Admins
                .Include(l => l.ID_AdminNavigation)
                .Where(l => l.ID_Utilizador_Afetado == userId)
                .OrderByDescending(l => l.Data_Hora)
                .ToListAsync();
        }

        public async Task<List<Log_Admin>> GetRecentLogsAsync(int count = 50)
        {
            return await _context.Log_Admins
                .Include(l => l.ID_AdminNavigation)
                .Include(l => l.ID_Utilizador_AfetadoNavigation)
                .OrderByDescending(l => l.Data_Hora)
                .Take(count)
                .ToListAsync();
        }
    }
}
