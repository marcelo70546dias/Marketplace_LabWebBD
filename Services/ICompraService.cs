using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Services
{
    public interface ICompraService
    {
        Task<CheckoutViewModel?> GetCheckoutDetailsAsync(int anuncioId);
        Task<int?> CreateCompraAsync(CheckoutViewModel model, int compradorId);
        Task<OrderConfirmationViewModel?> GetOrderConfirmationAsync(int compraId, int compradorId);
        Task<bool> SimulatePaymentAsync(int compraId, int compradorId);
        Task<List<CompraViewModel>> GetComprasByCompradorAsync(int compradorId);
        Task<List<CompraViewModel>> GetComprasByVendedorAsync(int vendedorId);
        Task<bool> CancelCompraAsync(int compraId, int compradorId);
    }
}
