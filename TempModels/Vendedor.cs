using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Vendedor
{
    public int IdUtilizador { get; set; }

    public string Tipo { get; set; } = null!;

    public string? Nif { get; set; }

    public string? DadosFaturacao { get; set; }

    public int? IdAprovacao { get; set; }

    public DateOnly? DataAprovacao { get; set; }

    public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();

    public virtual Admin? IdAprovacaoNavigation { get; set; }

    public virtual AspNetUser IdUtilizadorNavigation { get; set; } = null!;
}
