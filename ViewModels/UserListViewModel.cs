using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class UserListViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Contacto { get; set; }
        public DateOnly Data_Registo { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool? Bloqueado { get; set; }
        public string? Motivo_Bloqueio { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool? Email_Validado { get; set; }
        public string? VendorApprovalStatus { get; set; }
    }

    public class UserListPageViewModel
    {
        public List<UserListViewModel> Users { get; set; } = new List<UserListViewModel>();
        public string? SearchTerm { get; set; }
        public string? RoleFilter { get; set; }
        public string? BlockedFilter { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
    }
}
