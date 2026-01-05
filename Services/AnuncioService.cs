using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_LabWebBD.Services
{
    public class AnuncioService : IAnuncioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;

        public AnuncioService(ApplicationDbContext context, IImageUploadService imageUploadService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
        }

        public async Task<List<AnuncioViewModel>> GetAnunciosByVendedorAsync(int vendedorId)
        {
            var anuncios = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_CombustivelNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Include(a => a.Reservado_PorNavigation)
                    .ThenInclude(c => c!.ID_UtilizadorNavigation)
                .Where(a => a.ID_Vendedor == vendedorId)
                .OrderByDescending(a => a.Data_Publicacao)
                .ToListAsync();

            return anuncios.Select(a => MapToViewModel(a)).ToList();
        }

        public async Task<AnuncioViewModel?> GetAnuncioByIdAsync(int anuncioId)
        {
            var anuncio = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_CombustivelNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Include(a => a.Reservado_PorNavigation)
                    .ThenInclude(c => c!.ID_UtilizadorNavigation)
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId);

            return anuncio != null ? MapToViewModel(anuncio) : null;
        }

        public async Task<AnuncioViewModel?> GetAnuncioDetailsAsync(int anuncioId, int vendedorId)
        {
            var anuncio = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_CombustivelNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Include(a => a.Reservado_PorNavigation)
                    .ThenInclude(c => c!.ID_UtilizadorNavigation)
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId && a.ID_Vendedor == vendedorId);

            return anuncio != null ? MapToViewModel(anuncio) : null;
        }

        public async Task<int> CreateAnuncioAsync(CreateAnuncioViewModel model, int vendedorId)
        {
            // Criar o Carro
            var carro = new Carro
            {
                ID_Vendedor = vendedorId,
                Ano = model.Ano,
                Quilometragem = model.Quilometragem,
                Categoria = model.Categoria,
                Caixa = model.Caixa,
                ID_Modelo = model.ID_Modelo,
                ID_Combustivel = model.ID_Combustivel
            };

            _context.Carros.Add(carro);
            await _context.SaveChangesAsync();

            // Criar o Anúncio
            var anuncio = new Anuncio
            {
                Titulo = model.Titulo,
                Preco = model.Preco,
                Localizacao = model.Localizacao,
                Descricao = model.Descricao,
                Data_Publicacao = DateTime.Now,
                Estado_Anuncio = "Ativo",
                Prazo_Reserva_Dias = 7, // Default 7 dias
                ID_Carro = carro.ID_Carro,
                ID_Vendedor = vendedorId
            };

            _context.Anuncios.Add(anuncio);
            await _context.SaveChangesAsync();

            return anuncio.ID_Anuncio;
        }

        public async Task<bool> UpdateAnuncioAsync(EditAnuncioViewModel model, int vendedorId)
        {
            var anuncio = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                .FirstOrDefaultAsync(a => a.ID_Anuncio == model.ID_Anuncio && a.ID_Vendedor == vendedorId);

            if (anuncio == null)
                return false;

            // Vendedor só pode editar anúncios Ativos ou Pausados
            if (anuncio.Estado_Anuncio != "Ativo" && anuncio.Estado_Anuncio != "Pausado")
                return false;

            // Atualizar Anúncio
            anuncio.Titulo = model.Titulo;
            anuncio.Preco = model.Preco;
            anuncio.Localizacao = model.Localizacao;
            anuncio.Descricao = model.Descricao;
            anuncio.Data_Atualizacao = DateTime.Now;

            // Permitir alternar entre Ativo e Pausado
            if (model.Estado_Anuncio == "Ativo" || model.Estado_Anuncio == "Pausado")
            {
                anuncio.Estado_Anuncio = model.Estado_Anuncio;
            }

            // Atualizar Carro
            var carro = anuncio.ID_CarroNavigation;
            carro.Ano = model.Ano;
            carro.Quilometragem = model.Quilometragem;
            carro.Categoria = model.Categoria;
            carro.Caixa = model.Caixa;
            carro.ID_Modelo = model.ID_Modelo;
            carro.ID_Combustivel = model.ID_Combustivel;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAnuncioAsync(int anuncioId, int vendedorId)
        {
            var anuncio = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId && a.ID_Vendedor == vendedorId);

            if (anuncio == null)
                return false;

            // Só pode eliminar se estiver Ativo ou Pausado (não Reservado ou Vendido)
            if (anuncio.Estado_Anuncio == "Reservado" || anuncio.Estado_Anuncio == "Vendido")
                return false;

            // Remover fotos fisicamente do servidor e da BD
            if (anuncio.ID_CarroNavigation.Fotos.Any())
            {
                foreach (var foto in anuncio.ID_CarroNavigation.Fotos)
                {
                    // Delete physical file
                    if (!string.IsNullOrEmpty(foto.Fotografia))
                    {
                        await _imageUploadService.DeleteImageAsync(foto.Fotografia);
                    }
                }
                _context.Fotos.RemoveRange(anuncio.ID_CarroNavigation.Fotos);
            }

            // Remover anúncio
            _context.Anuncios.Remove(anuncio);

            // Remover carro
            _context.Carros.Remove(anuncio.ID_CarroNavigation);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeEstadoAsync(int anuncioId, string novoEstado, int vendedorId)
        {
            var anuncio = await _context.Anuncios
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId && a.ID_Vendedor == vendedorId);

            if (anuncio == null)
                return false;

            // Vendedor só pode alternar entre Ativo e Pausado
            if ((anuncio.Estado_Anuncio == "Ativo" || anuncio.Estado_Anuncio == "Pausado") &&
                (novoEstado == "Ativo" || novoEstado == "Pausado"))
            {
                anuncio.Estado_Anuncio = novoEstado;
                anuncio.Data_Atualizacao = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> CanEditAnuncioAsync(int anuncioId, int vendedorId)
        {
            var anuncio = await _context.Anuncios
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId && a.ID_Vendedor == vendedorId);

            if (anuncio == null)
                return false;

            // Só pode editar se Ativo ou Pausado
            return anuncio.Estado_Anuncio == "Ativo" || anuncio.Estado_Anuncio == "Pausado";
        }

        public async Task<List<SelectListItem>> GetMarcasAsync()
        {
            var marcas = await _context.Marcas
                .OrderBy(m => m.Nome)
                .Select(m => new SelectListItem
                {
                    Value = m.ID_Marca.ToString(),
                    Text = m.Nome
                })
                .ToListAsync();

            return marcas;
        }

        public async Task<List<SelectListItem>> GetModelosByMarcaAsync(int marcaId)
        {
            var modelos = await _context.Modelos
                .Where(m => m.ID_Marca == marcaId)
                .OrderBy(m => m.Nome)
                .Select(m => new SelectListItem
                {
                    Value = m.ID_Modelo.ToString(),
                    Text = m.Nome
                })
                .ToListAsync();

            return modelos;
        }

        public async Task<List<SelectListItem>> GetCombustiveisAsync()
        {
            var combustiveis = await _context.Combustivels
                .OrderBy(c => c.Tipo)
                .Select(c => new SelectListItem
                {
                    Value = c.ID_Combustivel.ToString(),
                    Text = c.Tipo
                })
                .ToListAsync();

            return combustiveis;
        }

        public async Task<EditAnuncioViewModel?> GetAnuncioForEditAsync(int anuncioId, int vendedorId)
        {
            var anuncio = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId && a.ID_Vendedor == vendedorId);

            if (anuncio == null || !await CanEditAnuncioAsync(anuncioId, vendedorId))
                return null;

            var carro = anuncio.ID_CarroNavigation;
            var modelo = carro.ID_ModeloNavigation;

            var viewModel = new EditAnuncioViewModel
            {
                ID_Anuncio = anuncio.ID_Anuncio,
                ID_Carro = anuncio.ID_Carro,
                Titulo = anuncio.Titulo,
                Preco = anuncio.Preco,
                Localizacao = anuncio.Localizacao ?? string.Empty,
                Descricao = anuncio.Descricao,
                Estado_Anuncio = anuncio.Estado_Anuncio ?? "Ativo",
                Data_Publicacao = anuncio.Data_Publicacao,

                Ano = carro.Ano,
                Quilometragem = carro.Quilometragem,
                Categoria = carro.Categoria ?? string.Empty,
                Caixa = carro.Caixa ?? string.Empty,
                ID_Modelo = carro.ID_Modelo,
                ID_Combustivel = carro.ID_Combustivel,
                ID_Marca = modelo.ID_Marca,

                FotosExistentes = carro.Fotos.Select(f => new FotoViewModel
                {
                    ID = f.ID,
                    Fotografia = f.Fotografia,
                    Ordem = f.Ordem
                }).ToList(),

                Marcas = await GetMarcasAsync(),
                Modelos = await GetModelosByMarcaAsync(modelo.ID_Marca),
                Combustiveis = await GetCombustiveisAsync()
            };

            return viewModel;
        }

        public async Task<int> GetAnunciosAtivosByVendedorAsync(int vendedorId)
        {
            return await _context.Anuncios
                .Where(a => a.ID_Vendedor == vendedorId && a.Estado_Anuncio == "Ativo")
                .CountAsync();
        }

        public async Task<int> GetAnunciosReservadosByVendedorAsync(int vendedorId)
        {
            return await _context.Anuncios
                .Where(a => a.ID_Vendedor == vendedorId && a.Estado_Anuncio == "Reservado")
                .CountAsync();
        }

        public async Task<int> GetAnunciosVendidosByVendedorAsync(int vendedorId)
        {
            return await _context.Anuncios
                .Where(a => a.ID_Vendedor == vendedorId && a.Estado_Anuncio == "Vendido")
                .CountAsync();
        }

        // ==================== GESTÃO DE FOTOGRAFIAS ====================

        public async Task<bool> AddFotosAsync(int anuncioId, List<IFormFile> fotos, int vendedorId)
        {
            if (fotos == null || fotos.Count == 0)
                return true; // No photos to add, not an error

            // Verify ownership and get carro ID
            var anuncio = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId && a.ID_Vendedor == vendedorId);

            if (anuncio == null)
                return false;

            try
            {
                // Upload images
                var uploadedPaths = await _imageUploadService.UploadMultipleImagesAsync(fotos, "anuncios");

                // Get current max order
                var maxOrder = anuncio.ID_CarroNavigation.Fotos.Any()
                    ? anuncio.ID_CarroNavigation.Fotos.Max(f => f.Ordem ?? 0)
                    : 0;

                // Create Foto records
                var fotoEntities = uploadedPaths.Select((path, index) => new Foto
                {
                    ID_Carro = anuncio.ID_Carro,
                    Fotografia = path,
                    Ordem = maxOrder + index + 1
                }).ToList();

                _context.Fotos.AddRange(fotoEntities);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteFotoAsync(int fotoId, int vendedorId)
        {
            // Get foto with anuncio ownership check
            var foto = await _context.Fotos
                .Include(f => f.ID_CarroNavigation)
                    .ThenInclude(c => c.Anuncios)
                .FirstOrDefaultAsync(f => f.ID == fotoId);

            if (foto == null)
                return false;

            // Check if any anuncio of this car belongs to the vendedor
            var belongsToVendedor = foto.ID_CarroNavigation.Anuncios
                .Any(a => a.ID_Vendedor == vendedorId);

            if (!belongsToVendedor)
                return false;

            // Delete physical file
            if (!string.IsNullOrEmpty(foto.Fotografia))
            {
                await _imageUploadService.DeleteImageAsync(foto.Fotografia);
            }

            // Delete from database
            _context.Fotos.Remove(foto);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReorderFotosAsync(int anuncioId, List<int> fotoIds, int vendedorId)
        {
            // Verify ownership
            var anuncio = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId && a.ID_Vendedor == vendedorId);

            if (anuncio == null)
                return false;

            // Update order
            var fotos = anuncio.ID_CarroNavigation.Fotos.ToList();
            for (int i = 0; i < fotoIds.Count; i++)
            {
                var foto = fotos.FirstOrDefault(f => f.ID == fotoIds[i]);
                if (foto != null)
                {
                    foto.Ordem = i + 1;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Helper method para mapear Anuncio para ViewModel
        private AnuncioViewModel MapToViewModel(Anuncio anuncio)
        {
            var carro = anuncio.ID_CarroNavigation;
            var modelo = carro.ID_ModeloNavigation;
            var marca = modelo.ID_MarcaNavigation;
            var combustivel = carro.ID_CombustivelNavigation;

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
                NomeComprador = anuncio.Reservado_PorNavigation?.ID_UtilizadorNavigation?.Nome,

                ID_Modelo = carro.ID_Modelo,
                ID_Combustivel = carro.ID_Combustivel,
                ID_Vendedor = anuncio.ID_Vendedor
            };
        }
    }
}
