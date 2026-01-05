using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class AspNetUser
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public bool? EmailValidado { get; set; }

    public string? TokenValidacao { get; set; }

    public DateTime? DataToken { get; set; }

    public string? Contacto { get; set; }

    public string? Morada { get; set; }

    public DateOnly DataRegisto { get; set; }

    public string? Status { get; set; }

    public bool? Bloqueado { get; set; }

    public string? MotivoBloqueio { get; set; }

    public DateOnly? DataBloqueio { get; set; }

    public int? IdAdminBloqueio { get; set; }

    public string? RolePendente { get; set; }

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public DateOnly? DataAprovacaoVendedor { get; set; }

    public int? IdAprovacaoVendedor { get; set; }

    public string? MotivoRejeicaoVendedor { get; set; }

    public string? VendorApprovalStatus { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    public virtual ICollection<Comprador> Compradors { get; set; } = new List<Comprador>();

    public virtual ICollection<Crium> Cria { get; set; } = new List<Crium>();

    public virtual ICollection<LogAdmin> LogAdmins { get; set; } = new List<LogAdmin>();

    public virtual ICollection<Preferencia> Preferencia { get; set; } = new List<Preferencia>();

    public virtual Vendedor? Vendedor { get; set; }

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
