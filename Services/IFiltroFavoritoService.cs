using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Services
{
    public interface IFiltroFavoritoService
    {
        Task<bool> SaveFilterAsync(FiltroFavoritoViewModel model, int compradorId);
        Task<List<FiltroFavoritoViewModel>> GetFiltersByCompradorAsync(int compradorId);
        Task<FiltroFavoritoViewModel?> GetFilterByIdAsync(int filtroId, int compradorId);
        Task<bool> DeleteFilterAsync(int filtroId, int compradorId);
        Task<SearchViewModel> ApplyFilterAsync(int filtroId, int compradorId);
    }
}
