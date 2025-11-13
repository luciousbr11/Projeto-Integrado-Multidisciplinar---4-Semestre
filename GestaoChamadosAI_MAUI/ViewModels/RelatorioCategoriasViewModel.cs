using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    public partial class RelatorioCategoriasViewModel : ObservableObject
    {
        private readonly IRelatorioService _relatorioService;
        private readonly IPdfService _pdfService;
        private RelatorioCategorias? _relatorioCompleto;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private ObservableCollection<CategoriaDetalhada> categorias = new();

        [ObservableProperty]
        private int totalCategorias;

        [ObservableProperty]
        private int totalChamados;

        public RelatorioCategoriasViewModel(IRelatorioService relatorioService, IPdfService pdfService)
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
                var relatorio = await _relatorioService.GetRelatorioCategoriasAsync();

                if (relatorio != null)
                {
                    _relatorioCompleto = relatorio;
                    
                    Categorias.Clear();
                    foreach (var categoria in relatorio.Categorias.OrderByDescending(c => c.Total))
                    {
                        Categorias.Add(categoria);
                    }

                    TotalCategorias = Categorias.Count;
                    TotalChamados = Categorias.Sum(c => c.Total);
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
                var filePath = await _pdfService.GerarRelatorioCategoriasAsync(_relatorioCompleto);
                IsLoading = false;

                var result = await Application.Current!.MainPage!.DisplayAlert(
                    "PDF Gerado",
                    $"Relatório gerado!\n\nArquivo: {Path.GetFileName(filePath)}\n\nAbrir?",
                    "Sim", "Não");

                if (result)
                {
                    await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath) });
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro: {ex.Message}", "OK");
            }
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
    }
}
