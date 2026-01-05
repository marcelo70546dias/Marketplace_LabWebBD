using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class FiltroFavorito
{
    public int IdFiltro { get; set; }

    public int IdComprador { get; set; }

    public string NomeFiltro { get; set; } = null!;

    public string? Categoria { get; set; }

    public int? IdMarca { get; set; }

    public int? IdModelo { get; set; }

    public int? AnoMin { get; set; }

    public int? AnoMax { get; set; }

    public decimal? PrecoMin { get; set; }

    public decimal? PrecoMax { get; set; }

    public int? QuilometragemMax { get; set; }

    public int? IdCombustivel { get; set; }

    public string? Caixa { get; set; }

    public string? Localizacao { get; set; }

    public DateTime? DataCriacao { get; set; }

    public virtual Combustivel? IdCombustivelNavigation { get; set; }

    public virtual Comprador IdCompradorNavigation { get; set; } = null!;

    public virtual Marca? IdMarcaNavigation { get; set; }

    public virtual Modelo? IdModeloNavigation { get; set; }
}
