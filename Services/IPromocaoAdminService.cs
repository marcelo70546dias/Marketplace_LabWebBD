using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Services
{
    public interface IPromocaoAdminService
    {
        Task<bool> CreatePedidoAsync(int userId, string tipoUtilizador, string justificacao);
        Task<bool> HasPedidoPendenteAsync(int userId);
        Task<List<PedidoPromocaoAdminViewModel>> GetPedidosPendentesAsync();
        Task<List<PedidoPromocaoAdminViewModel>> GetAllPedidosAsync();
        Task<PedidoPromocaoAdminViewModel?> GetPedidoByIdAsync(int pedidoId);
        Task<bool> AprovarPedidoAsync(int pedidoId, int adminId, string? observacoes);
        Task<bool> RejeitarPedidoAsync(int pedidoId, int adminId, string? observacoes);
        Task<int> GetNumPedidosPendentesAsync();
    }
}
