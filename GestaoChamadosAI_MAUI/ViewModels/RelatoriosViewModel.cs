using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Views;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    public partial class RelatoriosViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isLoading;

        [RelayCommand]
        private async Task AbrirRelatorioUsuariosAsync()
        {
            await Shell.Current.GoToAsync(nameof(RelatorioUsuariosPage));
        }

        [RelayCommand]
        private async Task AbrirRelatorioChamadosAsync()
        {
            await Shell.Current.GoToAsync(nameof(RelatorioChamadosPeriodoPage));
        }

        [RelayCommand]
        private async Task AbrirRelatorioSuportesAsync()
        {
            await Shell.Current.GoToAsync(nameof(RelatorioSuportesPage));
        }

        [RelayCommand]
        private async Task AbrirRelatorioCategoriasAsync()
        {
            await Shell.Current.GoToAsync(nameof(RelatorioCategoriasPage));
        }

        [RelayCommand]
        private async Task VoltarAsync()
        {
            try
            {
                var navigation = Application.Current?.MainPage?.Navigation;
                if (navigation != null && navigation.NavigationStack.Count > 1)
                {
                    await navigation.PopAsync();
                }
                else
                {
                    await Shell.Current.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao voltar: {ex.Message}");
                await Shell.Current.GoToAsync("//DashboardPage");
            }
        }
    }
}
