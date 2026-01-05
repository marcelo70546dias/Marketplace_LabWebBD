using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Combustivel
{
    public int ID_Combustivel { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();

    public virtual ICollection<Filtro_Favorito> Filtro_Favoritos { get; set; } = new List<Filtro_Favorito>();
}
