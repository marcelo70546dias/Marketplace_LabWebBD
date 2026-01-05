using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Filtro_Favorito
{
    public int ID_Filtro { get; set; }

    public int ID_Comprador { get; set; }

    public string Nome_Filtro { get; set; } = null!;

    public string? Categoria { get; set; }

    public int? ID_Marca { get; set; }

    public int? ID_Modelo { get; set; }

    public int? Ano_Min { get; set; }

    public int? Ano_Max { get; set; }

    public decimal? Preco_Min { get; set; }

    public decimal? Preco_Max { get; set; }

    public int? Quilometragem_Max { get; set; }

    public int? ID_Combustivel { get; set; }

    public string? Caixa { get; set; }

    public string? Localizacao { get; set; }

    public DateTime? Data_Criacao { get; set; }

    public virtual Combustivel? ID_CombustivelNavigation { get; set; }

    public virtual Comprador ID_CompradorNavigation { get; set; } = null!;

    public virtual Marca? ID_MarcaNavigation { get; set; }

    public virtual Modelo? ID_ModeloNavigation { get; set; }
}
