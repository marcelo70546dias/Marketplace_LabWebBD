using Microsoft.EntityFrameworkCore;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Marketplace_LabWebBD.Services
{
    public class ReservaService : IReservaService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservaService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> CanReserveAsync(int anuncioId, int compradorId)
        {
            var anuncio = await _context.Anuncios.FindAsync(anuncioId);

            if (anuncio == null)
                return false;

            // Só pode reservar se estiver Ativo
            if (anuncio.Estado_Anuncio != "Ativo")
                return false;

            // Verificar se já tem uma reserva pendente ou confirmada para este anúncio
            var reservaExistente = await _context.Historico_Reservas
                .AnyAsync(h => h.ID_Comprador == compradorId
                          && h.ID_Anuncio == anuncioId
                          && (h.Estado == "Pendente" || h.Estado == "Confirmada"));

            return !reservaExistente;
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

        public async Task<(bool Success, string? ErrorMessage)> CreateReservaAsync(int anuncioId, int compradorId)
        {
            try
            {
                var anuncio = await _context.Anuncios.FindAsync(anuncioId);

                if (anuncio == null)
                    return (false, "Anúncio não encontrado");

                if (anuncio.Estado_Anuncio != "Ativo")
                    return (false, $"Estado do anúncio é '{anuncio.Estado_Anuncio}', esperado 'Ativo'");

                // Verificar se já tem uma reserva pendente/confirmada para este anúncio
                var reservaExistente = await _context.Historico_Reservas
                    .AnyAsync(h => h.ID_Comprador == compradorId
                              && h.ID_Anuncio == anuncioId
                              && (h.Estado == "Pendente" || h.Estado == "Confirmada"));

                if (reservaExistente)
                    return (false, "Já existe reserva pendente/confirmada para este anúncio");

                var prazoReserva = anuncio.Prazo_Reserva_Dias ?? 7;
                var dataExpiracao = DateTime.Now.AddDays(prazoReserva);

                var historicoReserva = new Historico_Reserva
                {
                    ID_Anuncio = anuncioId,
                    ID_Comprador = compradorId,
                    Data_Inicio = DateTime.Now,
                    Data_Fim = DateOnly.FromDateTime(dataExpiracao),
                    Estado = "Pendente"
                };

                _context.Historico_Reservas.Add(historicoReserva);
                await _context.SaveChangesAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                return (false, $"Erro BD: {innerMsg}");
            }
        }

        public async Task<bool> CancelReservaAsync(int historicoId, int userId, string userRole)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var historico = await _context.Historico_Reservas
                    .Include(h => h.ID_AnuncioNavigation)
                    .Include(h => h.ID_CompradorNavigation)
                    .FirstOrDefaultAsync(h => h.ID_Historico == historicoId);

                if (historico == null)
                    return false;

                // Só pode cancelar se estiver "Pendente" ou "Confirmada"
                if (historico.Estado != "Pendente" && historico.Estado != "Confirmada")
                    return false;

                // Verificar permissões
                if (userRole == "Comprador")
                {
                    var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == userId);
                    if (comprador == null || historico.ID_Comprador != comprador.ID_Comprador)
                        return false;
                }
                else if (userRole == "Vendedor")
                {
                    var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == userId);
                    if (vendedor == null || historico.ID_AnuncioNavigation.ID_Vendedor != vendedor.ID_Utilizador)
                        return false;
                }
                else
                {
                    return false;
                }

                var anuncio = historico.ID_AnuncioNavigation;

                // Se a reserva estava confirmada, voltar o anúncio para Ativo
                if (historico.Estado == "Confirmada" && anuncio.Estado_Anuncio == "Reservado")
                {
                    anuncio.Estado_Anuncio = "Ativo";
                    anuncio.Reservado_Por = null;
                    anuncio.Reservado_Ate = null;
                }

                historico.Estado = "Cancelada";
                historico.Data_Resposta = DateTime.Now;

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
            var historicos = await _context.Historico_Reservas
                .Include(h => h.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(h => h.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Where(h => h.ID_Comprador == compradorId)
                .OrderByDescending(h => h.Data_Inicio)
                .ToListAsync();

            return historicos.Select(h => new ReservaViewModel
            {
                ID_Historico = h.ID_Historico,
                ID_Anuncio = h.ID_Anuncio,
                Titulo = h.ID_AnuncioNavigation.Titulo ?? string.Empty,
                Preco = h.ID_AnuncioNavigation.Preco,
                Localizacao = h.ID_AnuncioNavigation.Localizacao ?? string.Empty,
                FotoPrincipal = h.ID_AnuncioNavigation.ID_CarroNavigation?.Fotos.FirstOrDefault()?.Fotografia,
                Data_Inicio = h.Data_Inicio,
                Data_Fim = h.Data_Fim.HasValue ? h.Data_Fim.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Now,
                DiasRestantes = h.Data_Fim.HasValue ? Math.Max(0, (h.Data_Fim.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days) : 0,
                Estado = h.Estado ?? string.Empty,
                MotivoRecusa = h.Motivo_Recusa,
                MarcaModelo = $"{h.ID_AnuncioNavigation.ID_CarroNavigation?.ID_ModeloNavigation?.ID_MarcaNavigation?.Nome} {h.ID_AnuncioNavigation.ID_CarroNavigation?.ID_ModeloNavigation?.Nome}"
            }).ToList();
        }

        public async Task<List<ReservaViewModel>> GetReservasByVendedorAsync(int vendedorId)
        {
            var historicos = await _context.Historico_Reservas
                .Include(h => h.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(h => h.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Include(h => h.ID_CompradorNavigation)
                .Where(h => h.ID_AnuncioNavigation.ID_Vendedor == vendedorId)
                .OrderByDescending(h => h.Data_Inicio)
                .ToListAsync();

            var reservas = new List<ReservaViewModel>();

            foreach (var h in historicos)
            {
                // Carregar dados do comprador via UserManager
                ApplicationUser? compradorUser = null;
                if (h.ID_CompradorNavigation != null)
                {
                    compradorUser = await _userManager.FindByIdAsync(h.ID_CompradorNavigation.ID_Utilizador.ToString());
                }

                reservas.Add(new ReservaViewModel
                {
                    ID_Historico = h.ID_Historico,
                    ID_Anuncio = h.ID_Anuncio,
                    Titulo = h.ID_AnuncioNavigation.Titulo ?? string.Empty,
                    Preco = h.ID_AnuncioNavigation.Preco,
                    Localizacao = h.ID_AnuncioNavigation.Localizacao ?? string.Empty,
                    FotoPrincipal = h.ID_AnuncioNavigation.ID_CarroNavigation?.Fotos.FirstOrDefault()?.Fotografia,
                    Data_Inicio = h.Data_Inicio,
                    Data_Fim = h.Data_Fim.HasValue ? h.Data_Fim.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Now,
                    DiasRestantes = h.Data_Fim.HasValue ? Math.Max(0, (h.Data_Fim.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days) : 0,
                    Estado = h.Estado ?? string.Empty,
                    MotivoRecusa = h.Motivo_Recusa,
                    MarcaModelo = $"{h.ID_AnuncioNavigation.ID_CarroNavigation?.ID_ModeloNavigation?.ID_MarcaNavigation?.Nome} {h.ID_AnuncioNavigation.ID_CarroNavigation?.ID_ModeloNavigation?.Nome}",
                    CompradorNome = compradorUser?.Nome,
                    CompradorEmail = compradorUser?.Email,
                    CompradorContacto = compradorUser?.Contacto
                });
            }

            return reservas;
        }

        public async Task ExpireReservasAsync()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);

            // Expirar reservas confirmadas que passaram da data
            var reservasExpiradas = await _context.Historico_Reservas
                .Include(h => h.ID_AnuncioNavigation)
                .Where(h => h.Estado == "Confirmada" && h.Data_Fim < hoje)
                .ToListAsync();

            foreach (var historico in reservasExpiradas)
            {
                var anuncio = historico.ID_AnuncioNavigation;

                // Voltar para Ativo
                if (anuncio.Estado_Anuncio == "Reservado")
                {
                    anuncio.Estado_Anuncio = "Ativo";
                    anuncio.Reservado_Por = null;
                    anuncio.Reservado_Ate = null;
                }

                historico.Estado = "Expirada";
            }

            // Também expirar pedidos de reserva que ficaram pendentes por mais de 7 dias
            var dataLimite = DateTime.Now.AddDays(-7);
            var pedidosPendentesAntigos = await _context.Historico_Reservas
                .Where(h => h.Estado == "Pendente" && h.Data_Inicio < dataLimite)
                .ToListAsync();

            foreach (var pedido in pedidosPendentesAntigos)
            {
                pedido.Estado = "Expirada";
                pedido.Motivo_Recusa = "Pedido expirado por falta de resposta do vendedor.";
            }

            if (reservasExpiradas.Any() || pedidosPendentesAntigos.Any())
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"[ReservaService] {reservasExpiradas.Count} reservas e {pedidosPendentesAntigos.Count} pedidos expirados.");
            }
        }

        // ========== NOVOS MÉTODOS PARA APROVAÇÃO ==========

        public async Task<bool> AprovarReservaAsync(int historicoId, int vendedorId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var historico = await _context.Historico_Reservas
                    .Include(h => h.ID_AnuncioNavigation)
                    .FirstOrDefaultAsync(h => h.ID_Historico == historicoId);

                if (historico == null || historico.Estado != "Pendente")
                    return false;

                var anuncio = historico.ID_AnuncioNavigation;

                // Verificar que o anúncio pertence ao vendedor
                if (anuncio.ID_Vendedor != vendedorId)
                    return false;

                // Verificar que o anúncio ainda está disponível
                if (anuncio.Estado_Anuncio != "Ativo")
                    return false;

                // Aprovar a reserva
                historico.Estado = "Confirmada";
                historico.Data_Resposta = DateTime.Now;

                // Atualizar o anúncio para Reservado
                var prazoReserva = anuncio.Prazo_Reserva_Dias ?? 7;
                anuncio.Estado_Anuncio = "Reservado";
                anuncio.Reservado_Por = historico.ID_Comprador;
                anuncio.Reservado_Ate = DateOnly.FromDateTime(DateTime.Now.AddDays(prazoReserva));

                // Atualizar data de fim da reserva
                historico.Data_Fim = anuncio.Reservado_Ate;

                // Recusar automaticamente outros pedidos pendentes para o mesmo anúncio
                var outrosPedidos = await _context.Historico_Reservas
                    .Where(h => h.ID_Anuncio == anuncio.ID_Anuncio
                           && h.ID_Historico != historicoId
                           && h.Estado == "Pendente")
                    .ToListAsync();

                foreach (var outroPedido in outrosPedidos)
                {
                    outroPedido.Estado = "Recusada";
                    outroPedido.Motivo_Recusa = "Veículo reservado por outro comprador.";
                    outroPedido.Data_Resposta = DateTime.Now;
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

        public async Task<bool> RecusarReservaAsync(int historicoId, int vendedorId, string? motivo)
        {
            try
            {
                var historico = await _context.Historico_Reservas
                    .Include(h => h.ID_AnuncioNavigation)
                    .FirstOrDefaultAsync(h => h.ID_Historico == historicoId);

                if (historico == null || historico.Estado != "Pendente")
                    return false;

                // Verificar que o anúncio pertence ao vendedor
                if (historico.ID_AnuncioNavigation.ID_Vendedor != vendedorId)
                    return false;

                historico.Estado = "Recusada";
                historico.Motivo_Recusa = motivo;
                historico.Data_Resposta = DateTime.Now;

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetNumReservasPendentesAprovacaoAsync(int vendedorId)
        {
            return await _context.Historico_Reservas
                .Include(h => h.ID_AnuncioNavigation)
                .Where(h => h.ID_AnuncioNavigation.ID_Vendedor == vendedorId
                       && h.Estado == "Pendente")
                .CountAsync();
        }

        public async Task<bool> CanBuyAsync(int anuncioId, int compradorId)
        {
            var anuncio = await _context.Anuncios.FindAsync(anuncioId);

            if (anuncio == null)
                return false;

            // Se está Ativo, qualquer um pode comprar
            if (anuncio.Estado_Anuncio == "Ativo")
                return true;

            // Se está Reservado, só quem reservou pode comprar
            if (anuncio.Estado_Anuncio == "Reservado")
            {
                return anuncio.Reservado_Por == compradorId;
            }

            // Se está Vendido ou outro estado, ninguém pode comprar
            return false;
        }
    }
}
