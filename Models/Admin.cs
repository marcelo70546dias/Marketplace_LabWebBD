using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Admin
{
    public int ID_Utilizador { get; set; }

    public virtual ICollection<Cria> Cria { get; set; } = new List<Cria>();

    public virtual Utilizador ID_UtilizadorNavigation { get; set; } = null!;

    public virtual ICollection<Log_Admin> Log_Admins { get; set; } = new List<Log_Admin>();

    public virtual ICollection<Modera> Moderas { get; set; } = new List<Modera>();

    public virtual ICollection<Utilizador> Utilizadors { get; set; } = new List<Utilizador>();

    public virtual ICollection<Vendedor> Vendedors { get; set; } = new List<Vendedor>();
}
