using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views
{
    public partial class EditarChamadoPage : ContentPage
    {
        public EditarChamadoPage(EditarChamadoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
