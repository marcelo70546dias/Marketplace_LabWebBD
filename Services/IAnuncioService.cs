using Marketplace_LabWebBD.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Marketplace_LabWebBD.Services
{
    public interface IAnuncioService
    {
        // Listagem e detalhes
        Task<List<AnuncioViewModel>> GetAnunciosByVendedorAsync(int vendedorId);
        Task<AnuncioViewModel?> GetAnuncioByIdAsync(int anuncioId);
        Task<AnuncioViewModel?> GetAnuncioDetailsAsync(int anuncioId, int vendedorId);

        // CRUD
        Task<int> CreateAnuncioAsync(CreateAnuncioViewModel model, int vendedorId);
        Task<bool> UpdateAnuncioAsync(EditAnuncioViewModel model, int vendedorId);
        Task<bool> DeleteAnuncioAsync(int anuncioId, int vendedorId);

        // Gestão de estado
        Task<bool> ChangeEstadoAsync(int anuncioId, string novoEstado, int vendedorId);
        Task<bool> CanEditAnuncioAsync(int anuncioId, int vendedorId);

        // Helper methods para dropdowns
        Task<List<SelectListItem>> GetMarcasAsync();
        Task<List<SelectListItem>> GetModelosByMarcaAsync(int marcaId);
        Task<List<SelectListItem>> GetCombustiveisAsync();

        // Obter dados para edição
        Task<EditAnuncioViewModel?> GetAnuncioForEditAsync(int anuncioId, int vendedorId);

        // Estatísticas do vendedor
        Task<int> GetAnunciosAtivosByVendedorAsync(int vendedorId);
        Task<int> GetAnunciosReservadosByVendedorAsync(int vendedorId);
        Task<int> GetAnunciosVendidosByVendedorAsync(int vendedorId);

        // Gestão de fotografias
        Task<bool> AddFotosAsync(int anuncioId, List<IFormFile> fotos, int vendedorId);
        Task<bool> DeleteFotoAsync(int fotoId, int vendedorId);
        Task<bool> ReorderFotosAsync(int anuncioId, List<int> fotoIds, int vendedorId);
    }
}
