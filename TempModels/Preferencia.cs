using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Preferencia
{
    public int IdUtilizador { get; set; }

    public int IdMarca { get; set; }

    public DateOnly? DataAdicao { get; set; }

    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    public virtual AspNetUser IdUtilizadorNavigation { get; set; } = null!;
}
