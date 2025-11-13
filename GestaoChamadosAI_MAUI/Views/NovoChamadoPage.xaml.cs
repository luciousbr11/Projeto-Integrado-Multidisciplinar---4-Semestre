using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views;

public partial class NovoChamadoPage : ContentPage
{
    public NovoChamadoPage(NovoChamadoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
