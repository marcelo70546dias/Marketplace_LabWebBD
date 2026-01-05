using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Modelo
{
    public int ID_Modelo { get; set; }

    public string Nome { get; set; } = null!;

    public int ID_Marca { get; set; }

    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();

    public virtual ICollection<Filtro_Favorito> Filtro_Favoritos { get; set; } = new List<Filtro_Favorito>();

    public virtual Marca ID_MarcaNavigation { get; set; } = null!;
}
