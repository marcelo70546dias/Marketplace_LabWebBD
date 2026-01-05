using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class AnuncioViewModel
    {
        public int ID_Anuncio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string? Localizacao { get; set; }
        public string? Descricao { get; set; }
        public DateTime Data_Publicacao { get; set; }
        public DateTime? Data_Atualizacao { get; set; }
        public string? Estado_Anuncio { get; set; }
        public int? Prazo_Reserva_Dias { get; set; }

        // Dados do Carro
        public int ID_Carro { get; set; }
        public int Ano { get; set; }
        public int Quilometragem { get; set; }
        public int? Lotacao { get; set; }
        public string? Categoria { get; set; }
        public string? Caixa { get; set; }
        public string? Cor { get; set; }

        // Marca/Modelo/Combustível (para display)
        public string MarcaNome { get; set; } = string.Empty;
        public string ModeloNome { get; set; } = string.Empty;
        public string CombustivelTipo { get; set; } = string.Empty;

        // Fotos
        public List<FotoViewModel> Fotos { get; set; } = new List<FotoViewModel>();
        public string? FotoPrincipal => Fotos.OrderBy(f => f.Ordem).FirstOrDefault()?.Fotografia;

        // Dados de Reserva (se aplicável)
        public int? Reservado_Por { get; set; }
        public DateOnly? Reservado_Ate { get; set; }
        public string? NomeComprador { get; set; }

        // IDs para edição
        public int ID_Modelo { get; set; }
        public int ID_Combustivel { get; set; }
        public int ID_Vendedor { get; set; }
    }

    public class FotoViewModel
    {
        public int ID { get; set; }
        public string Fotografia { get; set; } = string.Empty;
        public int? Ordem { get; set; }
    }
}
