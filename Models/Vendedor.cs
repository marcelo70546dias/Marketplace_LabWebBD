using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Vendedor
{
    public int ID_Utilizador { get; set; }

    public string Tipo { get; set; } = null!;

    public string? NIF { get; set; }

    public string? Dados_Faturacao { get; set; }

    public int? ID_Aprovacao { get; set; }

    public DateOnly? Data_Aprovacao { get; set; }

    public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();

    public virtual Admin? ID_AprovacaoNavigation { get; set; }

    public virtual Utilizador ID_UtilizadorNavigation { get; set; } = null!;
}
