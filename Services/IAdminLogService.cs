using Marketplace_LabWebBD.Models;

namespace Marketplace_LabWebBD.Services
{
    public interface IAdminLogService
    {
        Task LogUserBlockedAsync(int adminId, int userId, string motivo, string ipAddress);
        Task LogUserUnblockedAsync(int adminId, int userId, string ipAddress);
        Task LogVendorApprovedAsync(int adminId, int userId, string ipAddress);
        Task LogVendorRejectedAsync(int adminId, int userId, string motivo, string ipAddress);
        Task LogAdminApprovedAsync(int adminId, int userId, string ipAddress);
        Task LogAdminRejectedAsync(int adminId, int userId, string ipAddress);
        Task<List<Log_Admin>> GetLogsByUserAsync(int userId);
        Task<List<Log_Admin>> GetRecentLogsAsync(int count = 50);
    }
}
