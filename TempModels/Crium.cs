using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Crium
{
    public int IdAdmin { get; set; }

    public int IdUtilizador { get; set; }

    public DateOnly Data { get; set; }

    public virtual Admin IdAdminNavigation { get; set; } = null!;

    public virtual AspNetUser IdUtilizadorNavigation { get; set; } = null!;
}
