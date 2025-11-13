using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views
{
    public partial class UsuariosListPage : ContentPage
    {
        public UsuariosListPage(UsuariosListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is UsuariosListViewModel viewModel)
            {
                await viewModel.LoadDataCommand.ExecuteAsync(null);
            }
        }
    }
}
