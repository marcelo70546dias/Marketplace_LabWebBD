using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Utilizador
{
    public int ID { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

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

    public virtual Admin? Admin { get; set; }

    public virtual ICollection<Comprador> Compradors { get; set; } = new List<Comprador>();

    public virtual ICollection<Cria> Cria { get; set; } = new List<Cria>();

    public virtual Admin? ID_Admin_BloqueioNavigation { get; set; }

    public virtual ICollection<Log_Admin> Log_Admins { get; set; } = new List<Log_Admin>();

    public virtual ICollection<Preferencia> Preferencia { get; set; } = new List<Preferencia>();

    public virtual Vendedor? Vendedor { get; set; }
}
