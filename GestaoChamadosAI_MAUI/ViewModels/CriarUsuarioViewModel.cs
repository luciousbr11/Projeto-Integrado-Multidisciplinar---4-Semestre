using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    public partial class CriarUsuarioViewModel : ObservableObject
    {
        private readonly IUsuarioService _usuarioService;

        [ObservableProperty]
        private string nome = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string senha = string.Empty;

        [ObservableProperty]
        private string confirmarSenha = string.Empty;

        [ObservableProperty]
        private string tipo = "Cliente";

        [ObservableProperty]
        private bool isLoading;

        public List<string> TiposDisponiveis { get; } = new List<string>
        {
            "Cliente",
            "Suporte",
            "Administrador"
        };

        public CriarUsuarioViewModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [RelayCommand]
        private async Task SalvarAsync()
        {
            // Validações
            if (string.IsNullOrWhiteSpace(Nome))
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, informe o nome do usuário.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, informe o email do usuário.", "OK");
                return;
            }

            if (!Email.Contains("@"))
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, informe um email válido.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Senha))
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, informe uma senha.", "OK");
                return;
            }

            if (Senha.Length < 6)
            {
                await Shell.Current.DisplayAlert("Erro", "A senha deve ter no mínimo 6 caracteres.", "OK");
                return;
            }

            if (Senha != ConfirmarSenha)
            {
                await Shell.Current.DisplayAlert("Erro", "As senhas não coincidem.", "OK");
                return;
            }

            IsLoading = true;

            try
            {
                var request = new CreateUsuarioRequest
                {
                    Nome = Nome,
                    Email = Email,
                    Senha = Senha,
                    Tipo = Tipo
                };

                var (sucesso, mensagem) = await _usuarioService.CreateUsuarioAsync(request);

                if (sucesso)
                {
                    await Shell.Current.DisplayAlert("Sucesso", mensagem, "OK");
                    
                    // Volta 1 nível na navegação (para UsuariosListPage)
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", mensagem, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao criar usuário: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelarAsync()
        {
            // Pergunta se deseja realmente cancelar
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cancelar", 
                "Deseja realmente cancelar? Os dados não serão salvos.", 
                "Sim", 
                "Não");
            
            if (confirmar)
            {
                // Volta 1 nível na navegação
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
