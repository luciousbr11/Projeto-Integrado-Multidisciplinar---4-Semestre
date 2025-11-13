using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views;

public partial class ConfiguracoesPage : ContentPage
{
    private readonly ConfiguracoesViewModel _viewModel;

    public ConfiguracoesPage(ConfiguracoesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}
