using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class RegisterCompradorViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(150, ErrorMessage = "O nome não pode exceder 150 caracteres")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O username é obrigatório")]
        [StringLength(100, ErrorMessage = "O username não pode exceder 100 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password é obrigatória")]
        [StringLength(100, ErrorMessage = "A password deve ter pelo menos {2} caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Password")]
        [Compare("Password", ErrorMessage = "A password e a confirmação não coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "O contacto é obrigatório")]
        [Phone(ErrorMessage = "Número de telefone inválido")]
        [StringLength(50)]
        [Display(Name = "Contacto")]
        public string Contacto { get; set; } = string.Empty;

        [Required(ErrorMessage = "A morada é obrigatória")]
        [StringLength(200)]
        [Display(Name = "Morada")]
        public string Morada { get; set; } = string.Empty;

        [Required(ErrorMessage = "O código postal é obrigatório")]
        [StringLength(20)]
        [Display(Name = "Código Postal")]
        public string Codigo_Postal { get; set; } = string.Empty;

        [Required(ErrorMessage = "O distrito é obrigatório")]
        [Display(Name = "Distrito")]
        public string Distrito { get; set; } = string.Empty;

        // Preferências de comprador
        [Display(Name = "Receber notificações por email")]
        public bool NotificacoesEmail { get; set; } = true;

        [Display(Name = "Receber notificações push")]
        public bool NotificacoesPush { get; set; } = true;
    }
}
