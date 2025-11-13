using GestaoChamadosAI_MAUI.Views;
using GestaoChamadosAI_MAUI.Services;

namespace GestaoChamadosAI_MAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}

