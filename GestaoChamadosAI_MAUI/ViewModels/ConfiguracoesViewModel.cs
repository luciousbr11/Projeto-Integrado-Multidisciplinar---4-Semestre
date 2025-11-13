using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Services;
using GestaoChamadosAI_MAUI.Views;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    public partial class ConfiguracoesViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IStorageService _storageService;

        [ObservableProperty]
        private string apiUrl = "http://localhost:5000/api";

        [ObservableProperty]
        private string userName = string.Empty;

        [ObservableProperty]
        private string userEmail = string.Empty;

        [ObservableProperty]
        private string userTipo = string.Empty;

        public ConfiguracoesViewModel(IAuthService authService, IStorageService storageService)
        {
            _authService = authService;
            _storageService = storageService;
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user != null)
            {
                UserName = user.Nome;
                UserEmail = user.Email;
                UserTipo = user.Tipo;
            }
        }

        [RelayCommand]
        private async Task SaveApiUrlAsync()
        {
            await _storageService.SetAsync("api_url", ApiUrl);
            await Shell.Current.DisplayAlert("Sucesso", "URL da API salva com sucesso!", "OK");
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Confirmar Logout",
                "Deseja realmente sair?",
                "Sim",
                "Não");

            if (confirm)
            {
                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
            }
        }
    }
}
