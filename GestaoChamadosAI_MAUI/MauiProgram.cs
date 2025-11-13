using GestaoChamadosAI_MAUI.Services;
using GestaoChamadosAI_MAUI.ViewModels;
using GestaoChamadosAI_MAUI.Views;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace GestaoChamadosAI_MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Registrar Services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IChamadoService, ChamadoService>();
        builder.Services.AddSingleton<IStorageService, StorageService>();
        builder.Services.AddSingleton<IRelatorioService, RelatorioService>();
        builder.Services.AddSingleton<IUsuarioService, UsuarioService>();
        builder.Services.AddSingleton<IPdfService, PdfService>();

        // Registrar ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ChamadosListViewModel>();
        builder.Services.AddTransient<ChamadoDetalheViewModel>();
        builder.Services.AddTransient<NovoChamadoViewModel>();
        builder.Services.AddTransient<ChatViewModel>();
        builder.Services.AddTransient<ConfiguracoesViewModel>();
        builder.Services.AddTransient<FeedbackIAViewModel>();
        builder.Services.AddTransient<TransferirChamadoViewModel>();
        builder.Services.AddTransient<EditarChamadoViewModel>();
        builder.Services.AddTransient<RelatoriosViewModel>();
        builder.Services.AddTransient<RelatorioUsuariosViewModel>();
        builder.Services.AddTransient<RelatorioChamadosPeriodoViewModel>();
        builder.Services.AddTransient<RelatorioSuportesViewModel>();
        builder.Services.AddTransient<RelatorioCategoriasViewModel>();
        builder.Services.AddTransient<UsuariosListViewModel>();
        builder.Services.AddTransient<CriarUsuarioViewModel>();
        builder.Services.AddTransient<EditarUsuarioViewModel>();

        // Registrar Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ChamadosListPage>();
        builder.Services.AddTransient<ChamadoDetalhePage>();
        builder.Services.AddTransient<NovoChamadoPage>();
        builder.Services.AddTransient<ChatPage>();
        builder.Services.AddTransient<ConfiguracoesPage>();
        builder.Services.AddTransient<FeedbackIAPage>();
        builder.Services.AddTransient<TransferirChamadoPage>();
        builder.Services.AddTransient<EditarChamadoPage>();
        builder.Services.AddTransient<RelatoriosPage>();
        builder.Services.AddTransient<RelatorioUsuariosPage>();
        builder.Services.AddTransient<RelatorioChamadosPeriodoPage>();
        builder.Services.AddTransient<RelatorioSuportesPage>();
        builder.Services.AddTransient<RelatorioCategoriasPage>();
        builder.Services.AddTransient<UsuariosListPage>();
        builder.Services.AddTransient<CriarUsuarioPage>();
        builder.Services.AddTransient<EditarUsuarioPage>();

        return builder.Build();
    }
}
