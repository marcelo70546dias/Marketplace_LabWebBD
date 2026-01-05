using Marketplace_LabWebBD.Models;

namespace Marketplace_LabWebBD.ViewModels
{
    public class UserDetailsViewModel
    {
        // User basic info
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Contacto { get; set; }
        public string? Morada { get; set; }
        public DateOnly Data_Registo { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool? Email_Validado { get; set; }
        public bool? Bloqueado { get; set; }
        public string? Motivo_Bloqueio { get; set; }
        public DateOnly? Data_Bloqueio { get; set; }
        public int? ID_Admin_Bloqueio { get; set; }
        public string? NomeAdminBloqueio { get; set; }

        // Roles
        public List<string> Roles { get; set; } = new List<string>();
        public string? RolePendente { get; set; }

        // Vendor approval info
        public string? VendorApprovalStatus { get; set; }
        public int? ID_Aprovacao_Vendedor { get; set; }
        public DateOnly? Data_Aprovacao_Vendedor { get; set; }
        public string? Motivo_Rejeicao_Vendedor { get; set; }
        public string? NomeAdminAprovacao { get; set; }

        // Comprador info
        public Comprador? CompradorPerfil { get; set; }
        public List<Marca> MarcasFavoritas { get; set; } = new List<Marca>();

        // Vendedor info
        public Vendedor? VendedorPerfil { get; set; }

        // Admin logs
        public List<Log_Admin> Logs { get; set; } = new List<Log_Admin>();
    }
}
