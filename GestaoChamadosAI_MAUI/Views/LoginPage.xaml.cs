using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views;

public partial class LoginPage : ContentPage
{
    private const double MobileBreakpoint = 800; // Largura que define mobile vs desktop

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateLayout(width);
    }

    private void OnPageSizeChanged(object sender, EventArgs e)
    {
        UpdateLayout(Width);
    }

    private void UpdateLayout(double width)
    {
        bool isMobile = width < MobileBreakpoint;

        if (isMobile)
        {
            // Layout Mobile: 1 coluna
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            Grid.SetRow(BrandingPanel, 0);
            Grid.SetColumn(BrandingPanel, 0);
            Grid.SetRow(FormPanel, 1);
            Grid.SetColumn(FormPanel, 0);

            // Ajustar espaçamentos para mobile
            BrandingPanel.Padding = new Thickness(20, 30);
            BrandingContent.Spacing = 15;
            
            LogoImage.HeightRequest = 80;
            LogoImage.WidthRequest = 80;
            
            TitleLabel.FontSize = 28;
            SubtitleLabel.FontSize = 13;
            
            FeaturesPanel.Spacing = 10;
            FeaturesPanel.Margin = new Thickness(0, 10, 0, 0);

            FormContent.Padding = new Thickness(20, 30);
            FormContent.Spacing = 20;
            
            WelcomeLabel.FontSize = 24;
            WelcomeSubLabel.FontSize = 13;
        }
        else
        {
            // Layout Desktop: 2 colunas
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            Grid.SetRow(BrandingPanel, 0);
            Grid.SetColumn(BrandingPanel, 0);
            Grid.SetRow(FormPanel, 0);
            Grid.SetColumn(FormPanel, 1);

            // Ajustar espaçamentos para desktop
            BrandingPanel.Padding = new Thickness(60, 40);
            BrandingContent.Spacing = 30;
            
            LogoImage.HeightRequest = 180;
            LogoImage.WidthRequest = 180;
            
            TitleLabel.FontSize = 48;
            SubtitleLabel.FontSize = 18;
            
            FeaturesPanel.Spacing = 15;
            FeaturesPanel.Margin = new Thickness(0, 20, 0, 0);

            FormContent.Padding = new Thickness(60, 40);
            FormContent.Spacing = 30;
            
            WelcomeLabel.FontSize = 32;
            WelcomeSubLabel.FontSize = 16;
        }
    }
}
