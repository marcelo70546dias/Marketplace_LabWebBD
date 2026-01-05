using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Combustivel
{
    public int IdCombustivel { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();

    public virtual ICollection<FiltroFavorito> FiltroFavoritos { get; set; } = new List<FiltroFavorito>();
}
