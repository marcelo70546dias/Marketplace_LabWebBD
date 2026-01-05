using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class PedidoPromocaoViewModel
    {
        [Required(ErrorMessage = "A justificação é obrigatória")]
        [StringLength(500, ErrorMessage = "A justificação não pode exceder 500 caracteres")]
        [Display(Name = "Justificação")]
        public string Justificacao { get; set; } = string.Empty;
    }

    public class PedidoPromocaoAdminViewModel
    {
        public int ID_Pedido { get; set; }
        public int ID_Utilizador { get; set; }
        public string NomeUtilizador { get; set; } = string.Empty;
        public string EmailUtilizador { get; set; } = string.Empty;
        public string Tipo_Utilizador_Atual { get; set; } = string.Empty;
        public DateTime Data_Pedido { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? Data_Resposta { get; set; }
        public string? NomeAdminResposta { get; set; }
        public string? Justificacao { get; set; }
        public string? Observacoes_Admin { get; set; }
    }

    public class RespostaPedidoViewModel
    {
        [Required]
        public int ID_Pedido { get; set; }

        [StringLength(500)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }
    }
}
