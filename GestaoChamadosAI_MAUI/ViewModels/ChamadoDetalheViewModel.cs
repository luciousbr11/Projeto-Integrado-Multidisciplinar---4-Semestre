using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;
using GestaoChamadosAI_MAUI.Views;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public partial class ChamadoDetalheViewModel : ObservableObject
    {
        private readonly IChamadoService _chamadoService;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private Chamado? chamado;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private Usuario? currentUser;

        [ObservableProperty]
        private bool canAssume;

        [ObservableProperty]
        private bool canFinalize;

        [ObservableProperty]
        private bool canTransfer;

        // REMOVIDO: CanEdit - não permitir edição de chamados
        // [ObservableProperty]
        // private bool canEdit;

        [ObservableProperty]
        private bool canGenerateIA;

        [ObservableProperty]
        private bool canOpenChat;

        [ObservableProperty]
        private int diasDesdeAbertura;

        public ChamadoDetalheViewModel(IChamadoService chamadoService, IAuthService authService)
        {
            _chamadoService = chamadoService;
            _authService = authService;
        }

        [RelayCommand]
        private async Task LoadChamadoAsync()
        {
            if (Id == 0)
            {
                ErrorMessage = "ID do chamado inválido";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                CurrentUser = await _authService.GetCurrentUserAsync();
                Chamado = await _chamadoService.GetChamadoByIdAsync(Id);
                
                if (Chamado == null)
                {
                    ErrorMessage = "Chamado não encontrado ou erro ao carregar os dados da API";
                    await Shell.Current.DisplayAlert(
                        "Aviso", 
                        "Não foi possível carregar os detalhes deste chamado.\n\nIsso pode ocorrer se:\n• O chamado foi excluído\n• Há um problema na conexão com a API\n• Você não tem permissão para visualizar este chamado",
                        "OK");
                    await VoltarAsync();
                }
                else
                {
                    DiasDesdeAbertura = (DateTime.Now - Chamado.DataAbertura).Days;
                    UpdateButtonsVisibility();
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Erro de conexão: {ex.Message}";
                await Shell.Current.DisplayAlert("Erro de Conexão", "Não foi possível conectar à API. Verifique sua conexão de rede.", "OK");
                await VoltarAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar chamado: {ex.Message}";
                await Shell.Current.DisplayAlert("Erro", $"Ocorreu um erro inesperado:\n{ex.Message}", "OK");
                await VoltarAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateButtonsVisibility()
        {
            if (Chamado == null || CurrentUser == null) return;

            var isSuporte = CurrentUser.Tipo == "Suporte" || CurrentUser.Tipo == "Administrador";
            var isAdmin = CurrentUser.Tipo == "Administrador";
            var isCliente = CurrentUser.Tipo == "Cliente";
            var isClosed = Chamado.Status == "Concluído" || Chamado.Status == "Solucionado por IA";
            var isResponsavel = Chamado.SuporteResponsavelId == CurrentUser.Id;

            // Admin pode SEMPRE assumir (mesmo que já tenha outro suporte)
            // Suporte normal só pode assumir se não tem responsável
            CanAssume = !isClosed && (isAdmin || (isSuporte && Chamado.SuporteResponsavelId == null));

            // Cliente pode finalizar se é dele, ou Suporte se é responsável, e não está fechado
            CanFinalize = !isClosed && ((isCliente && Chamado.UsuarioId == CurrentUser.Id) || (isSuporte && isResponsavel));

            // Suporte pode transferir se é responsável ou Admin, e não está fechado
            CanTransfer = isSuporte && !isClosed && (isResponsavel || CurrentUser.Tipo == "Administrador");

            // REMOVIDO: CanEdit - não permitir edição de chamados
            // CanEdit = isSuporte;

            // Pode gerar IA se é suporte e não tem resposta ainda
            CanGenerateIA = isSuporte && string.IsNullOrEmpty(Chamado.RespostaIA);

            // Cliente pode SEMPRE abrir chat (mesmo sem suporte atribuído)
            // Suporte pode abrir chat se for responsável ou Admin
            CanOpenChat = isCliente || (isSuporte && (isResponsavel || CurrentUser.Tipo == "Administrador"));
        }

        [RelayCommand]
        private async Task VoltarAsync()
        {
            try
            {
                // Usar PopAsync para remover da pilha de navegação
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
                Console.WriteLine($"Erro ao voltar: {ex.Message}");
                // Fallback: navegar para a lista de chamados
                try
                {
                    await Shell.Current.GoToAsync("//ChamadosListPage");
                }
                catch
                {
                    // Último recurso: Dashboard
                    await Shell.Current.GoToAsync("//DashboardPage");
                }
            }
        }

        [RelayCommand]
        private async Task AssumirAsync()
        {
            try
            {
                var confirm = await Shell.Current.DisplayAlert(
                    "Assumir Atendimento",
                    "Deseja assumir o atendimento deste chamado?",
                    "Sim",
                    "Não");

                if (!confirm) return;

                IsLoading = true;
                
                var (success, message) = await _chamadoService.AssumirChamadoAsync(Id);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Sucesso", message, "OK");
                    await LoadChamadoAsync(); // Recarrega para atualizar
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao assumir chamado: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task FinalizarAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Finalizar Chamado",
                "Tem certeza que deseja finalizar este chamado?",
                "Sim",
                "Não");

            if (!confirm) return;

            IsLoading = true;
            try
            {
                var (success, message) = await _chamadoService.FinalizarChamadoAsync(Id);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Sucesso", message, "OK");
                    await VoltarAsync(); // Volta para a lista
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao finalizar chamado: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task TransferirAsync()
        {
            await Shell.Current.GoToAsync(nameof(TransferirChamadoPage), new Dictionary<string, object>
            {
                { "ChamadoId", Id },
                { "ChamadoTitulo", Chamado?.Titulo ?? "" }
            });
        }

        // REMOVIDO: EditarAsync - não permitir edição de chamados
        // [RelayCommand]
        // private async Task EditarAsync()
        // {
        //     await Shell.Current.GoToAsync(nameof(EditarChamadoPage), new Dictionary<string, object>
        //     {
        //         { "ChamadoId", Id }
        //     });
        // }

        [RelayCommand]
        private async Task GerarIAAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Gerar Resposta da IA",
                "Deseja gerar uma resposta automática usando inteligência artificial?",
                "Sim",
                "Não");

            if (!confirm) return;

            IsLoading = true;
            try
            {
                var (success, message) = await _chamadoService.GerarRespostaIAAsync(Id);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Resposta da IA gerada com sucesso!", "OK");
                    await LoadChamadoAsync(); // Recarrega para mostrar a resposta
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao gerar resposta da IA: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AbrirChatAsync()
        {
            await Shell.Current.GoToAsync(nameof(ChatPage), new Dictionary<string, object>
            {
                { "ChamadoId", Id }
            });
        }

        partial void OnIdChanged(int value)
        {
            if (value > 0)
            {
                _ = LoadChamadoAsync();
            }
        }
    }
}
