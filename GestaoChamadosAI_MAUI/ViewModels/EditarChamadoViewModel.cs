using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;
using System.Collections.ObjectModel;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    [QueryProperty(nameof(ChamadoId), nameof(ChamadoId))]
    [QueryProperty(nameof(ChamadoTituloOriginal), nameof(ChamadoTituloOriginal))]
    [QueryProperty(nameof(ChamadoDescricaoOriginal), nameof(ChamadoDescricaoOriginal))]
    [QueryProperty(nameof(ChamadoStatusOriginal), nameof(ChamadoStatusOriginal))]
    [QueryProperty(nameof(ChamadoPrioridadeOriginal), nameof(ChamadoPrioridadeOriginal))]
    public partial class EditarChamadoViewModel : ObservableObject
    {
        private readonly IChamadoService _chamadoService;

        [ObservableProperty]
        private int chamadoId;

        [ObservableProperty]
        private string chamadoTituloOriginal = string.Empty;

        [ObservableProperty]
        private string chamadoDescricaoOriginal = string.Empty;

        [ObservableProperty]
        private string chamadoStatusOriginal = string.Empty;

        [ObservableProperty]
        private string chamadoPrioridadeOriginal = string.Empty;

        [ObservableProperty]
        private string titulo = string.Empty;

        [ObservableProperty]
        private string descricao = string.Empty;

        [ObservableProperty]
        private string? statusSelecionado;

        [ObservableProperty]
        private string? prioridadeSelecionada;

        [ObservableProperty]
        private bool isLoading;

        public ObservableCollection<string> StatusOptions { get; } = new()
        {
            "Aberto",
            "Em Andamento",
            "Aguardando Cliente",
            "Concluido",
            "Solucionado por IA"
        };

        public ObservableCollection<string> PrioridadeOptions { get; } = new()
        {
            "Baixa",
            "Média",
            "Alta"
        };

        public EditarChamadoViewModel(IChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
        }

        partial void OnChamadoTituloOriginalChanged(string value)
        {
            Titulo = value;
        }

        partial void OnChamadoDescricaoOriginalChanged(string value)
        {
            Descricao = value;
        }

        partial void OnChamadoStatusOriginalChanged(string value)
        {
            StatusSelecionado = value;
        }

        partial void OnChamadoPrioridadeOriginalChanged(string value)
        {
            PrioridadeSelecionada = value;
        }

        [RelayCommand]
        private async Task SalvarAsync()
        {
            // Validações
            if (string.IsNullOrWhiteSpace(Titulo))
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Atenção",
                    "O título é obrigatório.",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Descricao))
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Atenção",
                    "A descrição é obrigatória.",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(StatusSelecionado))
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Atenção",
                    "Selecione um status.",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(PrioridadeSelecionada))
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Atenção",
                    "Selecione uma prioridade.",
                    "OK");
                return;
            }

            // Confirmação
            bool confirmar = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar Edição",
                $"Deseja realmente salvar as alterações no chamado #{ChamadoId}?",
                "Sim",
                "Não");

            if (!confirmar)
                return;

            IsLoading = true;

            try
            {
                var request = new EditarChamadoRequest
                {
                    Titulo = Titulo,
                    Descricao = Descricao,
                    Status = StatusSelecionado,
                    Prioridade = PrioridadeSelecionada
                };

                var (success, message) = await _chamadoService.EditarChamadoAsync(ChamadoId, request);

                if (success)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Sucesso",
                        message,
                        "OK");

                    // Volta para a página anterior
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Erro",
                        message,
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Erro",
                    $"Erro ao salvar alterações: {ex.Message}",
                    "OK");
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
