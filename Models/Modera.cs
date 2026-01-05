using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Modera
{
    public int ID_Admin { get; set; }

    public int ID_Anuncio { get; set; }

    public DateTime Data { get; set; }

    public string Acao { get; set; } = null!;

    public string? Motivo { get; set; }

    public virtual Admin ID_AdminNavigation { get; set; } = null!;

    public virtual Anuncio ID_AnuncioNavigation { get; set; } = null!;
}
