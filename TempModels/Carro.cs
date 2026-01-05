using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Carro
{
    public int IdCarro { get; set; }

    public int IdVendedor { get; set; }

    public int Ano { get; set; }

    public int Quilometragem { get; set; }

    public int? Lotacao { get; set; }

    public string? Categoria { get; set; }

    public string? Caixa { get; set; }

    public string? Cor { get; set; }

    public int IdModelo { get; set; }

    public int IdCombustivel { get; set; }

    public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

    public virtual ICollection<Foto> Fotos { get; set; } = new List<Foto>();

    public virtual Combustivel IdCombustivelNavigation { get; set; } = null!;

    public virtual Modelo IdModeloNavigation { get; set; } = null!;

    public virtual Vendedor IdVendedorNavigation { get; set; } = null!;
}
