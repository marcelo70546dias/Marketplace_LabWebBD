using Microsoft.AspNetCore.Identity;

namespace Marketplace_LabWebBD.Models
{
    // Classe que integra o Identity com a tabela Utilizador existente
    public class ApplicationUser : IdentityUser<int>
    {
        // Propriedades da tabela Utilizador
        public string Nome { get; set; } = string.Empty;
        public bool? Email_Validado { get; set; }
        public string? Token_Validacao { get; set; }
        public DateTime? Data_Token { get; set; }
        public string? Contacto { get; set; }
        public string? Morada { get; set; }
        public DateOnly Data_Registo { get; set; }
        public string? Status { get; set; }
        public bool? Bloqueado { get; set; }
        public string? Motivo_Bloqueio { get; set; }
        public DateOnly? Data_Bloqueio { get; set; }
        public int? ID_Admin_Bloqueio { get; set; }

        // Role pendente para aprovação (usado quando alguém se regista como Admin)
        public string? RolePendente { get; set; }

        // Campos para aprovação de vendedor
        public string? VendorApprovalStatus { get; set; } // null, "Pending", "Approved", "Rejected"
        public int? ID_Aprovacao_Vendedor { get; set; }
        public DateOnly? Data_Aprovacao_Vendedor { get; set; }
        public string? Motivo_Rejeicao_Vendedor { get; set; }
    }
}
