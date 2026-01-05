using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Carro
{
    public int ID_Carro { get; set; }

    public int ID_Vendedor { get; set; }

    public int Ano { get; set; }

    public int Quilometragem { get; set; }

    public int? Lotacao { get; set; }

    public string? Categoria { get; set; }

    public string? Caixa { get; set; }

    public string? Cor { get; set; }

    public int ID_Modelo { get; set; }

    public int ID_Combustivel { get; set; }

    public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

    public virtual ICollection<Foto> Fotos { get; set; } = new List<Foto>();

    public virtual Combustivel ID_CombustivelNavigation { get; set; } = null!;

    public virtual Modelo ID_ModeloNavigation { get; set; } = null!;

    public virtual Vendedor ID_VendedorNavigation { get; set; } = null!;
}
