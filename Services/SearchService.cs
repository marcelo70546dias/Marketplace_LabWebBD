using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Marketplace_LabWebBD.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SearchService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<SearchViewModel> SearchAnunciosAsync(SearchViewModel filters, int page, int pageSize)
        {
            // Base query - only show Active anuncios
            var query = _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_CombustivelNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Where(a => a.Estado_Anuncio == "Ativo");

            // Apply filters
            if (!string.IsNullOrEmpty(filters.Categoria))
            {
                query = query.Where(a => a.ID_CarroNavigation.Categoria == filters.Categoria);
            }

            if (filters.ID_Marca.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.ID_ModeloNavigation.ID_Marca == filters.ID_Marca.Value);
            }

            if (filters.ID_Modelo.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.ID_Modelo == filters.ID_Modelo.Value);
            }

            if (filters.Ano_Min.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.Ano >= filters.Ano_Min.Value);
            }

            if (filters.Ano_Max.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.Ano <= filters.Ano_Max.Value);
            }

            if (filters.Preco_Min.HasValue)
            {
                query = query.Where(a => a.Preco >= filters.Preco_Min.Value);
            }

            if (filters.Preco_Max.HasValue)
            {
                query = query.Where(a => a.Preco <= filters.Preco_Max.Value);
            }

            if (filters.Quilometragem_Min.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.Quilometragem >= filters.Quilometragem_Min.Value);
            }

            if (filters.Quilometragem_Max.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.Quilometragem <= filters.Quilometragem_Max.Value);
            }

            if (filters.ID_Combustivel.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.ID_Combustivel == filters.ID_Combustivel.Value);
            }

            if (!string.IsNullOrEmpty(filters.Caixa))
            {
                query = query.Where(a => a.ID_CarroNavigation.Caixa == filters.Caixa);
            }

            if (!string.IsNullOrEmpty(filters.Cor))
            {
                query = query.Where(a => a.ID_CarroNavigation.Cor == filters.Cor);
            }

            if (filters.Lotacao_Min.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.Lotacao >= filters.Lotacao_Min.Value);
            }

            if (filters.Lotacao_Max.HasValue)
            {
                query = query.Where(a => a.ID_CarroNavigation.Lotacao <= filters.Lotacao_Max.Value);
            }

            if (!string.IsNullOrEmpty(filters.Distrito))
            {
                query = query.Where(a => a.Localizacao != null && a.Localizacao.Contains(filters.Distrito));
            }

            // Apply sorting
            query = filters.SortBy switch
            {
                "preco_asc" => query.OrderBy(a => a.Preco),
                "preco_desc" => query.OrderByDescending(a => a.Preco),
                "quilometragem_asc" => query.OrderBy(a => a.ID_CarroNavigation.Quilometragem),
                "ano_desc" => query.OrderByDescending(a => a.ID_CarroNavigation.Ano),
                _ => query.OrderByDescending(a => a.Data_Publicacao) // data_desc (default)
            };

            // Get total count
            var totalResultados = await query.CountAsync();

            // Calculate pagination
            var totalPaginas = (int)Math.Ceiling(totalResultados / (double)pageSize);
            var paginaAtual = Math.Max(1, Math.Min(page, totalPaginas > 0 ? totalPaginas : 1));

            // Get paginated results
            var anuncios = await query
                .Skip((paginaAtual - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to ViewModels
            var resultados = new List<AnuncioViewModel>();
            foreach (var anuncio in anuncios)
            {
                resultados.Add(await MapToViewModelAsync(anuncio));
            }

            // Populate result
            filters.Resultados = resultados;
            filters.TotalResultados = totalResultados;
            filters.PaginaAtual = paginaAtual;
            filters.TotalPaginas = totalPaginas;
            filters.ItensPorPagina = pageSize;

            return filters;
        }

        public async Task<List<AnuncioViewModel>> GetRecentAnunciosAsync(int count = 6)
        {
            var anuncios = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_CombustivelNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Where(a => a.Estado_Anuncio == "Ativo")
                .OrderByDescending(a => a.Data_Publicacao)
                .Take(count)
                .ToListAsync();

            var result = new List<AnuncioViewModel>();
            foreach (var anuncio in anuncios)
            {
                result.Add(await MapToViewModelAsync(anuncio));
            }
            return result;
        }

        public async Task<List<AnuncioViewModel>> GetFeaturedAnunciosAsync(int count = 6)
        {
            // Featured: Ativo + preço acima da média
            var avgPrice = await _context.Anuncios
                .Where(a => a.Estado_Anuncio == "Ativo")
                .AverageAsync(a => (decimal?)a.Preco) ?? 0;

            var anuncios = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_CombustivelNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Where(a => a.Estado_Anuncio == "Ativo" && a.Preco >= avgPrice)
                .OrderByDescending(a => a.Preco)
                .Take(count)
                .ToListAsync();

            var result = new List<AnuncioViewModel>();
            foreach (var anuncio in anuncios)
            {
                result.Add(await MapToViewModelAsync(anuncio));
            }
            return result;
        }

        public async Task<AnuncioViewModel?> GetAnuncioDetailsAsync(int anuncioId)
        {
            var anuncio = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_CombustivelNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Include(a => a.ID_VendedorNavigation)
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId);

            return anuncio != null ? await MapToViewModelAsync(anuncio) : null;
        }

        // Helper method to map Anuncio to ViewModel
        private async Task<AnuncioViewModel> MapToViewModelAsync(Anuncio anuncio)
        {
            var carro = anuncio.ID_CarroNavigation;
            var modelo = carro.ID_ModeloNavigation;
            var marca = modelo.ID_MarcaNavigation;
            var combustivel = carro.ID_CombustivelNavigation;

            // Load vendedor data via UserManager if available
            string? nomeVendedor = null;
            string? contactoVendedor = null;
            if (anuncio.ID_VendedorNavigation != null)
            {
                var vendedorUser = await _userManager.FindByIdAsync(anuncio.ID_VendedorNavigation.ID_Utilizador.ToString());
                if (vendedorUser != null)
                {
                    nomeVendedor = vendedorUser.Nome;
                    contactoVendedor = vendedorUser.Contacto;
                }
            }

            return new AnuncioViewModel
            {
                ID_Anuncio = anuncio.ID_Anuncio,
                Titulo = anuncio.Titulo,
                Preco = anuncio.Preco,
                Localizacao = anuncio.Localizacao,
                Descricao = anuncio.Descricao,
                Data_Publicacao = anuncio.Data_Publicacao,
                Data_Atualizacao = anuncio.Data_Atualizacao,
                Estado_Anuncio = anuncio.Estado_Anuncio,
                Prazo_Reserva_Dias = anuncio.Prazo_Reserva_Dias,

                ID_Carro = carro.ID_Carro,
                Ano = carro.Ano,
                Quilometragem = carro.Quilometragem,
                Lotacao = carro.Lotacao,
                Categoria = carro.Categoria,
                Caixa = carro.Caixa,
                Cor = carro.Cor,

                MarcaNome = marca.Nome,
                ModeloNome = modelo.Nome,
                CombustivelTipo = combustivel.Tipo,

                Fotos = carro.Fotos.Select(f => new FotoViewModel
                {
                    ID = f.ID,
                    Fotografia = f.Fotografia,
                    Ordem = f.Ordem
                }).ToList(),

                Reservado_Por = anuncio.Reservado_Por,
                Reservado_Ate = anuncio.Reservado_Ate,

                ID_Modelo = carro.ID_Modelo,
                ID_Combustivel = carro.ID_Combustivel,
                ID_Vendedor = anuncio.ID_Vendedor,

                // Vendedor info (for detail views)
                NomeVendedor = nomeVendedor,
                ContactoVendedor = contactoVendedor
            };
        }
    }
}
