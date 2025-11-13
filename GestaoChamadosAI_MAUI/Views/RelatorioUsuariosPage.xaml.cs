using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views
{
    public partial class RelatorioUsuariosPage : ContentPage
    {
        public RelatorioUsuariosPage(RelatorioUsuariosViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            Loaded += async (s, e) => await viewModel.LoadRelatorioCommand.ExecuteAsync(null);
        }
    }
}
