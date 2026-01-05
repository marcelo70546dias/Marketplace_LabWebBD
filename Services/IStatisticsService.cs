using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Services
{
    public interface IStatisticsService
    {
        Task<AdminStatisticsViewModel> GetAdminStatisticsAsync(int? months = 12);
    }
}
