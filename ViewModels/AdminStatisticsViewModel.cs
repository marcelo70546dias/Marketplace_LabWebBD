namespace Marketplace_LabWebBD.ViewModels
{
    public class AdminStatisticsViewModel
    {
        public int TotalCompradores { get; set; }
        public int TotalVendedores { get; set; }
        public int TotalAnunciosAtivos { get; set; }
        public int TotalAnunciosReservados { get; set; }
        public int TotalAnunciosVendidos { get; set; }
        public decimal TotalVendasValor { get; set; }
        public decimal ValorMedioPorVenda { get; set; }
        public int NovasContasEsteMes { get; set; }
        public int VendasEsteMes { get; set; }

        public List<SalesByPeriodViewModel> VendasPorMes { get; set; } = new();
        public List<TopBrandViewModel> TopMarcas { get; set; } = new();
        public List<TopModelViewModel> TopModelos { get; set; } = new();
    }

    public class SalesByPeriodViewModel
    {
        public string Periodo { get; set; } = string.Empty; // Ex: "Jan/2025"
        public int NumeroVendas { get; set; }
        public decimal ValorTotal { get; set; }
    }

    public class TopBrandViewModel
    {
        public string NomeMarca { get; set; } = string.Empty;
        public int NumeroAnuncios { get; set; }
        public int NumeroVendas { get; set; }
    }

    public class TopModelViewModel
    {
        public string NomeMarca { get; set; } = string.Empty;
        public string NomeModelo { get; set; } = string.Empty;
        public int NumeroAnuncios { get; set; }
        public int NumeroVendas { get; set; }
    }
}
