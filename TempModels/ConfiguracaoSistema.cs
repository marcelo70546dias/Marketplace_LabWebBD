using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.TempModels;

public partial class ConfiguracaoSistema
{
    public int Id { get; set; }

    public string NomeMarketplace { get; set; } = null!;

    public string? EmailContacto { get; set; }

    public string? TelefoneContacto { get; set; }

    public string? HorarioSuporte { get; set; }

    public string? TermosUso { get; set; }

    public string? PoliticaPrivacidade { get; set; }

    public string? DescricaoSobre { get; set; }
}
