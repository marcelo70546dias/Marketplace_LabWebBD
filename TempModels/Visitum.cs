using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Visitum
{
    public int IdVisita { get; set; }

    public int IdComprador { get; set; }

    public int IdAnuncio { get; set; }

    public DateTime DataHora { get; set; }

    public string? Localizacao { get; set; }

    public string Estado { get; set; } = null!;

    public string? Observacoes { get; set; }

    public DateTime? DataCriacao { get; set; }

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;

    public virtual Comprador IdCompradorNavigation { get; set; } = null!;
}
