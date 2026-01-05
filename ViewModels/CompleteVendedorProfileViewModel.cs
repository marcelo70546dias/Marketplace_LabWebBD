using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class CompleteVendedorProfileViewModel
    {
        [Required(ErrorMessage = "Selecione o tipo de vendedor")]
        [Display(Name = "Tipo de Vendedor")]
        public string Tipo { get; set; } = string.Empty; // "Particular" ou "Empresa"

        [Required(ErrorMessage = "O NIF é obrigatório")]
        [StringLength(20, ErrorMessage = "NIF não pode ter mais de 20 caracteres")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "NIF deve conter exatamente 9 dígitos numéricos")]
        public string NIF { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Dados de Faturação (opcional)")]
        public string? Dados_Faturacao { get; set; }
    }
}
