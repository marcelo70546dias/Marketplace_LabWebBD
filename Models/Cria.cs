using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Cria
{
    public int ID_Admin { get; set; }

    public int ID_Utilizador { get; set; }

    public DateOnly Data { get; set; }

    public virtual Admin ID_AdminNavigation { get; set; } = null!;

    public virtual Utilizador ID_UtilizadorNavigation { get; set; } = null!;
}
