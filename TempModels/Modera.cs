using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Modera
{
    public int IdAdmin { get; set; }

    public int IdAnuncio { get; set; }

    public DateTime Data { get; set; }

    public string Acao { get; set; } = null!;

    public string? Motivo { get; set; }

    public virtual Admin IdAdminNavigation { get; set; } = null!;

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;
}
