using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class FiltroFavoritoViewModel
    {
        public int ID_Filtro { get; set; }

        [Required(ErrorMessage = "O nome do filtro é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do filtro não pode ter mais de 100 caracteres")]
        [Display(Name = "Nome do Filtro")]
        public string Nome_Filtro { get; set; } = string.Empty;

        public string? Categoria { get; set; }
        public int? ID_Marca { get; set; }
        public int? ID_Modelo { get; set; }
        public int? Ano_Min { get; set; }
        public int? Ano_Max { get; set; }
        public decimal? Preco_Min { get; set; }
        public decimal? Preco_Max { get; set; }
        public int? Quilometragem_Min { get; set; }
        public int? Quilometragem_Max { get; set; }
        public int? ID_Combustivel { get; set; }
        public string? Caixa { get; set; }
        public string? Cor { get; set; }
        public int? Lotacao_Min { get; set; }
        public int? Lotacao_Max { get; set; }
        public string? Localizacao { get; set; }
        public DateTime? Data_Criacao { get; set; }

        // Display properties
        public string? MarcaNome { get; set; }
        public string? ModeloNome { get; set; }
        public string? CombustivelNome { get; set; }
        public int NumResultados { get; set; }

        // Helper para descrição legível do filtro
        public string GetDescricao()
        {
            var descricoes = new List<string>();

            if (!string.IsNullOrEmpty(Categoria))
                descricoes.Add($"Categoria: {Categoria}");

            if (!string.IsNullOrEmpty(MarcaNome))
                descricoes.Add($"Marca: {MarcaNome}");

            if (!string.IsNullOrEmpty(ModeloNome))
                descricoes.Add($"Modelo: {ModeloNome}");

            if (Ano_Min.HasValue && Ano_Max.HasValue)
                descricoes.Add($"Ano: {Ano_Min}-{Ano_Max}");
            else if (Ano_Min.HasValue)
                descricoes.Add($"Ano mín: {Ano_Min}");
            else if (Ano_Max.HasValue)
                descricoes.Add($"Ano máx: {Ano_Max}");

            if (Preco_Min.HasValue && Preco_Max.HasValue)
                descricoes.Add($"Preço: €{Preco_Min:N0}-€{Preco_Max:N0}");
            else if (Preco_Min.HasValue)
                descricoes.Add($"Preço mín: €{Preco_Min:N0}");
            else if (Preco_Max.HasValue)
                descricoes.Add($"Preço máx: €{Preco_Max:N0}");

            if (Quilometragem_Min.HasValue && Quilometragem_Max.HasValue)
                descricoes.Add($"KM: {Quilometragem_Min:N0}-{Quilometragem_Max:N0} km");
            else if (Quilometragem_Min.HasValue)
                descricoes.Add($"KM mín: {Quilometragem_Min:N0} km");
            else if (Quilometragem_Max.HasValue)
                descricoes.Add($"KM máx: {Quilometragem_Max:N0} km");

            if (!string.IsNullOrEmpty(CombustivelNome))
                descricoes.Add($"Combustível: {CombustivelNome}");

            if (!string.IsNullOrEmpty(Caixa))
                descricoes.Add($"Caixa: {Caixa}");

            if (!string.IsNullOrEmpty(Cor))
                descricoes.Add($"Cor: {Cor}");

            if (Lotacao_Min.HasValue && Lotacao_Max.HasValue)
                descricoes.Add($"Lotação: {Lotacao_Min}-{Lotacao_Max} lugares");
            else if (Lotacao_Min.HasValue)
                descricoes.Add($"Lotação mín: {Lotacao_Min} lugares");
            else if (Lotacao_Max.HasValue)
                descricoes.Add($"Lotação máx: {Lotacao_Max} lugares");

            if (!string.IsNullOrEmpty(Localizacao))
                descricoes.Add($"Localização: {Localizacao}");

            return descricoes.Any() ? string.Join(" | ", descricoes) : "Sem filtros específicos";
        }
    }
}
