using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Visita
{
    public int ID_Visita { get; set; }

    public int ID_Comprador { get; set; }

    public int ID_Anuncio { get; set; }

    public DateTime Data_Hora { get; set; }

    public string? Localizacao { get; set; }

    public string Estado { get; set; } = null!;

    public string? Observacoes { get; set; }

    public DateTime? Data_Criacao { get; set; }

    public virtual Anuncio ID_AnuncioNavigation { get; set; } = null!;

    public virtual Comprador ID_CompradorNavigation { get; set; } = null!;
}
