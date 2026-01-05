using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class HistoricoReserva
{
    public int IdHistorico { get; set; }

    public int IdAnuncio { get; set; }

    public int IdComprador { get; set; }

    public DateTime DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }

    public string? Estado { get; set; }

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;

    public virtual Comprador IdCompradorNavigation { get; set; } = null!;
}
