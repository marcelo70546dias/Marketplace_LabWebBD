using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class LogAdmin
{
    public int IdLog { get; set; }

    public int IdAdmin { get; set; }

    public string TipoAcao { get; set; } = null!;

    public string? Descricao { get; set; }

    public int? IdUtilizadorAfetado { get; set; }

    public int? IdAnuncioAfetado { get; set; }

    public DateTime DataHora { get; set; }

    public string? IpAddress { get; set; }

    public virtual Admin IdAdminNavigation { get; set; } = null!;

    public virtual Anuncio? IdAnuncioAfetadoNavigation { get; set; }

    public virtual AspNetUser? IdUtilizadorAfetadoNavigation { get; set; }
}
