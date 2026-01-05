using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Comprador
{
    public int IdComprador { get; set; }

    public int IdUtilizador { get; set; }

    public bool? NotificacoesEmail { get; set; }

    public bool? NotificacoesPush { get; set; }

    public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual ICollection<FiltroFavorito> FiltroFavoritos { get; set; } = new List<FiltroFavorito>();

    public virtual ICollection<HistoricoReserva> HistoricoReservas { get; set; } = new List<HistoricoReserva>();

    public virtual AspNetUser IdUtilizadorNavigation { get; set; } = null!;

    public virtual ICollection<Visitum> Visita { get; set; } = new List<Visitum>();
}
