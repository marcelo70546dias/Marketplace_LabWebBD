using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Anuncio
{
    public int ID_Anuncio { get; set; }

    public string Titulo { get; set; } = null!;

    public decimal Preco { get; set; }

    public string? Localizacao { get; set; }

    public string? Descricao { get; set; }

    public DateTime Data_Publicacao { get; set; }

    public DateTime? Data_Atualizacao { get; set; }

    public string? Estado_Anuncio { get; set; }

    public int? Reservado_Por { get; set; }

    public DateOnly? Reservado_Ate { get; set; }

    public int? Prazo_Reserva_Dias { get; set; }

    public int ID_Carro { get; set; }

    public int ID_Vendedor { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual ICollection<Historico_Reserva> Historico_Reservas { get; set; } = new List<Historico_Reserva>();

    public virtual Carro ID_CarroNavigation { get; set; } = null!;

    public virtual Vendedor ID_VendedorNavigation { get; set; } = null!;

    public virtual ICollection<Log_Admin> Log_Admins { get; set; } = new List<Log_Admin>();

    public virtual ICollection<Modera> Moderas { get; set; } = new List<Modera>();

    public virtual Comprador? Reservado_PorNavigation { get; set; }

    public virtual ICollection<Visita> Visita { get; set; } = new List<Visita>();
}
