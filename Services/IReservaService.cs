using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Services
{
    public interface IReservaService
    {
        Task<bool> CanReserveAsync(int anuncioId);
        Task<CreateReservaViewModel?> GetReservaDetailsAsync(int anuncioId);
        Task<bool> CreateReservaAsync(int anuncioId, int compradorId);
        Task<bool> CancelReservaAsync(int anuncioId, int compradorId);
        Task<List<ReservaViewModel>> GetReservasByCompradorAsync(int compradorId);
        Task<List<ReservaViewModel>> GetReservasByVendedorAsync(int vendedorId);
        Task ExpireReservasAsync();
    }
}
