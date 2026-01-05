using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Services
{
    public interface IReservaService
    {
        Task<bool> CanReserveAsync(int anuncioId, int compradorId);
        Task<CreateReservaViewModel?> GetReservaDetailsAsync(int anuncioId);
        Task<(bool Success, string? ErrorMessage)> CreateReservaAsync(int anuncioId, int compradorId);
        Task<bool> CancelReservaAsync(int historicoId, int userId, string userRole);
        Task<List<ReservaViewModel>> GetReservasByCompradorAsync(int compradorId);
        Task<List<ReservaViewModel>> GetReservasByVendedorAsync(int vendedorId);
        Task ExpireReservasAsync();

        // Novos métodos para aprovação pelo vendedor
        Task<bool> AprovarReservaAsync(int historicoId, int vendedorId);
        Task<bool> RecusarReservaAsync(int historicoId, int vendedorId, string? motivo);
        Task<int> GetNumReservasPendentesAprovacaoAsync(int vendedorId);

        // Verificar se comprador pode comprar
        Task<bool> CanBuyAsync(int anuncioId, int compradorId);
    }
}
