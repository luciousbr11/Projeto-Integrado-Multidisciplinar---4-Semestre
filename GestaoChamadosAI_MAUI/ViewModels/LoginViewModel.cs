using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Services;
using GestaoChamadosAI_MAUI.Views;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string senha = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            // Valida��o b�sica
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
            {
                ErrorMessage = "Por favor, preencha todos os campos";
                return;
            }

            // Valida��o de email
            if (!Email.Contains("@"))
            {
                ErrorMessage = "Por favor, insira um e-mail v�lido";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN] Tentando login: {Email}");
                
                var (success, message, usuario) = await _authService.LoginAsync(Email, Senha);
                
                System.Diagnostics.Debug.WriteLine($"[LOGIN] Resultado: {success}, {message}");

                if (success && usuario != null)
                {
                    await Shell.Current.GoToAsync($"///{nameof(DashboardPage)}");
                }
                else
                {
                    ErrorMessage = $"Login falhou: {message ?? "Credenciais inv�lidas"}";
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN] HttpRequestException: {ex}");
                ErrorMessage = $"ERRO DE CONEX�O\n\nVerifique:\n\n1. API rodando na porta 5000?\n2. PC e celular na MESMA rede Wi-Fi?\n3. Firewall aberto?\n\nIP configurado: 192.168.200.107:5000\n\nErro t�cnico: {ex.Message}";
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN] Timeout: {ex}");
                ErrorMessage = "TEMPO ESGOTADO\n\nA API n�o respondeu.\nVerifique se est� rodando.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN] Exception: {ex}");
                ErrorMessage = $"ERRO INESPERADO\n\n{ex.GetType().Name}:\n{ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
