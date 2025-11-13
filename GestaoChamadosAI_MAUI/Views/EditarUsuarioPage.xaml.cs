using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views
{
    public partial class EditarUsuarioPage : ContentPage
    {
        private readonly EditarUsuarioViewModel _viewModel;

        public EditarUsuarioPage(EditarUsuarioViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
