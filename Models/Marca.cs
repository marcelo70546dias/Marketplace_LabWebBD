using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Marca
{
    public int ID_Marca { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<Filtro_Favorito> Filtro_Favoritos { get; set; } = new List<Filtro_Favorito>();

    public virtual ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();

    public virtual ICollection<Preferencia> Preferencia { get; set; } = new List<Preferencia>();
}
