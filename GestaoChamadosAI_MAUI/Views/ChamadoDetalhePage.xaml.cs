using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views;

public partial class ChamadoDetalhePage : ContentPage
{
    public ChamadoDetalhePage(ChamadoDetalheViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
