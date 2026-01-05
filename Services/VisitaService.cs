using Microsoft.EntityFrameworkCore;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Marketplace_LabWebBD.Services
{
    public class VisitaService : IVisitaService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VisitaService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<CreateVisitaViewModel?> GetVisitaDetailsAsync(int anuncioId)
        {
            var anuncio = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .FirstOrDefaultAsync(a => a.ID_Anuncio == anuncioId);

            if (anuncio == null)
                return null;

            // Visitas podem ser agendadas se o anúncio está Ativo ou Reservado
            if (anuncio.Estado_Anuncio != "Ativo" && anuncio.Estado_Anuncio != "Reservado")
                return null;

            var carro = anuncio.ID_CarroNavigation;
            var modelo = carro?.ID_ModeloNavigation;
            var marca = modelo?.ID_MarcaNavigation;

            return new CreateVisitaViewModel
            {
                ID_Anuncio = anuncio.ID_Anuncio,
                Titulo = anuncio.Titulo ?? string.Empty,
                Preco = anuncio.Preco,
                FotoPrincipal = carro?.Fotos.FirstOrDefault()?.Fotografia,
                MarcaModelo = $"{marca?.Nome} {modelo?.Nome}",
                Localizacao = anuncio.Localizacao ?? string.Empty,
                Data_Hora = DateTime.Now.AddDays(1) // Default: amanhã
            };
        }

        public async Task<bool> CreateVisitaAsync(CreateVisitaViewModel model, int compradorId)
        {
            try
            {
                // Validar que a data é futura (pelo menos 30 minutos no futuro)
                if (model.Data_Hora <= DateTime.Now.AddMinutes(30))
                    return false;

                // Validar ID_Anuncio
                if (model.ID_Anuncio <= 0)
                    return false;

                var anuncio = await _context.Anuncios.FindAsync(model.ID_Anuncio);
                if (anuncio == null)
                    return false;

                // Verificar que o anúncio está disponível para visitas
                if (anuncio.Estado_Anuncio != "Ativo" && anuncio.Estado_Anuncio != "Reservado")
                    return false;

                // Verificar se o comprador existe
                var compradorExists = await _context.Compradors.AnyAsync(c => c.ID_Comprador == compradorId);
                if (!compradorExists)
                    return false;

                // Verificar se já existe uma visita pendente/confirmada do mesmo comprador para o mesmo anúncio
                var visitaExistente = await _context.Visita
                    .AnyAsync(v => v.ID_Comprador == compradorId
                              && v.ID_Anuncio == model.ID_Anuncio
                              && (v.Estado == "Pendente" || v.Estado == "Confirmada"));

                if (visitaExistente)
                    return false;

                var visita = new Visita
                {
                    ID_Comprador = compradorId,
                    ID_Anuncio = model.ID_Anuncio,
                    Data_Hora = model.Data_Hora,
                    Localizacao = model.Localizacao ?? anuncio.Localizacao ?? "A definir",
                    Observacoes = model.Observacoes,
                    Estado = "Pendente",  // Estado inicial - aguarda aprovação do vendedor
                    Data_Criacao = DateTime.Now
                };

                _context.Visita.Add(visita);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<VisitaViewModel>> GetVisitasByCompradorAsync(int compradorId)
        {
            var visitas = await _context.Visita
                .Include(v => v.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(v => v.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Where(v => v.ID_Comprador == compradorId)
                .OrderByDescending(v => v.Data_Hora)
                .Select(v => new VisitaViewModel
                {
                    ID_Visita = v.ID_Visita,
                    ID_Anuncio = v.ID_Anuncio,
                    Titulo = v.ID_AnuncioNavigation.Titulo ?? string.Empty,
                    Preco = v.ID_AnuncioNavigation.Preco,
                    FotoPrincipal = v.ID_AnuncioNavigation.ID_CarroNavigation!.Fotos.FirstOrDefault()!.Fotografia,
                    Data_Hora = v.Data_Hora,
                    Localizacao = v.Localizacao ?? string.Empty,
                    Observacoes = v.Observacoes,
                    Estado = v.Estado,
                    MarcaModelo = $"{v.ID_AnuncioNavigation.ID_CarroNavigation!.ID_ModeloNavigation!.ID_MarcaNavigation!.Nome} {v.ID_AnuncioNavigation.ID_CarroNavigation.ID_ModeloNavigation.Nome}"
                })
                .ToListAsync();

            return visitas;
        }

        public async Task<List<VisitaViewModel>> GetVisitasByVendedorAsync(int vendedorId)
        {
            var visitasData = await _context.Visita
                .Include(v => v.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(v => v.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c.Fotos)
                .Include(v => v.ID_CompradorNavigation)
                .Where(v => v.ID_AnuncioNavigation.ID_Vendedor == vendedorId)
                .OrderByDescending(v => v.Data_Hora)
                .ToListAsync();

            var visitas = new List<VisitaViewModel>();

            foreach (var v in visitasData)
            {
                // Carregar dados do comprador via UserManager
                var compradorUser = await _userManager.FindByIdAsync(v.ID_CompradorNavigation.ID_Utilizador.ToString());

                visitas.Add(new VisitaViewModel
                {
                    ID_Visita = v.ID_Visita,
                    ID_Anuncio = v.ID_Anuncio,
                    Titulo = v.ID_AnuncioNavigation.Titulo ?? string.Empty,
                    Preco = v.ID_AnuncioNavigation.Preco,
                    FotoPrincipal = v.ID_AnuncioNavigation.ID_CarroNavigation?.Fotos.FirstOrDefault()?.Fotografia,
                    Data_Hora = v.Data_Hora,
                    Localizacao = v.Localizacao ?? string.Empty,
                    Observacoes = v.Observacoes,
                    Estado = v.Estado,
                    MarcaModelo = $"{v.ID_AnuncioNavigation.ID_CarroNavigation?.ID_ModeloNavigation?.ID_MarcaNavigation?.Nome} {v.ID_AnuncioNavigation.ID_CarroNavigation?.ID_ModeloNavigation?.Nome}",
                    CompradorNome = compradorUser?.Nome,
                    CompradorEmail = compradorUser?.Email,
                    CompradorContacto = compradorUser?.Contacto
                });
            }

            return visitas;
        }

        public async Task<bool> CancelVisitaAsync(int visitaId, int userId, string userRole)
        {
            try
            {
                var visita = await _context.Visita
                    .Include(v => v.ID_AnuncioNavigation)
                    .Include(v => v.ID_CompradorNavigation)
                    .FirstOrDefaultAsync(v => v.ID_Visita == visitaId);

                // Permitir cancelar visitas em "Pendente" ou "Confirmada"
                if (visita == null || (visita.Estado != "Pendente" && visita.Estado != "Confirmada"))
                    return false;

                // Verificar permissões
                if (userRole == "Comprador")
                {
                    var comprador = await _context.Compradors.FirstOrDefaultAsync(c => c.ID_Utilizador == userId);
                    if (comprador == null || visita.ID_Comprador != comprador.ID_Comprador)
                        return false;
                }
                else if (userRole == "Vendedor")
                {
                    var vendedor = await _context.Vendedors.FirstOrDefaultAsync(v => v.ID_Utilizador == userId);
                    if (vendedor == null || visita.ID_AnuncioNavigation.ID_Vendedor != vendedor.ID_Utilizador)
                        return false;
                }
                else
                {
                    return false;
                }

                visita.Estado = "Cancelada";
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkVisitaRealizadaAsync(int visitaId, int vendedorId)
        {
            try
            {
                var visita = await _context.Visita
                    .Include(v => v.ID_AnuncioNavigation)
                    .FirstOrDefaultAsync(v => v.ID_Visita == visitaId);

                // Só pode marcar como realizada se já foi confirmada
                if (visita == null || visita.Estado != "Confirmada")
                    return false;

                // Verificar que o anúncio pertence ao vendedor
                if (visita.ID_AnuncioNavigation.ID_Vendedor != vendedorId)
                    return false;

                visita.Estado = "Realizada";
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ========== NOVOS MÉTODOS PARA APROVAÇÃO ==========

        public async Task<bool> AprovarVisitaAsync(int visitaId, int vendedorId)
        {
            try
            {
                var visita = await _context.Visita
                    .Include(v => v.ID_AnuncioNavigation)
                    .FirstOrDefaultAsync(v => v.ID_Visita == visitaId);

                if (visita == null || visita.Estado != "Pendente")
                    return false;

                // Verificar que o anúncio pertence ao vendedor
                if (visita.ID_AnuncioNavigation.ID_Vendedor != vendedorId)
                    return false;

                // Verificar se a data ainda é futura
                if (visita.Data_Hora <= DateTime.Now)
                    return false;

                visita.Estado = "Confirmada";  // Usar "Confirmada" em vez de "Aprovada" para a BD
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AprovarVisita] ERRO: {ex.Message}");
                Console.WriteLine($"[AprovarVisita] Inner: {ex.InnerException?.Message}");
                return false;
            }
        }

        public async Task<bool> RecusarVisitaAsync(int visitaId, int vendedorId, string? motivo)
        {
            try
            {
                var visita = await _context.Visita
                    .Include(v => v.ID_AnuncioNavigation)
                    .FirstOrDefaultAsync(v => v.ID_Visita == visitaId);

                if (visita == null || visita.Estado != "Pendente")
                    return false;

                // Verificar que o anúncio pertence ao vendedor
                if (visita.ID_AnuncioNavigation.ID_Vendedor != vendedorId)
                    return false;

                visita.Estado = "Recusada";
                if (!string.IsNullOrEmpty(motivo))
                {
                    visita.Observacoes = $"[Recusada pelo vendedor] {motivo}";
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetNumVisitasPendentesAprovacaoAsync(int vendedorId)
        {
            return await _context.Visita
                .Include(v => v.ID_AnuncioNavigation)
                .Where(v => v.ID_AnuncioNavigation.ID_Vendedor == vendedorId
                       && v.Estado == "Pendente")
                .CountAsync();
        }
    }
}
