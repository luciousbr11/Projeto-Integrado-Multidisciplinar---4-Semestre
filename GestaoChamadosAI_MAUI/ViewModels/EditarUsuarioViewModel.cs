using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    [QueryProperty(nameof(UsuarioId), "UsuarioId")]
    public partial class EditarUsuarioViewModel : ObservableObject
    {
        private readonly IUsuarioService _usuarioService;

        [ObservableProperty]
        private int usuarioId;

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

        [ObservableProperty]
        private bool alterarSenha;

        public List<string> TiposDisponiveis { get; } = new List<string>
        {
            "Cliente",
            "Suporte",
            "Administrador"
        };

        public EditarUsuarioViewModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        partial void OnUsuarioIdChanged(int value)
        {
            if (value > 0)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await CarregarUsuarioAsync();
                });
            }
        }

        private async Task CarregarUsuarioAsync()
        {
            IsLoading = true;

            try
            {
                var usuarioDetalhado = await _usuarioService.GetUsuarioAsync(UsuarioId);

                if (usuarioDetalhado != null)
                {
                    Nome = usuarioDetalhado.Nome;
                    Email = usuarioDetalhado.Email;
                    Tipo = usuarioDetalhado.Tipo;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Usuário não encontrado.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar usuário: {ex.Message}", "OK");
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsLoading = false;
            }
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

            // Valida senha apenas se alterar senha estiver marcado
            if (AlterarSenha)
            {
                if (string.IsNullOrWhiteSpace(Senha))
                {
                    await Shell.Current.DisplayAlert("Erro", "Por favor, informe a nova senha.", "OK");
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
            }

            IsLoading = true;

            try
            {
                var request = new UpdateUsuarioRequest
                {
                    Nome = Nome,
                    Email = Email,
                    Tipo = Tipo
                };

                var (sucesso, mensagem) = await _usuarioService.UpdateUsuarioAsync(UsuarioId, request);

                if (sucesso)
                {
                    // Se deve alterar senha, faz a alteração separadamente
                    if (AlterarSenha && !string.IsNullOrWhiteSpace(Senha))
                    {
                        var (sucessoSenha, mensagemSenha) = await _usuarioService.AlterarSenhaAsync(UsuarioId, Senha);
                        if (!sucessoSenha)
                        {
                            await Shell.Current.DisplayAlert("Aviso", $"Usuário atualizado, mas houve erro ao alterar senha: {mensagemSenha}", "OK");
                            await Shell.Current.GoToAsync("..");
                            return;
                        }
                    }

                    await Shell.Current.DisplayAlert("Sucesso", mensagem, "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", mensagem, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao atualizar usuário: {ex.Message}", "OK");
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
    }
}
