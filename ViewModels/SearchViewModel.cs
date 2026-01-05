using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class SearchViewModel
    {
        // ==================== FILTROS ====================

        [Display(Name = "Categoria")]
        public string? Categoria { get; set; }

        [Display(Name = "Marca")]
        public int? ID_Marca { get; set; }

        [Display(Name = "Modelo")]
        public int? ID_Modelo { get; set; }

        [Display(Name = "Ano Mínimo")]
        public int? Ano_Min { get; set; }

        [Display(Name = "Ano Máximo")]
        public int? Ano_Max { get; set; }

        [Display(Name = "Preço Mínimo")]
        public decimal? Preco_Min { get; set; }

        [Display(Name = "Preço Máximo")]
        public decimal? Preco_Max { get; set; }

        [Display(Name = "Quilometragem Mínima")]
        public int? Quilometragem_Min { get; set; }

        [Display(Name = "Quilometragem Máxima")]
        public int? Quilometragem_Max { get; set; }

        [Display(Name = "Combustível")]
        public int? ID_Combustivel { get; set; }

        [Display(Name = "Caixa")]
        public string? Caixa { get; set; }

        [Display(Name = "Cor")]
        public string? Cor { get; set; }

        [Display(Name = "Lotação Mínima")]
        public int? Lotacao_Min { get; set; }

        [Display(Name = "Lotação Máxima")]
        public int? Lotacao_Max { get; set; }

        [Display(Name = "Distrito")]
        public string? Distrito { get; set; }

        [Display(Name = "Ordenar por")]
        public string? SortBy { get; set; } = "data_desc"; // Default: mais recentes primeiro

        // ==================== RESULTADOS ====================

        public List<AnuncioViewModel> Resultados { get; set; } = new List<AnuncioViewModel>();

        public int TotalResultados { get; set; }
        public int PaginaAtual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int ItensPorPagina { get; set; } = 12;

        // ==================== DROPDOWNS ====================

        public List<SelectListItem> Marcas { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Modelos { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Combustiveis { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Cores { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> CategoriasDisponiveis => new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Todas" },
            new SelectListItem { Value = "SUV", Text = "SUV" },
            new SelectListItem { Value = "Coupe", Text = "Coupe" },
            new SelectListItem { Value = "Hatchback", Text = "Hatchback" },
            new SelectListItem { Value = "Saloon", Text = "Saloon" },
            new SelectListItem { Value = "Executive", Text = "Executive" },
            new SelectListItem { Value = "Sports", Text = "Sports" }
        };

        public List<SelectListItem> CaixasDisponiveis => new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Todas" },
            new SelectListItem { Value = "Manual", Text = "Manual" },
            new SelectListItem { Value = "Automática", Text = "Automática" }
        };

        public List<SelectListItem> DistritosDisponiveis => new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Todos" },
            new SelectListItem { Value = "Aveiro", Text = "Aveiro" },
            new SelectListItem { Value = "Beja", Text = "Beja" },
            new SelectListItem { Value = "Braga", Text = "Braga" },
            new SelectListItem { Value = "Bragança", Text = "Bragança" },
            new SelectListItem { Value = "Castelo Branco", Text = "Castelo Branco" },
            new SelectListItem { Value = "Coimbra", Text = "Coimbra" },
            new SelectListItem { Value = "Évora", Text = "Évora" },
            new SelectListItem { Value = "Faro", Text = "Faro" },
            new SelectListItem { Value = "Guarda", Text = "Guarda" },
            new SelectListItem { Value = "Leiria", Text = "Leiria" },
            new SelectListItem { Value = "Lisboa", Text = "Lisboa" },
            new SelectListItem { Value = "Portalegre", Text = "Portalegre" },
            new SelectListItem { Value = "Porto", Text = "Porto" },
            new SelectListItem { Value = "Santarém", Text = "Santarém" },
            new SelectListItem { Value = "Setúbal", Text = "Setúbal" },
            new SelectListItem { Value = "Viana do Castelo", Text = "Viana do Castelo" },
            new SelectListItem { Value = "Vila Real", Text = "Vila Real" },
            new SelectListItem { Value = "Viseu", Text = "Viseu" },
            new SelectListItem { Value = "Açores", Text = "Açores" },
            new SelectListItem { Value = "Madeira", Text = "Madeira" }
        };

        public List<SelectListItem> OpcoesOrdenacao => new List<SelectListItem>
        {
            new SelectListItem { Value = "data_desc", Text = "Mais recentes" },
            new SelectListItem { Value = "preco_asc", Text = "Preço: menor para maior" },
            new SelectListItem { Value = "preco_desc", Text = "Preço: maior para menor" },
            new SelectListItem { Value = "quilometragem_asc", Text = "Menor quilometragem" },
            new SelectListItem { Value = "ano_desc", Text = "Mais novos" }
        };

        // ==================== HELPER METHODS ====================

        public bool HasFilters()
        {
            return !string.IsNullOrEmpty(Categoria)
                || ID_Marca.HasValue
                || ID_Modelo.HasValue
                || Ano_Min.HasValue
                || Ano_Max.HasValue
                || Preco_Min.HasValue
                || Preco_Max.HasValue
                || Quilometragem_Min.HasValue
                || Quilometragem_Max.HasValue
                || ID_Combustivel.HasValue
                || !string.IsNullOrEmpty(Caixa)
                || !string.IsNullOrEmpty(Cor)
                || Lotacao_Min.HasValue
                || Lotacao_Max.HasValue
                || !string.IsNullOrEmpty(Distrito);
        }

        public string GetFilterSummary()
        {
            if (!HasFilters())
                return "Todos os anúncios";

            var filters = new List<string>();

            if (!string.IsNullOrEmpty(Categoria))
                filters.Add($"Categoria: {Categoria}");

            if (ID_Marca.HasValue && Marcas.Any())
            {
                var marcaNome = Marcas.FirstOrDefault(m => m.Value == ID_Marca.ToString())?.Text;
                if (marcaNome != null)
                    filters.Add($"Marca: {marcaNome}");
            }

            if (Preco_Min.HasValue || Preco_Max.HasValue)
            {
                var precoRange = $"Preço: {(Preco_Min.HasValue ? $"€{Preco_Min:N0}" : "Min")} - {(Preco_Max.HasValue ? $"€{Preco_Max:N0}" : "Max")}";
                filters.Add(precoRange);
            }

            if (Ano_Min.HasValue || Ano_Max.HasValue)
            {
                var anoRange = $"Ano: {(Ano_Min.HasValue ? Ano_Min.ToString() : "Min")} - {(Ano_Max.HasValue ? Ano_Max.ToString() : "Max")}";
                filters.Add(anoRange);
            }

            return string.Join(", ", filters);
        }
    }
}
