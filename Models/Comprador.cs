using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Comprador
{
    public int ID_Comprador { get; set; }

    public int ID_Utilizador { get; set; }

    public bool? Notificacoes_Email { get; set; }

    public bool? Notificacoes_Push { get; set; }

    public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual ICollection<Filtro_Favorito> Filtro_Favoritos { get; set; } = new List<Filtro_Favorito>();

    public virtual ICollection<Historico_Reserva> Historico_Reservas { get; set; } = new List<Historico_Reserva>();

    public virtual Utilizador ID_UtilizadorNavigation { get; set; } = null!;

    public virtual ICollection<Visita> Visita { get; set; } = new List<Visita>();
}
