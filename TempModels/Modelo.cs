using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Modelo
{
    public int IdModelo { get; set; }

    public string Nome { get; set; } = null!;

    public int IdMarca { get; set; }

    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();

    public virtual ICollection<FiltroFavorito> FiltroFavoritos { get; set; } = new List<FiltroFavorito>();

    public virtual Marca IdMarcaNavigation { get; set; } = null!;
}
