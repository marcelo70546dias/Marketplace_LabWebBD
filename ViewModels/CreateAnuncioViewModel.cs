using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Marketplace_LabWebBD.ViewModels
{
    public class CreateAnuncioViewModel
    {
        // Título do Anúncio
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título não pode exceder 200 caracteres")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        // Categoria do Veículo
        [Required(ErrorMessage = "A categoria é obrigatória")]
        [Display(Name = "Categoria")]
        public string Categoria { get; set; } = string.Empty;

        // Marca e Modelo (dropdown cascata)
        [Required(ErrorMessage = "A marca é obrigatória")]
        [Display(Name = "Marca")]
        public int ID_Marca { get; set; }

        [Required(ErrorMessage = "O modelo é obrigatório")]
        [Display(Name = "Modelo")]
        public int ID_Modelo { get; set; }

        // Preço
        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0.01, 999999999.99, ErrorMessage = "O preço deve ser maior que zero")]
        [Display(Name = "Preço (€)")]
        public decimal Preco { get; set; }

        // Ano
        [Required(ErrorMessage = "O ano é obrigatório")]
        [Range(1900, 2100, ErrorMessage = "Ano inválido")]
        [Display(Name = "Ano")]
        public int Ano { get; set; }

        // Quilometragem
        [Required(ErrorMessage = "A quilometragem é obrigatória")]
        [Range(0, 9999999, ErrorMessage = "Quilometragem inválida")]
        [Display(Name = "Quilometragem (km)")]
        public int Quilometragem { get; set; }

        // Transmissão (Manual/Automática)
        [Required(ErrorMessage = "A transmissão é obrigatória")]
        [Display(Name = "Transmissão")]
        public string Caixa { get; set; } = string.Empty;

        // Combustível
        [Required(ErrorMessage = "O combustível é obrigatório")]
        [Display(Name = "Combustível")]
        public int ID_Combustivel { get; set; }

        // Localização (Distrito)
        [Required(ErrorMessage = "A localização é obrigatória")]
        [Display(Name = "Localização")]
        public string Localizacao { get; set; } = string.Empty;

        // Descrição
        [StringLength(2000, ErrorMessage = "A descrição não pode exceder 2000 caracteres")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        // Fotografias
        public List<IFormFile>? Fotos { get; set; }

        // Listas para dropdowns (preenchidas no controller)
        public List<SelectListItem> Categorias { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Marcas { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Modelos { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Transmissoes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Combustiveis { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Distritos { get; set; } = new List<SelectListItem>();
    }
}
