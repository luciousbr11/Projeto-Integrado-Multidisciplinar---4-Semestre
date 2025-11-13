using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views;

public partial class ChamadosListPage : ContentPage
{
    private readonly ChamadosListViewModel _viewModel;

    public ChamadosListPage(ChamadosListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSuportesAsync();
        await _viewModel.LoadChamadosCommand.ExecuteAsync(null);
    }
}
