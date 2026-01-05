using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class CreateVisitaViewModel
    {
        public int ID_Anuncio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string MarcaModelo { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string? FotoPrincipal { get; set; }

        [Required(ErrorMessage = "A data e hora da visita são obrigatórias")]
        [Display(Name = "Data e Hora")]
        [DataType(DataType.DateTime)]
        public DateTime Data_Hora { get; set; }

        [Required(ErrorMessage = "A localização é obrigatória")]
        [StringLength(200)]
        [Display(Name = "Localização")]
        public string Localizacao { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }
    }

    public class VisitaViewModel
    {
        public int ID_Visita { get; set; }
        public int ID_Anuncio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string MarcaModelo { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string? FotoPrincipal { get; set; }
        public DateTime Data_Hora { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public string? Observacoes { get; set; }
        public string? Estado { get; set; }

        // Info do comprador (para vendedor)
        public string? CompradorNome { get; set; }
        public string? CompradorEmail { get; set; }
        public string? CompradorContacto { get; set; }
    }
}
