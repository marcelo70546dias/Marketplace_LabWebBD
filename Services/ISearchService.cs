using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Search for anuncios with filters and pagination
        /// </summary>
        Task<SearchViewModel> SearchAnunciosAsync(SearchViewModel filters, int page, int pageSize);

        /// <summary>
        /// Get recent anuncios (for homepage/dashboard)
        /// </summary>
        Task<List<AnuncioViewModel>> GetRecentAnunciosAsync(int count = 6);

        /// <summary>
        /// Get featured anuncios (for homepage/dashboard)
        /// </summary>
        Task<List<AnuncioViewModel>> GetFeaturedAnunciosAsync(int count = 6);

        /// <summary>
        /// Get anuncio details for comprador view
        /// </summary>
        Task<AnuncioViewModel?> GetAnuncioDetailsAsync(int anuncioId);
    }
}
