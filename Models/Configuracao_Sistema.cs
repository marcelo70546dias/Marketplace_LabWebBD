using System;
using System.Collections.Generic;

namespace Marketplace_LabWebBD.Models;

public partial class Configuracao_Sistema
{
    public int ID { get; set; }

    public string Nome_Marketplace { get; set; } = null!;

    public string? Email_Contacto { get; set; }

    public string? Telefone_Contacto { get; set; }

    public string? Horario_Suporte { get; set; }

    public string? Termos_Uso { get; set; }

    public string? Politica_Privacidade { get; set; }

    public string? Descricao_Sobre { get; set; }
}
