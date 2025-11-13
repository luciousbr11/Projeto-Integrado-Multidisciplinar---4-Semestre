using GestaoChamadosAI_MAUI.Views;

namespace GestaoChamadosAI_MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rotas para navegação
        // LoginPage e DashboardPage estão definidos no AppShell.xaml como ShellContent
        // ChamadosListPage está definido no AppShell.xaml como ShellContent
        Routing.RegisterRoute(nameof(ChamadoDetalhePage), typeof(ChamadoDetalhePage));
        Routing.RegisterRoute(nameof(NovoChamadoPage), typeof(NovoChamadoPage));
        Routing.RegisterRoute(nameof(ChatPage), typeof(ChatPage));
        Routing.RegisterRoute(nameof(ConfiguracoesPage), typeof(ConfiguracoesPage));
        Routing.RegisterRoute(nameof(FeedbackIAPage), typeof(FeedbackIAPage));
        Routing.RegisterRoute(nameof(TransferirChamadoPage), typeof(TransferirChamadoPage));
        Routing.RegisterRoute(nameof(EditarChamadoPage), typeof(EditarChamadoPage));
        Routing.RegisterRoute(nameof(RelatoriosPage), typeof(RelatoriosPage));
        Routing.RegisterRoute(nameof(RelatorioUsuariosPage), typeof(RelatorioUsuariosPage));
        Routing.RegisterRoute(nameof(RelatorioChamadosPeriodoPage), typeof(RelatorioChamadosPeriodoPage));
        Routing.RegisterRoute(nameof(RelatorioSuportesPage), typeof(RelatorioSuportesPage));
        Routing.RegisterRoute(nameof(RelatorioCategoriasPage), typeof(RelatorioCategoriasPage));
        
        // Rotas de Gestão de Usuários
        Routing.RegisterRoute(nameof(UsuariosListPage), typeof(UsuariosListPage));
        Routing.RegisterRoute(nameof(CriarUsuarioPage), typeof(CriarUsuarioPage));
        Routing.RegisterRoute(nameof(EditarUsuarioPage), typeof(EditarUsuarioPage));
    }
}
