using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Log_Admin
{
    public int ID_Log { get; set; }

    public int ID_Admin { get; set; }

    public string Tipo_Acao { get; set; } = null!;

    public string? Descricao { get; set; }

    public int? ID_Utilizador_Afetado { get; set; }

    public int? ID_Anuncio_Afetado { get; set; }

    public DateTime Data_Hora { get; set; }

    public string? IP_Address { get; set; }

    public virtual Admin ID_AdminNavigation { get; set; } = null!;

    public virtual Anuncio? ID_Anuncio_AfetadoNavigation { get; set; }

    public virtual Utilizador? ID_Utilizador_AfetadoNavigation { get; set; }
}
