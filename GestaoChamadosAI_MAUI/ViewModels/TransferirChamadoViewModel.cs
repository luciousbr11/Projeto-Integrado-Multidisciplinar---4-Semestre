using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    [QueryProperty(nameof(ChamadoId), nameof(ChamadoId))]
    [QueryProperty(nameof(ChamadoTitulo), nameof(ChamadoTitulo))]
    public partial class TransferirChamadoViewModel : ObservableObject
    {
        private readonly IChamadoService _chamadoService;

        [ObservableProperty]
        private int chamadoId;

        [ObservableProperty]
        private string chamadoTitulo = string.Empty;

        [ObservableProperty]
        private List<Usuario> suportes = new();

        [ObservableProperty]
        private Usuario? suporteSelecionado;

        [ObservableProperty]
        private bool isLoading;

        public TransferirChamadoViewModel(IChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
        }

        partial void OnChamadoIdChanged(int value)
        {
            if (value > 0)
            {
                _ = LoadSuportesAsync();
            }
        }

        private async Task LoadSuportesAsync()
        {
            IsLoading = true;
            try
            {
                var suportesLista = await _chamadoService.GetSuportesAsync();
                if (suportesLista != null)
                {
                    Suportes = suportesLista;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar suportes: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task TransferirAsync()
        {
            if (SuporteSelecionado == null)
            {
                await Shell.Current.DisplayAlert("Atenção", "Selecione um suporte para transferir", "OK");
                return;
            }

            var confirm = await Shell.Current.DisplayAlert(
                "Confirmar Transferência",
                $"Deseja transferir este chamado para {SuporteSelecionado.Nome}?",
                "Sim",
                "Não");

            if (!confirm) return;

            IsLoading = true;
            try
            {
                var (success, message) = await _chamadoService.TransferirChamadoAsync(ChamadoId, SuporteSelecionado.Id);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Sucesso", message, "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao transferir chamado: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelarAsync()
        {
            await Shell.Current.GoToAsync("..");
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
                await Shell.Current.GoToAsync("//ChamadosListPage");
            }
        }
    }
}
