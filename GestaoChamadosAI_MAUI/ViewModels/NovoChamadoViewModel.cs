using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;
using GestaoChamadosAI_MAUI.Views;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    public partial class NovoChamadoViewModel : ObservableObject
    {
        private readonly IChamadoService _chamadoService;

        [ObservableProperty]
        private string titulo = string.Empty;

        [ObservableProperty]
        private string descricao = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public NovoChamadoViewModel(IChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
        }

        [RelayCommand]
        private async Task CreateChamadoAsync()
        {
            if (string.IsNullOrWhiteSpace(Titulo) || string.IsNullOrWhiteSpace(Descricao))
            {
                ErrorMessage = "Por favor, preencha todos os campos";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var request = new NovoChamadoRequest
                {
                    Titulo = Titulo,
                    Descricao = Descricao
                };

                var (success, message, chamado) = await _chamadoService.CreateChamadoAsync(request);

                if (success && chamado != null)
                {
                    // Se a IA gerou uma resposta, mostra a tela de feedback
                    if (!string.IsNullOrEmpty(chamado.RespostaIA))
                    {
                        var navigationParameter = new Dictionary<string, object>
                        {
                            { "ChamadoId", chamado.Id },
                            { "Titulo", chamado.Titulo },
                            { "Descricao", chamado.Descricao },
                            { "RespostaIA", chamado.RespostaIA }
                        };
                        
                        await Shell.Current.GoToAsync(nameof(FeedbackIAPage), navigationParameter);
                    }
                    else
                    {
                        // Se não há resposta da IA, vai direto para os detalhes
                        await Shell.Current.DisplayAlert("Sucesso", message, "OK");
                        
                        var navigationParameter = new Dictionary<string, object>
                        {
                            { "Id", chamado.Id }
                        };
                        
                        await Shell.Current.GoToAsync($"../{nameof(ChamadoDetalhePage)}", navigationParameter);
                    }
                }
                else
                {
                    ErrorMessage = message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao criar chamado: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
