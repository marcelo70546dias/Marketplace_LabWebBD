using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Compra
{
    public int IdCompra { get; set; }

    public int IdComprador { get; set; }

    public int IdAnuncio { get; set; }

    public DateOnly Data { get; set; }

    public decimal Preco { get; set; }

    public string? EstadoPagamento { get; set; }

    public string? MetodoPagamento { get; set; }

    public string? Notas { get; set; }

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;

    public virtual Comprador IdCompradorNavigation { get; set; } = null!;
}
