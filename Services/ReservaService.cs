using Microsoft.EntityFrameworkCore;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Services
{
    public class ReservaService : IReservaService
    {
        private readonly ApplicationDbContext _context;

        public ReservaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CanReserveAsync(int anuncioId)
        {
            var anuncio = await _context.Anuncios.FindAsync(anuncioId);

            if (anuncio == null)
                return false;

            // Só pode reservar se estiver Ativo
            return anuncio.Estado_Anuncio == "Ativo";
        }

        public async Task<CreateReservaViewModel?> GetReservaDetailsAsync(int anuncioId)
        {
            var anuncio = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId);

            if (anuncio == null || anuncio.Estado_Anuncio != "Ativo")
                return null;

            var carro = anuncio.ID_CarroNavigation;
            var modelo = carro?.ID_ModeloNavigation;
            var marca = modelo?.ID_MarcaNavigation;

            var prazoReserva = anuncio.Prazo_Reserva_Dias ?? 7; // Default 7 dias
            var dataExpiracao = DateTime.Now.AddDays(prazoReserva);

            return new CreateReservaViewModel
            {
                ID_Anuncio = anuncio.ID_Anuncio,
                Titulo = anuncio.Titulo ?? string.Empty,
                Preco = anuncio.Preco,
                Localizacao = anuncio.Localizacao ?? string.Empty,
                FotoPrincipal = carro?.Fotos.FirstOrDefault()?.Fotografia,
                MarcaModelo = $"{marca?.Nome} {modelo?.Nome}",
                Prazo_Reserva_Dias = prazoReserva,
                Data_Expiracao = dataExpiracao
            };
        }

        public async Task<bool> CreateReservaAsync(int anuncioId, int compradorId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var anuncio = await _context.Anuncios.FindAsync(anuncioId);

                if (anuncio == null || anuncio.Estado_Anuncio != "Ativo")
                    return false;

                var prazoReserva = anuncio.Prazo_Reserva_Dias ?? 7;
                var dataExpiracao = DateTime.Now.AddDays(prazoReserva);

                // Atualizar estado do anúncio
                anuncio.Estado_Anuncio = "Reservado";
                anuncio.Reservado_Por = compradorId;
                anuncio.Reservado_Ate = DateOnly.FromDateTime(dataExpiracao);

                // Criar histórico de reserva
                var historicoReserva = new Historico_Reserva
                {
                    ID_Anuncio = anuncioId,
                    ID_Comprador = compradorId,
                    Data_Inicio = DateTime.Now,
                    Data_Fim = DateOnly.FromDateTime(dataExpiracao),
                    Estado = "Ativa"
                };

                _context.Historico_Reservas.Add(historicoReserva);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> CancelReservaAsync(int anuncioId, int compradorId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var anuncio = await _context.Anuncios.FindAsync(anuncioId);

                if (anuncio == null || anuncio.Reservado_Por != compradorId)
                    return false;

                // Voltar estado para Ativo
                anuncio.Estado_Anuncio = "Ativo";
                anuncio.Reservado_Por = null;
                anuncio.Reservado_Ate = null;

                // Atualizar histórico
                var historico = await _context.Historico_Reservas
                    .Where(h => h.ID_Anuncio == anuncioId && h.ID_Comprador == compradorId && h.Estado == "Ativa")
                    .FirstOrDefaultAsync();

                if (historico != null)
                {
                    historico.Estado = "Cancelada";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<ReservaViewModel>> GetReservasByCompradorAsync(int compradorId)
        {
            var reservas = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Where(a => a.Reservado_Por == compradorId && a.Estado_Anuncio == "Reservado")
                .Select(a => new ReservaViewModel
                {
                    ID_Anuncio = a.ID_Anuncio,
                    Titulo = a.Titulo ?? string.Empty,
                    Preco = a.Preco,
                    Localizacao = a.Localizacao ?? string.Empty,
                    FotoPrincipal = a.ID_CarroNavigation!.Fotos.FirstOrDefault()!.Fotografia,
                    Data_Inicio = DateTime.Now,
                    Data_Fim = a.Reservado_Ate.HasValue ? a.Reservado_Ate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Now,
                    DiasRestantes = a.Reservado_Ate.HasValue ? (a.Reservado_Ate.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days : 0,
                    Estado = a.Estado_Anuncio ?? string.Empty,
                    MarcaModelo = $"{a.ID_CarroNavigation!.ID_ModeloNavigation!.ID_MarcaNavigation!.Nome} {a.ID_CarroNavigation.ID_ModeloNavigation.Nome}"
                })
                .ToListAsync();

            return reservas;
        }

        public async Task<List<ReservaViewModel>> GetReservasByVendedorAsync(int vendedorId)
        {
            var reservas = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Include(a => a.Reservado_PorNavigation)
                    .ThenInclude(c => c!.ID_UtilizadorNavigation)
                .Where(a => a.ID_Vendedor == vendedorId && a.Estado_Anuncio == "Reservado")
                .Select(a => new ReservaViewModel
                {
                    ID_Anuncio = a.ID_Anuncio,
                    Titulo = a.Titulo ?? string.Empty,
                    Preco = a.Preco,
                    Localizacao = a.Localizacao ?? string.Empty,
                    FotoPrincipal = a.ID_CarroNavigation!.Fotos.FirstOrDefault()!.Fotografia,
                    Data_Inicio = DateTime.Now,
                    Data_Fim = a.Reservado_Ate.HasValue ? a.Reservado_Ate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Now,
                    DiasRestantes = a.Reservado_Ate.HasValue ? (a.Reservado_Ate.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days : 0,
                    Estado = a.Estado_Anuncio ?? string.Empty,
                    MarcaModelo = $"{a.ID_CarroNavigation!.ID_ModeloNavigation!.ID_MarcaNavigation!.Nome} {a.ID_CarroNavigation.ID_ModeloNavigation.Nome}",
                    CompradorNome = a.Reservado_PorNavigation != null ? a.Reservado_PorNavigation.ID_UtilizadorNavigation.Nome : null,
                    CompradorEmail = a.Reservado_PorNavigation != null ? a.Reservado_PorNavigation.ID_UtilizadorNavigation.Email : null,
                    CompradorContacto = a.Reservado_PorNavigation != null ? a.Reservado_PorNavigation.ID_UtilizadorNavigation.Contacto : null
                })
                .ToListAsync();

            return reservas;
        }

        public async Task ExpireReservasAsync()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);

            var reservasExpiradas = await _context.Anuncios
                .Where(a => a.Estado_Anuncio == "Reservado" && a.Reservado_Ate < hoje)
                .ToListAsync();

            foreach (var anuncio in reservasExpiradas)
            {
                // Voltar para Ativo
                anuncio.Estado_Anuncio = "Ativo";
                anuncio.Reservado_Por = null;
                anuncio.Reservado_Ate = null;

                // Atualizar histórico
                var historico = await _context.Historico_Reservas
                    .Where(h => h.ID_Anuncio == anuncio.ID_Anuncio && h.Estado == "Ativa")
                    .FirstOrDefaultAsync();

                if (historico != null)
                {
                    historico.Estado = "Expirada";
                }
            }

            if (reservasExpiradas.Any())
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"[ReservaService] {reservasExpiradas.Count} reservas expiradas foram libertadas.");
            }
        }
    }
}
