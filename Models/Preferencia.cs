using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Preferencia
{
    public int ID_Utilizador { get; set; }

    public int ID_Marca { get; set; }

    public DateOnly? Data_Adicao { get; set; }

    public virtual Marca ID_MarcaNavigation { get; set; } = null!;

    public virtual Utilizador ID_UtilizadorNavigation { get; set; } = null!;
}
