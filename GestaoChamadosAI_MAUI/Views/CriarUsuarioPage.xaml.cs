using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views
{
    public partial class CriarUsuarioPage : ContentPage
    {
        public CriarUsuarioPage(CriarUsuarioViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
