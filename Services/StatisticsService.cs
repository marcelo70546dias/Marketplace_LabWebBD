using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Marketplace_LabWebBD.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminStatisticsViewModel> GetAdminStatisticsAsync(int? months = 12)
        {
            var stats = new AdminStatisticsViewModel();

            // Contar compradores e vendedores
            stats.TotalCompradores = await _context.Compradors.CountAsync();
            stats.TotalVendedores = await _context.Vendedors.CountAsync();

            // Contar anúncios por estado
            stats.TotalAnunciosAtivos = await _context.Anuncios
                .CountAsync(a => a.Estado_Anuncio == "Ativo");

            stats.TotalAnunciosReservados = await _context.Anuncios
                .CountAsync(a => a.Estado_Anuncio == "Reservado");

            stats.TotalAnunciosVendidos = await _context.Anuncios
                .CountAsync(a => a.Estado_Anuncio == "Vendido");

            // Calcular valor total de vendas (compras pagas)
            var comprasPagas = await _context.Compras
                .Where(c => c.Estado_Pagamento == "Pago")
                .ToListAsync();

            stats.TotalVendasValor = comprasPagas.Sum(c => c.Preco);
            stats.ValorMedioPorVenda = comprasPagas.Any()
                ? comprasPagas.Average(c => c.Preco)
                : 0;

            // Contar novas contas este mês
            var inicioMesAtual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            stats.NovasContasEsteMes = await _context.Users
                .Where(u => u.Data_Registo >= DateOnly.FromDateTime(inicioMesAtual))
                .CountAsync();

            // Contar vendas este mês
            stats.VendasEsteMes = await _context.Compras
                .Where(c => c.Data >= DateOnly.FromDateTime(inicioMesAtual) &&
                            c.Estado_Pagamento == "Pago")
                .CountAsync();

            // Vendas por mês (últimos N meses)
            stats.VendasPorMes = await GetSalesByMonthAsync(months ?? 12);

            // Top 10 marcas
            stats.TopMarcas = await GetTopBrandsAsync();

            // Top 10 modelos
            stats.TopModelos = await GetTopModelsAsync();

            return stats;
        }

        private async Task<List<SalesByPeriodViewModel>> GetSalesByMonthAsync(int months)
        {
            var dataInicio = DateTime.Now.AddMonths(-months);
            var compras = await _context.Compras
                .Include(c => c.ID_AnuncioNavigation)
                .Where(c => c.Data >= DateOnly.FromDateTime(dataInicio) &&
                            c.Estado_Pagamento == "Pago")
                .ToListAsync();

            var vendasPorMes = compras
                .GroupBy(c => new {
                    Ano = c.Data.Year,
                    Mes = c.Data.Month
                })
                .Select(g => new SalesByPeriodViewModel
                {
                    Periodo = GetMonthName(g.Key.Mes) + "/" + g.Key.Ano,
                    NumeroVendas = g.Count(),
                    ValorTotal = g.Sum(c => c.Preco)
                })
                .OrderBy(v => v.Periodo)
                .ToList();

            // Garantir que todos os meses estão representados (mesmo os sem vendas)
            var resultado = new List<SalesByPeriodViewModel>();
            for (int i = months - 1; i >= 0; i--)
            {
                var data = DateTime.Now.AddMonths(-i);
                var periodo = GetMonthName(data.Month) + "/" + data.Year;

                var vendaMes = vendasPorMes.FirstOrDefault(v => v.Periodo == periodo);
                resultado.Add(vendaMes ?? new SalesByPeriodViewModel
                {
                    Periodo = periodo,
                    NumeroVendas = 0,
                    ValorTotal = 0
                });
            }

            return resultado;
        }

        private async Task<List<TopBrandViewModel>> GetTopBrandsAsync()
        {
            var anuncios = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c!.ID_ModeloNavigation)
                        .ThenInclude(m => m!.ID_MarcaNavigation)
                .ToListAsync();

            var topMarcas = anuncios
                .Where(a => a.ID_CarroNavigation?.ID_ModeloNavigation?.ID_MarcaNavigation != null)
                .GroupBy(a => a.ID_CarroNavigation!.ID_ModeloNavigation!.ID_MarcaNavigation!.Nome)
                .Select(g => new TopBrandViewModel
                {
                    NomeMarca = g.Key,
                    NumeroAnuncios = g.Count(),
                    NumeroVendas = g.Count(a => a.Estado_Anuncio == "Vendido")
                })
                .OrderByDescending(b => b.NumeroVendas)
                .ThenByDescending(b => b.NumeroAnuncios)
                .Take(10)
                .ToList();

            return topMarcas;
        }

        private async Task<List<TopModelViewModel>> GetTopModelsAsync()
        {
            var anuncios = await _context.Anuncios
                .Include(a => a.ID_CarroNavigation)
                    .ThenInclude(c => c!.ID_ModeloNavigation)
                        .ThenInclude(m => m!.ID_MarcaNavigation)
                .ToListAsync();

            var topModelos = anuncios
                .Where(a => a.ID_CarroNavigation?.ID_ModeloNavigation != null &&
                           a.ID_CarroNavigation.ID_ModeloNavigation.ID_MarcaNavigation != null)
                .GroupBy(a => new
                {
                    Marca = a.ID_CarroNavigation!.ID_ModeloNavigation!.ID_MarcaNavigation!.Nome,
                    Modelo = a.ID_CarroNavigation!.ID_ModeloNavigation!.Nome
                })
                .Select(g => new TopModelViewModel
                {
                    NomeMarca = g.Key.Marca,
                    NomeModelo = g.Key.Modelo,
                    NumeroAnuncios = g.Count(),
                    NumeroVendas = g.Count(a => a.Estado_Anuncio == "Vendido")
                })
                .OrderByDescending(m => m.NumeroVendas)
                .ThenByDescending(m => m.NumeroAnuncios)
                .Take(10)
                .ToList();

            return topModelos;
        }

        private string GetMonthName(int month)
        {
            var culture = new CultureInfo("pt-PT");
            return culture.DateTimeFormat.GetAbbreviatedMonthName(month);
        }
    }
}
