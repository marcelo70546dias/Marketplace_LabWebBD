using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Historico_Reserva
{
    public int ID_Historico { get; set; }

    public int ID_Anuncio { get; set; }

    public int ID_Comprador { get; set; }

    public DateTime Data_Inicio { get; set; }

    public DateOnly? Data_Fim { get; set; }

    public string? Estado { get; set; }

    public virtual Anuncio ID_AnuncioNavigation { get; set; } = null!;

    public virtual Comprador ID_CompradorNavigation { get; set; } = null!;
}
