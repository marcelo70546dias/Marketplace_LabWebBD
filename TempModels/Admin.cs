using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Admin
{
    public int IdUtilizador { get; set; }

    public virtual ICollection<Crium> Cria { get; set; } = new List<Crium>();

    public virtual AspNetUser IdUtilizadorNavigation { get; set; } = null!;

    public virtual ICollection<LogAdmin> LogAdmins { get; set; } = new List<LogAdmin>();

    public virtual ICollection<Modera> Moderas { get; set; } = new List<Modera>();

    public virtual ICollection<Vendedor> Vendedors { get; set; } = new List<Vendedor>();
}
