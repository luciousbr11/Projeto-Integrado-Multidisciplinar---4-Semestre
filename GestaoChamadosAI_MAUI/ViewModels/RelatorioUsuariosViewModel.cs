using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;
using GestaoChamadosAI_MAUI.Views;
using System.Collections.ObjectModel;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    public partial class RelatorioUsuariosViewModel : ObservableObject
    {
        private readonly IRelatorioService _relatorioService;
        private readonly IPdfService _pdfService;
        private RelatorioUsuarios? _relatorioCompleto;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private int totalUsuarios;

        [ObservableProperty]
        private int clientes;

        [ObservableProperty]
        private int suportes;

        [ObservableProperty]
        private int administradores;

        public ObservableCollection<UsuarioDetalhado> Usuarios { get; } = new();

        public RelatorioUsuariosViewModel(IRelatorioService relatorioService, IPdfService pdfService)
        {
            _relatorioService = relatorioService;
            _pdfService = pdfService;
        }

        [RelayCommand]
        private async Task LoadRelatorioAsync()
        {
            IsLoading = true;
            System.Diagnostics.Debug.WriteLine("=== Iniciando carregamento do relatório de usuários ===");
            
            try
            {
                System.Diagnostics.Debug.WriteLine("Chamando serviço GetRelatorioUsuariosAsync...");
                var relatorio = await _relatorioService.GetRelatorioUsuariosAsync();
                
                if (relatorio != null)
                {
                    _relatorioCompleto = relatorio; // Guardar para usar no PDF
                    
                    System.Diagnostics.Debug.WriteLine($"Relatório recebido: {relatorio.TotalUsuarios} usuários");
                    
                    TotalUsuarios = relatorio.TotalUsuarios;
                    Clientes = relatorio.Clientes;
                    Suportes = relatorio.Suportes;
                    Administradores = relatorio.Administradores;

                    System.Diagnostics.Debug.WriteLine($"Stats: Total={TotalUsuarios}, Clientes={Clientes}, Suportes={Suportes}, Admin={Administradores}");

                    Usuarios.Clear();
                    if (relatorio.Usuarios != null && relatorio.Usuarios.Any())
                    {
                        foreach (var usuario in relatorio.Usuarios)
                        {
                            System.Diagnostics.Debug.WriteLine($"Adicionando usuário: {usuario.Nome} ({usuario.Tipo})");
                            Usuarios.Add(usuario);
                        }
                        System.Diagnostics.Debug.WriteLine($"Total de usuários adicionados: {Usuarios.Count}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("⚠️ Lista de usuários está vazia ou nula!");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Relatório retornou NULL!");
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Aviso",
                        "Nenhum dado foi retornado pela API. Verifique se o servidor está rodando.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERRO: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                await Application.Current!.MainPage!.DisplayAlert(
                    "Erro",
                    $"Erro ao carregar relatório: {ex.Message}\n\nVerifique se a API está rodando na porta 5000.",
                    "OK");
            }
            finally
            {
                IsLoading = false;
                System.Diagnostics.Debug.WriteLine("=== Carregamento finalizado ===");
            }
        }

        [RelayCommand]
        private async Task VoltarAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== BOTÃO VOLTAR CLICADO ===");
            try
            {
                System.Diagnostics.Debug.WriteLine("Tentando fechar a página atual...");
                
                // Obter a página atual e removê-la da pilha de navegação
                var navigation = Application.Current?.MainPage?.Navigation;
                if (navigation != null && navigation.NavigationStack.Count > 1)
                {
                    await navigation.PopAsync();
                    System.Diagnostics.Debug.WriteLine("Página removida com sucesso!");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Usando Shell.Current.Navigation...");
                    await Shell.Current.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO ao voltar: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Tentar navegação absoluta para DashboardPage
                try
                {
                    System.Diagnostics.Debug.WriteLine("Tentando navegação absoluta para Dashboard...");
                    await Shell.Current.GoToAsync("//DashboardPage");
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro no fallback: {ex2.Message}");
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Erro",
                        $"Não foi possível voltar. Por favor, use o menu principal.",
                        "OK");
                }
            }
        }

        [RelayCommand]
        private async Task ImprimirPDFAsync()
        {
            if (_relatorioCompleto == null)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Aviso",
                    "Carregue o relatório primeiro antes de gerar o PDF.",
                    "OK");
                return;
            }

            try
            {
                IsLoading = true;

                var filePath = await _pdfService.GerarRelatorioUsuariosPdfAsync(_relatorioCompleto);

                IsLoading = false;

                var result = await Application.Current!.MainPage!.DisplayAlert(
                    "PDF Gerado",
                    $"Relatório gerado com sucesso!\n\nArquivo salvo em:\n{filePath}\n\nDeseja abrir o arquivo?",
                    "Sim",
                    "Não");

                if (result)
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath)
                    });
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                await Application.Current!.MainPage!.DisplayAlert(
                    "Erro",
                    $"Erro ao gerar PDF: {ex.Message}",
                    "OK");
            }
        }
    }
}
