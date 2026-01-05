using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Foto
{
    public int Id { get; set; }

    public string Fotografia { get; set; } = null!;

    public int? Ordem { get; set; }

    public int IdCarro { get; set; }

    public virtual Carro IdCarroNavigation { get; set; } = null!;
}
