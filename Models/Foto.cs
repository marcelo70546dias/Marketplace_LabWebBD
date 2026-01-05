using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Foto
{
    public int ID { get; set; }

    public string Fotografia { get; set; } = null!;

    public int? Ordem { get; set; }

    public int ID_Carro { get; set; }

    public virtual Carro ID_CarroNavigation { get; set; } = null!;
}
