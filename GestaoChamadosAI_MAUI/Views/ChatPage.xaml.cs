using GestaoChamadosAI_MAUI.ViewModels;

namespace GestaoChamadosAI_MAUI.Views;

public partial class ChatPage : ContentPage
{
    private readonly ChatViewModel _viewModel;

    public ChatPage(ChatViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        DebugLogger.Log($"[UI] 🎨 ChatPage construído");
        
        // Inscrever no evento de nova mensagem para fazer scroll
        _viewModel.Mensagens.CollectionChanged += (s, e) =>
        {
            DebugLogger.Log($"[UI] 🔔 CollectionChanged! Action={e.Action}, NewItems={e.NewItems?.Count ?? 0}, Total={_viewModel.Mensagens.Count}");
            
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                DebugLogger.Log($"[UI] ➕ Mensagem(ns) adicionada(s)! Fazendo scroll...");
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    ScrollToBottom();
                });
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                DebugLogger.Log($"[UI] 🔄 Collection foi resetada (Clear foi chamado)");
            }
        };
    }

    private void ScrollToBottom()
    {
        if (_viewModel.Mensagens.Count > 0)
        {
            var lastMessage = _viewModel.Mensagens[_viewModel.Mensagens.Count - 1];
            MessagesScrollView.ScrollToAsync(0, MessagesScrollView.ContentSize.Height, false);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DebugLogger.Log($"[UI] 👁️ OnAppearing - Mensagens Count: {_viewModel.Mensagens.Count}");
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(500);
            DebugLogger.Log($"[UI] 📜 Fazendo scroll inicial...");
            ScrollToBottom();
            
            // Log das mensagens visíveis
            DebugLogger.Log($"[UI] 🔍 MessagesContainer existe: {MessagesContainer != null}");
            if (MessagesContainer != null)
            {
                DebugLogger.Log($"[UI] 👶 MessagesContainer.Children.Count: {MessagesContainer.Children.Count}");
            }
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnDisappearing();
    }

    private async void OnVerLogsClicked(object sender, EventArgs e)
    {
        var logs = DebugLogger.ReadLogs();
        var logPath = DebugLogger.GetLogPath();
        
        await DisplayAlert(
            "📋 Debug Logs", 
            $"Logs salvos em:\n{logPath}\n\nÚltimas linhas:\n\n{GetLastLines(logs, 30)}", 
            "OK"
        );
    }

    private string GetLastLines(string text, int lineCount)
    {
        if (string.IsNullOrEmpty(text))
            return "Nenhum log disponível.";
            
        var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        var lastLines = lines.Skip(Math.Max(0, lines.Length - lineCount)).ToArray();
        return string.Join(Environment.NewLine, lastLines);
    }
}
