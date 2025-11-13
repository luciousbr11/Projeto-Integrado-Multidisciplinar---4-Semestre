using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;
using GestaoChamadosAI_MAUI.Views;
using System.Collections.ObjectModel;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    public partial class UsuariosListViewModel : ObservableObject
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private ObservableCollection<Usuario> usuarios = new();

        [ObservableProperty]
        private ObservableCollection<Usuario> usuariosFiltrados = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string tipoFiltro = "Todos";

        [ObservableProperty]
        private int totalUsuarios;

        [ObservableProperty]
        private int totalAdministradores;

        [ObservableProperty]
        private int totalSuportes;

        [ObservableProperty]
        private int totalClientes;

        public List<string> TiposFiltro { get; } = new List<string> 
        { 
            "Todos", 
            "Administrador", 
            "Suporte", 
            "Cliente" 
        };

        public UsuariosListViewModel(IUsuarioService usuarioService, IAuthService authService)
        {
            _usuarioService = usuarioService;
            _authService = authService;
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            IsLoading = true;

            try
            {
                var usuarios = await _usuarioService.GetUsuariosAsync();

                if (usuarios != null)
                {
                    Usuarios.Clear();
                    foreach (var usuario in usuarios.OrderBy(u => u.Nome))
                    {
                        Usuarios.Add(usuario);
                    }

                    // Calcula estatísticas
                    TotalUsuarios = Usuarios.Count;
                    TotalAdministradores = Usuarios.Count(u => u.Tipo == "Administrador");
                    TotalSuportes = Usuarios.Count(u => u.Tipo == "Suporte");
                    TotalClientes = Usuarios.Count(u => u.Tipo == "Cliente");

                    AplicarFiltros();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar usuários: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            AplicarFiltros();
        }

        partial void OnTipoFiltroChanged(string value)
        {
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            var query = Usuarios.AsEnumerable();

            // Filtro por tipo
            if (TipoFiltro != "Todos")
            {
                query = query.Where(u => u.Tipo == TipoFiltro);
            }

            // Filtro por busca (nome ou email)
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                query = query.Where(u =>
                    u.Nome.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower));
            }

            UsuariosFiltrados.Clear();
            foreach (var usuario in query)
            {
                UsuariosFiltrados.Add(usuario);
            }
        }

        [RelayCommand]
        private async Task NavigateToNovoUsuarioAsync()
        {
            await Shell.Current.GoToAsync(nameof(CriarUsuarioPage));
        }

        [RelayCommand]
        private async Task EditarUsuarioAsync(Usuario usuario)
        {
            if (usuario != null)
            {
                await Shell.Current.GoToAsync($"{nameof(EditarUsuarioPage)}?UsuarioId={usuario.Id}");
            }
        }

        [RelayCommand]
        private async Task ExcluirUsuarioAsync(Usuario usuario)
        {
            if (usuario == null) return;

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser?.Id == usuario.Id)
            {
                await Shell.Current.DisplayAlert("Erro", "Você não pode excluir sua própria conta!", "OK");
                return;
            }

            var confirm = await Shell.Current.DisplayAlert(
                "Confirmar Exclusão",
                $"Deseja realmente excluir o usuário '{usuario.Nome}'?\n\nEsta ação não pode ser desfeita.",
                "Excluir",
                "Cancelar");

            if (!confirm) return;

            IsLoading = true;

            try
            {
                var (sucesso, mensagem) = await _usuarioService.DeleteUsuarioAsync(usuario.Id);

                if (sucesso)
                {
                    await Shell.Current.DisplayAlert("Sucesso", mensagem, "OK");
                    await LoadDataAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", mensagem, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao excluir usuário: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void LimparFiltros()
        {
            SearchText = string.Empty;
            TipoFiltro = "Todos";
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
