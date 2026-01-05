using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Compra
{
    public int ID_Compra { get; set; }

    public int ID_Comprador { get; set; }

    public int ID_Anuncio { get; set; }

    public DateOnly Data { get; set; }

    public decimal Preco { get; set; }

    public string? Estado_Pagamento { get; set; }

    public string? Metodo_Pagamento { get; set; }

    public string? Notas { get; set; }

    public virtual Anuncio ID_AnuncioNavigation { get; set; } = null!;

    public virtual Comprador ID_CompradorNavigation { get; set; } = null!;
}
