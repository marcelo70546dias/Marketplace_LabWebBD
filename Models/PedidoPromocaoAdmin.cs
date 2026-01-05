using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace_LabWebBD.Models
{
    public class PedidoPromocaoAdmin
    {
        [Key]
        public int ID_Pedido { get; set; }

        [Required]
        public int ID_Utilizador { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo_Utilizador_Atual { get; set; } = string.Empty; // "Comprador" ou "Vendedor"

        [Required]
        public DateTime Data_Pedido { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendente"; // Pendente, Aprovado, Rejeitado

        public DateTime? Data_Resposta { get; set; }

        public int? ID_Admin_Resposta { get; set; }

        [StringLength(500)]
        public string? Justificacao { get; set; }

        [StringLength(500)]
        public string? Observacoes_Admin { get; set; }

        // Navigation properties
        [ForeignKey("ID_Utilizador")]
        public virtual ApplicationUser? ID_UtilizadorNavigation { get; set; }

        [ForeignKey("ID_Admin_Resposta")]
        public virtual ApplicationUser? ID_Admin_RespostaNavigation { get; set; }
    }
}
