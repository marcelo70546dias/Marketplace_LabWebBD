using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_LabWebBD.Services
{
    public class FiltroFavoritoService : IFiltroFavoritoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISearchService _searchService;

        public FiltroFavoritoService(ApplicationDbContext context, ISearchService searchService)
        {
            _context = context;
            _searchService = searchService;
        }

        public async Task<bool> SaveFilterAsync(FiltroFavoritoViewModel model, int compradorId)
        {
            try
            {
                var filtro = new Filtro_Favorito
                {
                    ID_Comprador = compradorId,
                    Nome_Filtro = model.Nome_Filtro,
                    Categoria = model.Categoria,
                    ID_Marca = model.ID_Marca,
                    ID_Modelo = model.ID_Modelo,
                    Ano_Min = model.Ano_Min,
                    Ano_Max = model.Ano_Max,
                    Preco_Min = model.Preco_Min,
                    Preco_Max = model.Preco_Max,
                    Quilometragem_Min = model.Quilometragem_Min,
                    Quilometragem_Max = model.Quilometragem_Max,
                    ID_Combustivel = model.ID_Combustivel,
                    Caixa = model.Caixa,
                    Cor = model.Cor,
                    Lotacao_Min = model.Lotacao_Min,
                    Lotacao_Max = model.Lotacao_Max,
                    Localizacao = model.Localizacao,
                    Data_Criacao = DateTime.Now
                };

                _context.Filtro_Favoritos.Add(filtro);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<FiltroFavoritoViewModel>> GetFiltersByCompradorAsync(int compradorId)
        {
            var filtros = await _context.Filtro_Favoritos
                .Include(f => f.ID_MarcaNavigation)
                .Include(f => f.ID_ModeloNavigation)
                .Include(f => f.ID_CombustivelNavigation)
                .Where(f => f.ID_Comprador == compradorId)
                .OrderByDescending(f => f.Data_Criacao)
                .ToListAsync();

            var viewModels = new List<FiltroFavoritoViewModel>();

            foreach (var filtro in filtros)
            {
                // Contar resultados atuais para este filtro
                var searchFilters = new SearchViewModel
                {
                    Categoria = filtro.Categoria,
                    ID_Marca = filtro.ID_Marca,
                    ID_Modelo = filtro.ID_Modelo,
                    Ano_Min = filtro.Ano_Min,
                    Ano_Max = filtro.Ano_Max,
                    Preco_Min = filtro.Preco_Min,
                    Preco_Max = filtro.Preco_Max,
                    Quilometragem_Min = filtro.Quilometragem_Min,
                    Quilometragem_Max = filtro.Quilometragem_Max,
                    ID_Combustivel = filtro.ID_Combustivel,
                    Caixa = filtro.Caixa,
                    Cor = filtro.Cor,
                    Lotacao_Min = filtro.Lotacao_Min,
                    Lotacao_Max = filtro.Lotacao_Max,
                    Distrito = filtro.Localizacao
                };

                var results = await _searchService.SearchAnunciosAsync(searchFilters, 1, 1);

                viewModels.Add(new FiltroFavoritoViewModel
                {
                    ID_Filtro = filtro.ID_Filtro,
                    Nome_Filtro = filtro.Nome_Filtro,
                    Categoria = filtro.Categoria,
                    ID_Marca = filtro.ID_Marca,
                    ID_Modelo = filtro.ID_Modelo,
                    Ano_Min = filtro.Ano_Min,
                    Ano_Max = filtro.Ano_Max,
                    Preco_Min = filtro.Preco_Min,
                    Preco_Max = filtro.Preco_Max,
                    Quilometragem_Min = filtro.Quilometragem_Min,
                    Quilometragem_Max = filtro.Quilometragem_Max,
                    ID_Combustivel = filtro.ID_Combustivel,
                    Caixa = filtro.Caixa,
                    Cor = filtro.Cor,
                    Lotacao_Min = filtro.Lotacao_Min,
                    Lotacao_Max = filtro.Lotacao_Max,
                    Localizacao = filtro.Localizacao,
                    Data_Criacao = filtro.Data_Criacao,
                    MarcaNome = filtro.ID_MarcaNavigation?.Nome,
                    ModeloNome = filtro.ID_ModeloNavigation?.Nome,
                    CombustivelNome = filtro.ID_CombustivelNavigation?.Tipo,
                    NumResultados = results.TotalResultados
                });
            }

            return viewModels;
        }

        public async Task<FiltroFavoritoViewModel?> GetFilterByIdAsync(int filtroId, int compradorId)
        {
            var filtro = await _context.Filtro_Favoritos
                .Include(f => f.ID_MarcaNavigation)
                .Include(f => f.ID_ModeloNavigation)
                .Include(f => f.ID_CombustivelNavigation)
                .FirstOrDefaultAsync(f => f.ID_Filtro == filtroId && f.ID_Comprador == compradorId);

            if (filtro == null)
                return null;

            return new FiltroFavoritoViewModel
            {
                ID_Filtro = filtro.ID_Filtro,
                Nome_Filtro = filtro.Nome_Filtro,
                Categoria = filtro.Categoria,
                ID_Marca = filtro.ID_Marca,
                ID_Modelo = filtro.ID_Modelo,
                Ano_Min = filtro.Ano_Min,
                Ano_Max = filtro.Ano_Max,
                Preco_Min = filtro.Preco_Min,
                Preco_Max = filtro.Preco_Max,
                Quilometragem_Min = filtro.Quilometragem_Min,
                Quilometragem_Max = filtro.Quilometragem_Max,
                ID_Combustivel = filtro.ID_Combustivel,
                Caixa = filtro.Caixa,
                Cor = filtro.Cor,
                Lotacao_Min = filtro.Lotacao_Min,
                Lotacao_Max = filtro.Lotacao_Max,
                Localizacao = filtro.Localizacao,
                Data_Criacao = filtro.Data_Criacao,
                MarcaNome = filtro.ID_MarcaNavigation?.Nome,
                ModeloNome = filtro.ID_ModeloNavigation?.Nome,
                CombustivelNome = filtro.ID_CombustivelNavigation?.Tipo
            };
        }

        public async Task<bool> DeleteFilterAsync(int filtroId, int compradorId)
        {
            try
            {
                var filtro = await _context.Filtro_Favoritos
                    .FirstOrDefaultAsync(f => f.ID_Filtro == filtroId && f.ID_Comprador == compradorId);

                if (filtro == null)
                    return false;

                _context.Filtro_Favoritos.Remove(filtro);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<SearchViewModel> ApplyFilterAsync(int filtroId, int compradorId)
        {
            var filtro = await _context.Filtro_Favoritos
                .FirstOrDefaultAsync(f => f.ID_Filtro == filtroId && f.ID_Comprador == compradorId);

            if (filtro == null)
                throw new InvalidOperationException("Filtro não encontrado.");

            return new SearchViewModel
            {
                Categoria = filtro.Categoria,
                ID_Marca = filtro.ID_Marca,
                ID_Modelo = filtro.ID_Modelo,
                Ano_Min = filtro.Ano_Min,
                Ano_Max = filtro.Ano_Max,
                Preco_Min = filtro.Preco_Min,
                Preco_Max = filtro.Preco_Max,
                Quilometragem_Min = filtro.Quilometragem_Min,
                Quilometragem_Max = filtro.Quilometragem_Max,
                ID_Combustivel = filtro.ID_Combustivel,
                Caixa = filtro.Caixa,
                Cor = filtro.Cor,
                Lotacao_Min = filtro.Lotacao_Min,
                Lotacao_Max = filtro.Lotacao_Max,
                Distrito = filtro.Localizacao
            };
        }
    }
}
