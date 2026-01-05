using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class CheckoutViewModel
    {
        public int ID_Anuncio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string MarcaModelo { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public string? FotoPrincipal { get; set; }

        [Required(ErrorMessage = "O método de pagamento é obrigatório")]
        [Display(Name = "Método de Pagamento")]
        public string Metodo_Pagamento { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Notas")]
        public string? Notas { get; set; }

        public List<string> MetodosPagamento { get; set; } = new List<string>
        {
            "Transferência Bancária",
            "MB Way",
            "Multibanco"
        };
    }

    public class CompraViewModel
    {
        public int ID_Compra { get; set; }
        public int ID_Anuncio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string MarcaModelo { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string? FotoPrincipal { get; set; }
        public DateOnly Data { get; set; }
        public string? Estado_Pagamento { get; set; }
        public string? Metodo_Pagamento { get; set; }
        public string? Notas { get; set; }

        // Info do vendedor (para comprador)
        public string? VendedorNome { get; set; }
        public string? VendedorEmail { get; set; }
        public string? VendedorContacto { get; set; }

        // Info do comprador (para vendedor)
        public string? CompradorNome { get; set; }
        public string? CompradorEmail { get; set; }
        public string? CompradorContacto { get; set; }
    }

    public class OrderConfirmationViewModel
    {
        public int ID_Compra { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string MarcaModelo { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string? FotoPrincipal { get; set; }
        public DateOnly Data { get; set; }
        public string Metodo_Pagamento { get; set; } = string.Empty;
        public string Estado_Pagamento { get; set; } = string.Empty;

        // Dados de pagamento simulados
        public string? ReferenciaMB { get; set; }
        public string? EntidadeMB { get; set; }
        public string? NumeroTelemovelMBWay { get; set; }
        public string? IBAN { get; set; }
    }
}
