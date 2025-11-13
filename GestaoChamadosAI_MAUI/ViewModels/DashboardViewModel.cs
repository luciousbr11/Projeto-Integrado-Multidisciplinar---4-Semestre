using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;
using GestaoChamadosAI_MAUI.Views;
using System.Collections.ObjectModel;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IChamadoService _chamadoService;

        [ObservableProperty]
        private Usuario? currentUser;

        [ObservableProperty]
        private DashboardEstatisticas? estatisticas;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string welcomeMessage = string.Empty;

        [ObservableProperty]
        private bool isAdministrador;

        [ObservableProperty]
        private bool isNotCliente;

        [ObservableProperty]
        private bool isCliente;

        [ObservableProperty]
        private bool isSuporte;

        [ObservableProperty]
        private ObservableCollection<ChamadoListItem> chamadosAtivos = new();

        [ObservableProperty]
        private ObservableCollection<ChamadoListItem> chamadosRecentes = new();

        [ObservableProperty]
        private ObservableCollection<ChamadoListItem> chamadosRecentesAdmin = new();

        [ObservableProperty]
        private ObservableCollection<ChamadoListItem> meusChamadosAtivos = new();

        [ObservableProperty]
        private ObservableCollection<ChamadoListItem> chamadosAbertos = new();

        [ObservableProperty]
        private bool hasChamadosAtivos;

        [ObservableProperty]
        private bool hasChamadosRecentes;

        // Admin counts
        [ObservableProperty]
        private int totalChamados;

        [ObservableProperty]
        private int chamadosAbertosCount;

        [ObservableProperty]
        private int chamadosEmAndamentoCount;

        [ObservableProperty]
        private int chamadosConcluidosCount;

        // Suporte counts
        [ObservableProperty]
        private int chamadosPendentesCount;

        [ObservableProperty]
        private int chamadosEmAtendimentoCount;

        [ObservableProperty]
        private int chamadosConcluidosHojeCount;

        [ObservableProperty]
        private bool hasMeusChamadosAtivos;

        [ObservableProperty]
        private bool hasChamadosAbertos;

        [ObservableProperty]
        private bool hasNoChamadosAtivos;

        public DashboardViewModel(IAuthService authService, IChamadoService chamadoService)
        {
            _authService = authService;
            _chamadoService = chamadoService;
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            IsLoading = true;

            try
            {
                CurrentUser = await _authService.GetCurrentUserAsync();
                
                if (CurrentUser != null)
                {
                    WelcomeMessage = $"Olá, {CurrentUser.Nome}!";
                    IsAdministrador = CurrentUser.Tipo == "Administrador";
                    IsSuporte = CurrentUser.Tipo == "Suporte";
                    IsCliente = CurrentUser.Tipo == "Cliente";
                    IsNotCliente = CurrentUser.Tipo != "Cliente";
                    Estatisticas = await _chamadoService.GetEstatisticasAsync();

                    // Carrega dados específicos por tipo de usuário
                    if (IsCliente)
                    {
                        await LoadChamadosClienteAsync();
                    }
                    else if (IsSuporte)
                    {
                        await LoadChamadosSuporteAsync();
                    }
                    else if (IsAdministrador)
                    {
                        await LoadChamadosAdminAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dashboard: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadChamadosClienteAsync()
        {
            try
            {
                // Busca chamados do cliente
                var pagedResult = await _chamadoService.GetChamadosAsync(status: null);

                if (pagedResult != null && pagedResult.Items != null)
                {
                    var chamados = pagedResult.Items;

                    // Chamados ativos (Aberto ou Em Atendimento)
                    var ativos = chamados
                        .Where(c => c.Status == "Aberto" || c.Status == "Em Atendimento")
                        .OrderByDescending(c => c.DataAbertura)
                        .ToList();

                    ChamadosAtivos.Clear();
                    foreach (var chamado in ativos)
                    {
                        ChamadosAtivos.Add(chamado);
                    }

                    // Chamados recentes concluídos (últimos 3)
                    var recentes = chamados
                        .Where(c => c.Status == "Concluído" || c.Status == "Solucionado por IA")
                        .OrderByDescending(c => c.DataAbertura)
                        .Take(3)
                        .ToList();

                    ChamadosRecentes.Clear();
                    foreach (var chamado in recentes)
                    {
                        ChamadosRecentes.Add(chamado);
                    }

                    HasChamadosAtivos = ChamadosAtivos.Count > 0;
                    HasChamadosRecentes = ChamadosRecentes.Count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chamados cliente: {ex.Message}");
            }
        }

        private async Task LoadChamadosAdminAsync()
        {
            try
            {
                // Busca todos os chamados para calcular estatísticas
                var pagedResult = await _chamadoService.GetChamadosAsync(page: 1, pageSize: 100);

                if (pagedResult != null && pagedResult.Items != null)
                {
                    var chamados = pagedResult.Items;

                    // Calcula as contagens
                    TotalChamados = chamados.Count;
                    ChamadosAbertosCount = chamados.Count(c => c.Status == "Aberto");
                    ChamadosEmAndamentoCount = chamados.Count(c => c.Status == "Em Atendimento");
                    ChamadosConcluidosCount = chamados.Count(c => c.Status == "Concluído" || c.Status == "Solucionado por IA");

                    // Chamados recentes (últimos 10)
                    ChamadosRecentesAdmin.Clear();
                    foreach (var chamado in chamados.OrderByDescending(c => c.DataAbertura).Take(10))
                    {
                        ChamadosRecentesAdmin.Add(chamado);
                    }
                    HasChamadosRecentes = ChamadosRecentesAdmin.Count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chamados admin: {ex.Message}");
            }
        }

        private async Task LoadChamadosSuporteAsync()
        {
            try
            {
                var pagedResult = await _chamadoService.GetChamadosAsync(page: 1, pageSize: 100);

                if (pagedResult != null && pagedResult.Items != null)
                {
                    var chamados = pagedResult.Items;

                    // Calcula contagens
                    ChamadosPendentesCount = chamados.Count(c => c.Status == "Aberto" && c.SuporteResponsavelId == null);
                    ChamadosEmAtendimentoCount = chamados.Count(c => c.Status == "Em Atendimento" && c.SuporteResponsavelId == CurrentUser?.Id);
                    ChamadosConcluidosHojeCount = chamados.Count(c => 
                        (c.Status == "Concluído" || c.Status == "Solucionado por IA") && 
                        c.DataAbertura.Date == DateTime.Today);

                    // Meus chamados ativos (assumidos por mim e não finalizados)
                    var meus = chamados
                        .Where(c => c.SuporteResponsavelId == CurrentUser?.Id && 
                                   (c.Status == "Aberto" || c.Status == "Em Atendimento"))
                        .OrderByDescending(c => c.DataAbertura)
                        .ToList();

                    MeusChamadosAtivos.Clear();
                    foreach (var chamado in meus)
                    {
                        MeusChamadosAtivos.Add(chamado);
                    }

                    // Chamados aguardando atendimento (não assumidos)
                    var abertos = chamados
                        .Where(c => c.SuporteResponsavelId == null && c.Status == "Aberto")
                        .OrderByDescending(c => c.DataAbertura)
                        .Take(10)
                        .ToList();

                    ChamadosAbertos.Clear();
                    foreach (var chamado in abertos)
                    {
                        ChamadosAbertos.Add(chamado);
                    }

                    HasMeusChamadosAtivos = MeusChamadosAtivos.Count > 0;
                    HasChamadosAbertos = ChamadosAbertos.Count > 0;
                    HasNoChamadosAtivos = !HasMeusChamadosAtivos && !HasChamadosAbertos;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chamados suporte: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task NavigateToChamadoDetalheAsync(ChamadoListItem chamado)
        {
            if (chamado != null)
            {
                await Shell.Current.GoToAsync($"{nameof(ChamadoDetalhePage)}?Id={chamado.Id}");
            }
        }

        [RelayCommand]
        private async Task NavigateToChamadosAsync()
        {
            // Para suporte: mostrar apenas chamados dele
            if (CurrentUser?.Tipo == "Suporte")
            {
                await Shell.Current.GoToAsync($"//ChamadosListPage?filtroSuporte={CurrentUser.Id}");
            }
            else
            {
                await Shell.Current.GoToAsync("//ChamadosListPage");
            }
        }

        [RelayCommand]
        private async Task NavigateToChamadosPendentesAsync()
        {
            // Chamados com status "Aberto"
            await Shell.Current.GoToAsync("//ChamadosListPage?filtroStatus=Aberto");
        }

        [RelayCommand]
        private async Task NavigateToChamadosEmAtendimentoAsync()
        {
            // Chamados em atendimento pelo suporte logado
            if (CurrentUser?.Tipo == "Suporte")
            {
                await Shell.Current.GoToAsync($"//ChamadosListPage?filtroSuporte={CurrentUser.Id}&filtroStatus=Em Atendimento");
            }
            else
            {
                await Shell.Current.GoToAsync("//ChamadosListPage?filtroStatus=Em Atendimento");
            }
        }

        [RelayCommand]
        private async Task NavigateToUsuariosAsync()
        {
            await Shell.Current.GoToAsync(nameof(UsuariosListPage));
        }

        [RelayCommand]
        private async Task NavigateToNovoChamadoAsync()
        {
            await Shell.Current.GoToAsync(nameof(NovoChamadoPage));
        }

        [RelayCommand]
        private async Task NavigateToConfiguracoesAsync()
        {
            await Shell.Current.GoToAsync(nameof(ConfiguracoesPage));
        }

        [RelayCommand]
        private async Task NavigateToRelatoriosAsync()
        {
            await Shell.Current.GoToAsync(nameof(RelatoriosPage));
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Confirmar Logout",
                "Deseja realmente sair?",
                "Sim",
                "Não");

            if (confirm)
            {
                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
            }
        }
    }
}
