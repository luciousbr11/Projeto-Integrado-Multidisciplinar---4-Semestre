using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views
{
    public partial class RelatoriosPage : ContentPage
    {
        public RelatoriosPage(RelatoriosViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
