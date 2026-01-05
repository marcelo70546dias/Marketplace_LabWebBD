using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class BlockUserViewModel
    {
        public int UserId { get; set; }

        public string UserNome { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "O motivo do bloqueio é obrigatório")]
        [StringLength(500, ErrorMessage = "O motivo não pode exceder 500 caracteres")]
        public string Motivo { get; set; } = string.Empty;
    }
}
