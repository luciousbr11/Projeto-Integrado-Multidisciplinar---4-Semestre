using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Services;

namespace GestaoChamadosAI_MAUI.ViewModels;

[QueryProperty(nameof(ChamadoId), nameof(ChamadoId))]
[QueryProperty(nameof(Titulo), nameof(Titulo))]
[QueryProperty(nameof(Descricao), nameof(Descricao))]
[QueryProperty(nameof(RespostaIA), nameof(RespostaIA))]
public partial class FeedbackIAViewModel : ObservableObject
{
    private readonly IChamadoService _chamadoService;

    [ObservableProperty]
    private int chamadoId;

    [ObservableProperty]
    private string titulo = string.Empty;

    [ObservableProperty]
    private string descricao = string.Empty;

    [ObservableProperty]
    private string respostaIA = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    public FeedbackIAViewModel(IChamadoService chamadoService)
    {
        _chamadoService = chamadoService;
    }

    [RelayCommand]
    private async Task ResolveuAsync()
    {
        IsLoading = true;

        try
        {
            await _chamadoService.EnviarFeedbackAsync(ChamadoId, true);

            await Shell.Current.DisplayAlert(
                "✅ Ótimo!",
                "Ficamos felizes que a IA pôde resolver seu problema!\n\nSeu chamado foi marcado como 'Solucionado por IA'.",
                "OK"
            );

            // Volta para a lista de chamados
            await Shell.Current.GoToAsync("//ChamadosListPage");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao enviar feedback: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NaoResolveuAsync()
    {
        IsLoading = true;

        try
        {
            await _chamadoService.EnviarFeedbackAsync(ChamadoId, false);

            await Shell.Current.DisplayAlert(
                "📞 Entendido!",
                "Seu chamado foi encaminhado para nossa equipe de suporte.\n\nUm especialista irá atendê-lo em breve!",
                "OK"
            );

            // Volta para a lista de chamados
            await Shell.Current.GoToAsync("//ChamadosListPage");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao enviar feedback: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
