using System.ComponentModel.DataAnnotations;

namespace Marketplace_LabWebBD.ViewModels
{
    public class EditAnuncioViewModel : CreateAnuncioViewModel
    {
        [Required]
        public int ID_Anuncio { get; set; }

        [Required]
        public int ID_Carro { get; set; }

        [Required]
        [Display(Name = "Estado do Anúncio")]
        public string Estado_Anuncio { get; set; } = "Ativo";

        // Título do anúncio (override para incluir no EditViewModel)
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título não pode exceder 200 caracteres")]
        [Display(Name = "Título")]
        public new string Titulo { get; set; } = string.Empty;

        // Fotos existentes
        public List<FotoViewModel> FotosExistentes { get; set; } = new List<FotoViewModel>();

        // Data de publicação (readonly display)
        public DateTime Data_Publicacao { get; set; }

        // Novas fotos para upload (adicional às existentes)
        public List<IFormFile>? NovasFotos { get; set; }
    }
}
