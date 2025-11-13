using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;
using GestaoChamadosAI_MAUI.Views;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    [QueryProperty(nameof(FiltroStatus), "filtroStatus")]
    [QueryProperty(nameof(FiltroSuporte), "filtroSuporte")]
    public partial class ChamadosListViewModel : ObservableObject
    {
        private readonly IChamadoService _chamadoService;
        private readonly IUsuarioService _usuarioService;

        [ObservableProperty]
        private ObservableCollection<ChamadoListItem> chamados = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string selectedStatus = "Todos";

        [ObservableProperty]
        private string selectedPrioridade = "Todas";

        [ObservableProperty]
        private Usuario? selectedSuporte;

        [ObservableProperty]
        private ObservableCollection<Usuario> suportes = new();

        // Filtros vindos da navegação
        private string? _filtroStatus;
        public string? FiltroStatus
        {
            get => _filtroStatus;
            set
            {
                _filtroStatus = value;
                if (!string.IsNullOrEmpty(value))
                {
                    SelectedStatus = value;
                }
            }
        }

        private string? _filtroSuporte;
        public string? FiltroSuporte
        {
            get => _filtroSuporte;
            set
            {
                _filtroSuporte = value;
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int suporteId))
                {
                    // Selecionar suporte correspondente após carregar lista
                    Task.Run(async () =>
                    {
                        await LoadSuportesAsync();
                        SelectedSuporte = Suportes.FirstOrDefault(s => s.Id == suporteId);
                        await LoadChamadosAsync();
                    });
                }
            }
        }

        private int currentPage = 1;
        private bool hasMoreItems = true;

        public List<string> StatusOptions { get; } = new()
        {
            "Todos", "Aberto", "Em Atendimento", "Solucionado por IA", "Concluído"
        };

        public List<string> PrioridadeOptions { get; } = new()
        {
            "Todas", "Baixa", "Média", "Alta"
        };

        public ChamadosListViewModel(IChamadoService chamadoService, IUsuarioService usuarioService)
        {
            _chamadoService = chamadoService;
            _usuarioService = usuarioService;
            _ = LoadSuportesAsync();
        }

        partial void OnSelectedStatusChanged(string value)
        {
            currentPage = 1;
            _ = LoadChamadosAsync();
        }

        partial void OnSelectedPrioridadeChanged(string value)
        {
            currentPage = 1;
            _ = LoadChamadosAsync();
        }

        partial void OnSelectedSuporteChanged(Usuario? value)
        {
            currentPage = 1;
            _ = LoadChamadosAsync();
        }

        public async Task LoadSuportesAsync()
        {
            try
            {
                var usuarios = await _usuarioService.GetUsuariosAsync(tipo: "Suporte");
                
                if (usuarios != null)
                {
                    Suportes.Clear();
                    Suportes.Add(new Usuario { Id = 0, Nome = "Todos os Suportes" });
                    
                    foreach (var usuario in usuarios)
                    {
                        Suportes.Add(usuario);
                    }
                    
                    SelectedSuporte = Suportes.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                // Falha silenciosa, suportes não são críticos
            }
        }

        [RelayCommand]
        private async Task LoadChamadosAsync()
        {
            if (IsLoading)
                return;

            IsLoading = true;

            try
            {
                var status = SelectedStatus == "Todos" ? null : SelectedStatus;
                var prioridade = SelectedPrioridade == "Todas" ? null : SelectedPrioridade;
                int? suporteId = (SelectedSuporte?.Id ?? 0) > 0 ? SelectedSuporte.Id : null;

                var result = await _chamadoService.GetChamadosAsync(
                    currentPage, 
                    20, 
                    status, 
                    suporteId, 
                    prioridade
                );

                if (result != null)
                {
                    if (currentPage == 1)
                    {
                        Chamados.Clear();
                    }

                    foreach (var chamado in result.Items)
                    {
                        Chamados.Add(chamado);
                    }

                    hasMoreItems = result.CurrentPage < result.TotalPages;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar chamados: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            currentPage = 1;
            await LoadChamadosAsync();
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task LoadMoreAsync()
        {
            if (!hasMoreItems || IsLoading)
                return;

            currentPage++;
            await LoadChamadosAsync();
        }

        [RelayCommand]
        private async Task FilterByStatusAsync()
        {
            currentPage = 1;
            await LoadChamadosAsync();
        }

        [RelayCommand]
        private async Task ClearFiltersAsync()
        {
            SelectedStatus = "Todos";
            SelectedPrioridade = "Todas";
            SelectedSuporte = Suportes.FirstOrDefault();
            currentPage = 1;
            await LoadChamadosAsync();
        }

        [RelayCommand]
        private async Task NavigateToDetalhesAsync(ChamadoListItem chamado)
        {
            if (chamado == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(ChamadoDetalhePage)}?Id={chamado.Id}");
        }

        [RelayCommand]
        private async Task NavigateToNovoChamadoAsync()
        {
            await Shell.Current.GoToAsync(nameof(NovoChamadoPage));
        }

        [RelayCommand]
        private async Task VoltarAsync()
        {
            try
            {
                // Tenta voltar para a página anterior
                if (Shell.Current.Navigation.NavigationStack.Count > 1)
                {
                    await Shell.Current.Navigation.PopAsync();
                }
                else
                {
                    // Se não tiver pilha de navegação, vai para o dashboard
                    await Shell.Current.GoToAsync("//DashboardPage");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao voltar: {ex.Message}");
                // Fallback: tenta navegar para o dashboard
                await Shell.Current.GoToAsync("//DashboardPage");
            }
        }
    }
}
