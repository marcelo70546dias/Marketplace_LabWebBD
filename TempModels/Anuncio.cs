using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class Anuncio
{
    public int IdAnuncio { get; set; }

    public string Titulo { get; set; } = null!;

    public decimal Preco { get; set; }

    public string? Localizacao { get; set; }

    public string? Descricao { get; set; }

    public DateTime DataPublicacao { get; set; }

    public DateTime? DataAtualizacao { get; set; }

    public string? EstadoAnuncio { get; set; }

    public int? ReservadoPor { get; set; }

    public DateOnly? ReservadoAte { get; set; }

    public int? PrazoReservaDias { get; set; }

    public int IdCarro { get; set; }

    public int IdVendedor { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual ICollection<HistoricoReserva> HistoricoReservas { get; set; } = new List<HistoricoReserva>();

    public virtual Carro IdCarroNavigation { get; set; } = null!;

    public virtual Vendedor IdVendedorNavigation { get; set; } = null!;

    public virtual ICollection<LogAdmin> LogAdmins { get; set; } = new List<LogAdmin>();

    public virtual ICollection<Modera> Moderas { get; set; } = new List<Modera>();

    public virtual Comprador? ReservadoPorNavigation { get; set; }

    public virtual ICollection<Visitum> Visita { get; set; } = new List<Visitum>();
}
