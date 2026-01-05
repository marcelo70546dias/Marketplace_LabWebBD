using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Services
{
    public interface IVisitaService
    {
        Task<CreateVisitaViewModel?> GetVisitaDetailsAsync(int anuncioId);
        Task<bool> CreateVisitaAsync(CreateVisitaViewModel model, int compradorId);
        Task<List<VisitaViewModel>> GetVisitasByCompradorAsync(int compradorId);
        Task<List<VisitaViewModel>> GetVisitasByVendedorAsync(int vendedorId);
        Task<bool> CancelVisitaAsync(int visitaId, int userId, string userRole);
        Task<bool> MarkVisitaRealizadaAsync(int visitaId, int vendedorId);
    }
}
