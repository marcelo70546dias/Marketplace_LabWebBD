using Microsoft.EntityFrameworkCore;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.ViewModels;

namespace Marketplace_LabWebBD.Services
{
    public class CompraService : ICompraService
    {
        private readonly ApplicationDbContext _context;

        public CompraService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CheckoutViewModel?> GetCheckoutDetailsAsync(int anuncioId)
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

            // Só pode comprar se estiver Ativo ou Reservado
            if (anuncio.Estado_Anuncio != "Ativo" && anuncio.Estado_Anuncio != "Reservado")
                return null;

            var carro = anuncio.ID_CarroNavigation;
            var modelo = carro?.ID_ModeloNavigation;
            var marca = modelo?.ID_MarcaNavigation;

            return new CheckoutViewModel
            {
                ID_Anuncio = anuncio.ID_Anuncio,
                Titulo = anuncio.Titulo ?? string.Empty,
                Preco = anuncio.Preco,
                Localizacao = anuncio.Localizacao ?? string.Empty,
                FotoPrincipal = carro?.Fotos.FirstOrDefault()?.Fotografia,
                MarcaModelo = $"{marca?.Nome} {modelo?.Nome}"
            };
        }

        public async Task<int?> CreateCompraAsync(CheckoutViewModel model, int compradorId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var anuncio = await _context.Anuncios.FindAsync(model.ID_Anuncio);

                if (anuncio == null)
                    return null;

                // Verificar se o anúncio está disponível
                if (anuncio.Estado_Anuncio != "Ativo" && anuncio.Estado_Anuncio != "Reservado")
                    return null;

                // Criar a compra
                var compra = new Compra
                {
                    ID_Comprador = compradorId,
                    ID_Anuncio = model.ID_Anuncio,
                    Data = DateOnly.FromDateTime(DateTime.Now),
                    Preco = anuncio.Preco,
                    Estado_Pagamento = "Pendente",
                    Metodo_Pagamento = model.Metodo_Pagamento,
                    Notas = model.Notas
                };

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                // Atualizar o anúncio para Vendido
                anuncio.Estado_Anuncio = "Vendido";
                anuncio.Reservado_Por = null;
                anuncio.Reservado_Ate = null;

                // Se havia uma reserva ativa, marcá-la como concluída
                var historicoReserva = await _context.Historico_Reservas
                    .Where(h => h.ID_Anuncio == model.ID_Anuncio && h.Estado == "Ativa")
                    .FirstOrDefaultAsync();

                if (historicoReserva != null)
                {
                    historicoReserva.Estado = "Concluida";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return compra.ID_Compra;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<OrderConfirmationViewModel?> GetOrderConfirmationAsync(int compraId, int compradorId)
        {
            var compra = await _context.Compras
                .Include(c => c.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(car => car.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(c => c.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(car => car.Fotos)
                .FirstOrDefaultAsync(c => c.ID_Compra == compraId && c.ID_Comprador == compradorId);

            if (compra == null)
                return null;

            var anuncio = compra.ID_AnuncioNavigation;
            var carro = anuncio.ID_CarroNavigation;
            var modelo = carro?.ID_ModeloNavigation;
            var marca = modelo?.ID_MarcaNavigation;

            var result = new OrderConfirmationViewModel
            {
                ID_Compra = compra.ID_Compra,
                Titulo = anuncio.Titulo ?? string.Empty,
                Preco = compra.Preco,
                FotoPrincipal = carro?.Fotos.FirstOrDefault()?.Fotografia,
                MarcaModelo = $"{marca?.Nome} {modelo?.Nome}",
                Data = compra.Data,
                Metodo_Pagamento = compra.Metodo_Pagamento ?? string.Empty,
                Estado_Pagamento = compra.Estado_Pagamento ?? "Pendente"
            };

            // Gerar dados de pagamento simulados baseados no método
            switch (compra.Metodo_Pagamento)
            {
                case "Multibanco":
                    result.EntidadeMB = "12345";
                    result.ReferenciaMB = GenerateRandomReference();
                    break;
                case "MB Way":
                    result.NumeroTelemovelMBWay = "***  ***  " + new Random().Next(100, 999).ToString();
                    break;
                case "Transferência Bancária":
                    result.IBAN = "PT50 0002 0123 " + new Random().Next(1000, 9999) + " " +
                                   new Random().Next(1000, 9999) + " " +
                                   new Random().Next(10, 99);
                    break;
            }

            return result;
        }

        public async Task<bool> SimulatePaymentAsync(int compraId, int compradorId)
        {
            try
            {
                var compra = await _context.Compras
                    .FirstOrDefaultAsync(c => c.ID_Compra == compraId && c.ID_Comprador == compradorId);

                if (compra == null || compra.Estado_Pagamento != "Pendente")
                    return false;

                compra.Estado_Pagamento = "Pago";
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CompraViewModel>> GetComprasByCompradorAsync(int compradorId)
        {
            var compras = await _context.Compras
                .Include(c => c.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(car => car.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(c => c.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(car => car.Fotos)
                .Include(c => c.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_VendedorNavigation)
                    .ThenInclude(v => v.ID_UtilizadorNavigation)
                .Where(c => c.ID_Comprador == compradorId)
                .OrderByDescending(c => c.Data)
                .Select(c => new CompraViewModel
                {
                    ID_Compra = c.ID_Compra,
                    ID_Anuncio = c.ID_Anuncio,
                    Titulo = c.ID_AnuncioNavigation.Titulo ?? string.Empty,
                    Preco = c.Preco,
                    FotoPrincipal = c.ID_AnuncioNavigation.ID_CarroNavigation!.Fotos.FirstOrDefault()!.Fotografia,
                    Data = c.Data,
                    Estado_Pagamento = c.Estado_Pagamento,
                    Metodo_Pagamento = c.Metodo_Pagamento,
                    Notas = c.Notas,
                    MarcaModelo = $"{c.ID_AnuncioNavigation.ID_CarroNavigation!.ID_ModeloNavigation!.ID_MarcaNavigation!.Nome} {c.ID_AnuncioNavigation.ID_CarroNavigation.ID_ModeloNavigation.Nome}",
                    VendedorNome = c.ID_AnuncioNavigation.ID_VendedorNavigation.ID_UtilizadorNavigation.Nome,
                    VendedorEmail = c.ID_AnuncioNavigation.ID_VendedorNavigation.ID_UtilizadorNavigation.Email,
                    VendedorContacto = c.ID_AnuncioNavigation.ID_VendedorNavigation.ID_UtilizadorNavigation.Contacto
                })
                .ToListAsync();

            return compras;
        }

        public async Task<List<CompraViewModel>> GetComprasByVendedorAsync(int vendedorId)
        {
            var compras = await _context.Compras
                .Include(c => c.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(car => car.ID_ModeloNavigation)
                    .ThenInclude(m => m.ID_MarcaNavigation)
                .Include(c => c.ID_AnuncioNavigation)
                    .ThenInclude(a => a.ID_CarroNavigation)
                    .ThenInclude(car => car.Fotos)
                .Include(c => c.ID_CompradorNavigation)
                    .ThenInclude(comp => comp.ID_UtilizadorNavigation)
                .Where(c => c.ID_AnuncioNavigation.ID_Vendedor == vendedorId)
                .OrderByDescending(c => c.Data)
                .Select(c => new CompraViewModel
                {
                    ID_Compra = c.ID_Compra,
                    ID_Anuncio = c.ID_Anuncio,
                    Titulo = c.ID_AnuncioNavigation.Titulo ?? string.Empty,
                    Preco = c.Preco,
                    FotoPrincipal = c.ID_AnuncioNavigation.ID_CarroNavigation!.Fotos.FirstOrDefault()!.Fotografia,
                    Data = c.Data,
                    Estado_Pagamento = c.Estado_Pagamento,
                    Metodo_Pagamento = c.Metodo_Pagamento,
                    Notas = c.Notas,
                    MarcaModelo = $"{c.ID_AnuncioNavigation.ID_CarroNavigation!.ID_ModeloNavigation!.ID_MarcaNavigation!.Nome} {c.ID_AnuncioNavigation.ID_CarroNavigation.ID_ModeloNavigation.Nome}",
                    CompradorNome = c.ID_CompradorNavigation.ID_UtilizadorNavigation.Nome,
                    CompradorEmail = c.ID_CompradorNavigation.ID_UtilizadorNavigation.Email,
                    CompradorContacto = c.ID_CompradorNavigation.ID_UtilizadorNavigation.Contacto
                })
                .ToListAsync();

            return compras;
        }

        public async Task<bool> CancelCompraAsync(int compraId, int compradorId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var compra = await _context.Compras
                    .Include(c => c.ID_AnuncioNavigation)
                    .FirstOrDefaultAsync(c => c.ID_Compra == compraId && c.ID_Comprador == compradorId);

                if (compra == null || compra.Estado_Pagamento != "Pendente")
                    return false;

                // Cancelar a compra
                compra.Estado_Pagamento = "Cancelado";

                // Voltar o anúncio para Ativo
                compra.ID_AnuncioNavigation.Estado_Anuncio = "Ativo";

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

        private string GenerateRandomReference()
        {
            var random = new Random();
            return $"{random.Next(100, 999)} {random.Next(100, 999)} {random.Next(100, 999)}";
        }
    }
}
