using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    // ViewModel para exibir detalhes de uma reserva
    public class ReservaViewModel
    {
        public int ID_Historico { get; set; }  // ID do histórico de reserva
        public int ID_Anuncio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public string? FotoPrincipal { get; set; }
        public DateTime Data_Inicio { get; set; }
        public DateTime Data_Fim { get; set; }
        public int DiasRestantes { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string MarcaModelo { get; set; } = string.Empty;
        public string? MotivoRecusa { get; set; }

        // Informação do comprador (para vendedor ver)
        public string? CompradorNome { get; set; }
        public string? CompradorEmail { get; set; }
        public string? CompradorContacto { get; set; }
    }

    // ViewModel para criar uma nova reserva
    public class CreateReservaViewModel
    {
        public int ID_Anuncio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public string? FotoPrincipal { get; set; }
        public string MarcaModelo { get; set; } = string.Empty;
        public int Prazo_Reserva_Dias { get; set; }
        public DateTime Data_Expiracao { get; set; }
    }
}
