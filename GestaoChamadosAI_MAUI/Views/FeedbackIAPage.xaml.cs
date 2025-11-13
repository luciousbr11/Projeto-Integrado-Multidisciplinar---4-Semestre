using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views;

public partial class FeedbackIAPage : ContentPage
{
    public FeedbackIAPage(FeedbackIAViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
