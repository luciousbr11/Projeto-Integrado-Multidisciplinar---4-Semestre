using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    public partial class RelatorioChamadosPeriodoViewModel : ObservableObject
    {
        private readonly IRelatorioService _relatorioService;
        private readonly IPdfService _pdfService;
        private RelatorioChamadosPeriodo? _relatorioCompleto;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private DateTime dataInicio = DateTime.Now.AddMonths(-1);

        [ObservableProperty]
        private DateTime dataFim = DateTime.Now;

        [ObservableProperty]
        private int total;

        [ObservableProperty]
        private int abertos;

        [ObservableProperty]
        private int emAtendimento;

        [ObservableProperty]
        private int aguardandoCliente;

        [ObservableProperty]
        private int fechados;

        [ObservableProperty]
        private ObservableCollection<PrioridadeCount> porPrioridade = new();

        [ObservableProperty]
        private ObservableCollection<CategoriaCount> porCategoria = new();

        [ObservableProperty]
        private ObservableCollection<ChamadoResumo> chamados = new();

        public RelatorioChamadosPeriodoViewModel(IRelatorioService relatorioService, IPdfService pdfService)
        {
            _relatorioService = relatorioService;
            _pdfService = pdfService;
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            IsLoading = true;

            try
            {
                var relatorio = await _relatorioService.GetRelatorioChamadosPeriodoAsync(DataInicio, DataFim);

                if (relatorio != null)
                {
                    _relatorioCompleto = relatorio; // Guardar para PDF
                    
                    Total = relatorio.Total;
                    Abertos = relatorio.Abertos;
                    EmAtendimento = relatorio.EmAtendimento;
                    AguardandoCliente = relatorio.AguardandoCliente;
                    Fechados = relatorio.Fechados;

                    PorPrioridade.Clear();
                    foreach (var item in relatorio.PorPrioridade)
                    {
                        PorPrioridade.Add(item);
                    }

                    PorCategoria.Clear();
                    foreach (var item in relatorio.PorCategoria)
                    {
                        PorCategoria.Add(item);
                    }

                    Chamados.Clear();
                    foreach (var chamado in relatorio.Chamados)
                    {
                        Chamados.Add(chamado);
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar relatório: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task FiltrarAsync()
        {
            await LoadDataAsync();
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

        [RelayCommand]
        private async Task ImprimirPDFAsync()
        {
            if (_relatorioCompleto == null)
            {
                await Application.Current!.MainPage!.DisplayAlert("Aviso", "Carregue o relatório primeiro.", "OK");
                return;
            }

            try
            {
                IsLoading = true;
                var filePath = await _pdfService.GerarRelatorioChamadosPdfAsync(_relatorioCompleto);
                IsLoading = false;

                var result = await Application.Current!.MainPage!.DisplayAlert(
                    "PDF Gerado",
                    $"Relatório gerado com sucesso!\n\nArquivo: {Path.GetFileName(filePath)}\n\nDeseja abrir?",
                    "Sim", "Não");

                if (result)
                {
                    await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath) });
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro ao gerar PDF: {ex.Message}", "OK");
            }
        }
    }
}
