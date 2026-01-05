using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Marca
{
    public int IdMarca { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<FiltroFavorito> FiltroFavoritos { get; set; } = new List<FiltroFavorito>();

    public virtual ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();

    public virtual ICollection<Preferencia> Preferencia { get; set; } = new List<Preferencia>();
}
