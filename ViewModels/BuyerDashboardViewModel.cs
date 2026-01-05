namespace Marketplace_LabWebBD.ViewModels
{
    public class BuyerDashboardViewModel
    {
        public List<AnuncioViewModel> AnunciosRecentes { get; set; } = new List<AnuncioViewModel>();
        public List<AnuncioViewModel> AnunciosDestaque { get; set; } = new List<AnuncioViewModel>();
    }
}
