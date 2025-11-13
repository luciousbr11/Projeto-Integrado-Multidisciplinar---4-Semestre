using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestaoChamadosAI_MAUI.Models;
using GestaoChamadosAI_MAUI.Services;

namespace GestaoChamadosAI_MAUI.ViewModels
{
    [QueryProperty(nameof(ChamadoId), nameof(ChamadoId))]
    public partial class ChatViewModel : ObservableObject
    {
        private readonly IChamadoService _chamadoService;
        private readonly IAuthService _authService;
        private System.Timers.Timer? _pollingTimer;

        [ObservableProperty]
        private int chamadoId;

        [ObservableProperty]
        private Chamado? chamado;

        [ObservableProperty]
        private ObservableCollection<MensagemChamado> mensagens = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PodeEnviar))]
        private string novaMensagem = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PodeEnviar), nameof(PodeEnviarComAnexos))]
        private bool isLoading;

        [ObservableProperty]
        private int? currentUserId;

        [ObservableProperty]
        private string? currentUserTipo;

        [ObservableProperty]
        private string tituloChamado = "Chat";

        [ObservableProperty]
        private string descricaoStatus = "";

        [ObservableProperty]
        private string ultimaAtualizacao = "";

        [ObservableProperty]
        private string status = "";

        [ObservableProperty]
        private Color corStatus = Colors.Gray;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PodeEnviar), nameof(PodeEnviarComAnexos))]
        private bool chamadoFechado;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PodeEnviar))]
        private ObservableCollection<FileResult> anexosSelecionados = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PodeEnviar))]
        private bool temAnexos;

        public bool PodeEnviarComAnexos
        {
            get
            {
                var resultado = !IsLoading && !ChamadoFechado;
                DebugLogger.Log($"[CHAT] 🔐 PodeEnviarComAnexos={resultado} (IsLoading={IsLoading}, Fechado={ChamadoFechado})");
                return resultado;
            }
        }

        // Mostra aviso se for Admin/Suporte e NÃO for o responsável
        public bool MostrarAvisoAssumirChamado => IsSuporteOrAdmin && 
                                                   Chamado != null && 
                                                   Chamado.SuporteResponsavelId != CurrentUserId &&
                                                   !ChamadoFechado;

        // Cliente ou Suporte pode enviar mensagens
        // Admin só pode enviar se for o responsável
        public bool PodeEnviar => !string.IsNullOrWhiteSpace(NovaMensagem) && 
                                  !IsLoading && 
                                  !ChamadoFechado &&
                                  (CurrentUserTipo == "Cliente" || 
                                   CurrentUserTipo == "Suporte" ||
                                   (Chamado != null && Chamado.SuporteResponsavelId == CurrentUserId));

        // Admin pode SEMPRE assumir (mesmo que já tenha outro suporte)
        // Suporte normal só pode assumir se não tem responsável
        public bool PodeAssumir => Chamado != null && 
                                   (CurrentUserTipo == "Administrador" || 
                                    (CurrentUserTipo == "Suporte" && Chamado.SuporteResponsavelId == null)) &&
                                   Chamado.Status != "Concluído" &&
                                   Chamado.Status != "Solucionado por IA";

        public bool PodeFinalizar => Chamado != null && 
                                     (CurrentUserTipo == "Suporte" || CurrentUserTipo == "Administrador") &&
                                     Chamado.SuporteResponsavelId == CurrentUserId &&
                                     Chamado.Status != "Concluído" &&
                                     Chamado.Status != "Solucionado por IA";

        public bool PodeTransferir => Chamado != null && 
                                      (CurrentUserTipo == "Suporte" || CurrentUserTipo == "Administrador") &&
                                      Chamado.SuporteResponsavelId != null &&
                                      Chamado.Status == "Em Atendimento";

        public bool IsSuporteOrAdmin => CurrentUserTipo == "Suporte" || CurrentUserTipo == "Administrador";

        public ChatViewModel(IChamadoService chamadoService, IAuthService authService)
        {
            _chamadoService = chamadoService;
            _authService = authService;
        }

        [RelayCommand]
        private async Task LoadChamadoAsync()
        {
            if (ChamadoId == 0)
                return;

            IsLoading = true;

            try
            {
                var user = await _authService.GetCurrentUserAsync();
                CurrentUserId = user?.Id;
                CurrentUserTipo = user?.Tipo;
                
                DebugLogger.Log($"[CHAT] 👤 Usuário logado: ID={CurrentUserId}, Tipo={CurrentUserTipo}, Nome={user?.Nome}");

                // Carregar chamado
                Chamado = await _chamadoService.GetChamadoByIdAsync(ChamadoId);
                
                if (Chamado != null)
                {
                    DebugLogger.Log($"[CHAT] 📋 Chamado #{Chamado.Id} carregado");
                    TituloChamado = $"#{Chamado.Id} - {Chamado.Titulo}";
                    Status = Chamado.Status;
                    DescricaoStatus = ObterDescricaoStatus();
                    UltimaAtualizacao = $"Atualizado em {DateTime.Now:dd/MM/yyyy HH:mm}";
                    CorStatus = ObterCorStatus();
                    
                    // Verificar se chamado está fechado
                    ChamadoFechado = Chamado.Status == "Concluído" || Chamado.Status == "Solucionado por IA";
                    
                    OnPropertyChanged(nameof(PodeAssumir));
                    OnPropertyChanged(nameof(PodeFinalizar));
                    OnPropertyChanged(nameof(PodeTransferir));
                    OnPropertyChanged(nameof(PodeEnviar));
                    OnPropertyChanged(nameof(MostrarAvisoAssumirChamado));
                }

                // Carregar mensagens
                await CarregarMensagensAsync();

                // Iniciar polling
                IniciarPolling();
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"[CHAT] ❌ Erro em LoadChamadoAsync: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar chamado: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
        [RelayCommand]
        private async Task EnviarMensagemAsync()
        {
            DebugLogger.Log("[CHAT] � EnviarMensagemAsync INICIADO");
            DebugLogger.Log($"[CHAT] 📝 NovaMensagem: '{NovaMensagem}'");
            DebugLogger.Log($"[CHAT] 📎 AnexosSelecionados.Count: {AnexosSelecionados.Count}");
            
            if (string.IsNullOrWhiteSpace(NovaMensagem) && !AnexosSelecionados.Any())
            {
                DebugLogger.Log("[CHAT] ⚠️ Nenhuma mensagem ou anexo - retornando");
                return;
            }

            var mensagemTemp = NovaMensagem ?? "";
            NovaMensagem = string.Empty;
            OnPropertyChanged(nameof(PodeEnviar));

            try
            {
                DebugLogger.Log($"[CHAT] � Enviando mensagem: '{mensagemTemp}'");
                DebugLogger.Log($"[CHAT] 📎 Total de anexos a enviar: {AnexosSelecionados.Count}");

                // Upload de anexos primeiro
                var anexosUrls = new List<string>();
                
                if (AnexosSelecionados.Any())
                {
                    DebugLogger.Log($"[CHAT] 🔄 Iniciando upload de {AnexosSelecionados.Count} arquivo(s)...");
                    
                    for (int i = 0; i < AnexosSelecionados.Count; i++)
                    {
                        var arquivo = AnexosSelecionados[i];
                        DebugLogger.Log($"[CHAT] 📤 Uploading arquivo {i+1}/{AnexosSelecionados.Count}: {arquivo.FileName}");
                        
                        try
                        {
                            var uploadResult = await UploadArquivoAsync(arquivo);
                            
                            if (!string.IsNullOrEmpty(uploadResult))
                            {
                                anexosUrls.Add(uploadResult);
                                DebugLogger.Log($"[CHAT] ✅ Upload sucesso [{i+1}/{AnexosSelecionados.Count}]: {arquivo.FileName}");
                                DebugLogger.Log($"[CHAT] 🔗 URL retornada: {uploadResult}");
                            }
                            else
                            {
                                DebugLogger.Log($"[CHAT] ⚠️ Upload retornou vazio para: {arquivo.FileName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            DebugLogger.Log($"[CHAT] ❌ ERRO no upload {arquivo.FileName}:");
                            DebugLogger.Log($"[CHAT] ❌ Tipo: {ex.GetType().Name}");
                            DebugLogger.Log($"[CHAT] ❌ Mensagem: {ex.Message}");
                            DebugLogger.Log($"[CHAT] ❌ Stack: {ex.StackTrace}");
                            
                            await Shell.Current.DisplayAlert("Erro", $"Erro ao enviar arquivo {arquivo.FileName}: {ex.Message}", "OK");
                            return; // Para se houver erro no upload
                        }
                    }
                    
                    DebugLogger.Log($"[CHAT] ✅ Upload concluído! {anexosUrls.Count} URL(s) obtidas");
                    foreach (var url in anexosUrls)
                    {
                        DebugLogger.Log($"[CHAT] 🔗 URL: {url}");
                    }
                }
                else
                {
                    DebugLogger.Log("[CHAT] ℹ️ Nenhum anexo para enviar");
                }

                // Enviar mensagem com ou sem anexos
                DebugLogger.Log($"[CHAT] 🚀 Chamando EnviarMensagemComAnexosAsync...");
                DebugLogger.Log($"[CHAT] 🔢 ChamadoId: {ChamadoId}");
                DebugLogger.Log($"[CHAT] 📝 Mensagem: '{mensagemTemp}'");
                DebugLogger.Log($"[CHAT] 📎 Anexos: {anexosUrls.Count}");
                
                var (success, message) = await _chamadoService.EnviarMensagemComAnexosAsync(ChamadoId, mensagemTemp, anexosUrls);

                DebugLogger.Log($"[CHAT] 📬 Resposta recebida - Success: {success}");
                DebugLogger.Log($"[CHAT] 📬 Resposta recebida - Message: '{message}'");

                if (success)
                {
                    DebugLogger.Log($"[CHAT] ✅ SUCESSO! Mensagem enviada com {anexosUrls.Count} anexo(s)!");
                    
                    AnexosSelecionados.Clear();
                    TemAnexos = false;
                    DebugLogger.Log("[CHAT] 🧹 Anexos limpos");
                    
                    // Recarregar mensagens para pegar a mensagem com ID correto do servidor
                    DebugLogger.Log("[CHAT] 🔄 Recarregando mensagens...");
                    await CarregarMensagensAsync();
                    DebugLogger.Log("[CHAT] ✅ Mensagens recarregadas!");
                }
                else
                {
                    DebugLogger.Log($"[CHAT] ❌ FALHA ao enviar mensagem: {message}");
                    NovaMensagem = mensagemTemp;
                    await Shell.Current.DisplayAlert("Erro", message, "OK");
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"[CHAT] ❌❌❌ EXCEÇÃO CAPTURADA em EnviarMensagemAsync:");
                DebugLogger.Log($"[CHAT] ❌ Tipo: {ex.GetType().Name}");
                DebugLogger.Log($"[CHAT] ❌ Mensagem: {ex.Message}");
                DebugLogger.Log($"[CHAT] ❌ Stack Trace:");
                DebugLogger.Log($"[CHAT] {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    DebugLogger.Log($"[CHAT] ❌ Inner Exception: {ex.InnerException.GetType().Name}");
                    DebugLogger.Log($"[CHAT] ❌ Inner Message: {ex.InnerException.Message}");
                }
                
                NovaMensagem = mensagemTemp;
                await Shell.Current.DisplayAlert("Erro", $"Erro ao enviar mensagem: {ex.Message}", "OK");
            }
            
            DebugLogger.Log("[CHAT] 📤 EnviarMensagemAsync FINALIZADO");
        }

        private async Task CarregarMensagensAsync()
        {
            DebugLogger.Log($"[CHAT] 🔍 CarregarMensagensAsync chamado para ChamadoId={ChamadoId}");
            var msgs = await _chamadoService.GetMensagensAsync(ChamadoId);
            
            if (msgs != null)
            {
                DebugLogger.Log($"[CHAT] 📥 Recebidas {msgs.Count} mensagens da API");
                
                // IMPORTANTE: Ordenar mensagens por DataEnvio CRESCENTE (mais antigas primeiro, mais novas no final)
                var mensagensOrdenadas = msgs.OrderBy(m => m.DataEnvio).ToList();
                DebugLogger.Log($"[CHAT] 📊 Ordem após ordenação: {string.Join(", ", mensagensOrdenadas.Select(m => $"ID={m.Id}({m.DataEnvio:HH:mm:ss})"))}");
                
                // Atualizar collection de forma inteligente (não limpar tudo)
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Remover mensagens temporárias (Id = 0)
                    var temporarias = Mensagens.Where(m => m.Id == 0).ToList();
                    foreach (var temp in temporarias)
                    {
                        Mensagens.Remove(temp);
                        DebugLogger.Log($"[CHAT] 🗑️ Removida mensagem temporária");
                    }
                    
                    // Adicionar ou atualizar mensagens
                    foreach (var msg in mensagensOrdenadas)
                    {
                        msg.IsMinhaMensagem = msg.UsuarioId == CurrentUserId;
                        
                        // GARANTIR que Anexos nunca seja null
                        if (msg.Anexos == null)
                        {
                            msg.Anexos = new List<AnexoMensagem>();
                            DebugLogger.Log($"[CHAT] ⚠️ Anexos era null para msg ID={msg.Id}, inicializado como lista vazia");
                        }
                        
                        // LOG DETALHADO DOS ANEXOS
                        if (msg.Anexos.Any())
                        {
                            DebugLogger.Log($"[CHAT] 📎 Msg ID={msg.Id} tem {msg.Anexos.Count} anexo(s):");
                            foreach (var anexo in msg.Anexos)
                            {
                                DebugLogger.Log($"[CHAT]    - ID={anexo.Id}, Nome={anexo.NomeArquivo}");
                                DebugLogger.Log($"[CHAT]    - URL={anexo.Url}");
                                DebugLogger.Log($"[CHAT]    - Tipo={anexo.Tipo}");
                                DebugLogger.Log($"[CHAT]    - IsImage={anexo.IsImage}");
                                DebugLogger.Log($"[CHAT]    - TemAnexos={msg.TemAnexos}");
                            }
                        }
                        
                        var existente = Mensagens.FirstOrDefault(m => m.Id == msg.Id);
                        if (existente == null)
                        {
                            // Inserir na posição correta mantendo ordem por DataEnvio
                            int index = 0;
                            for (int i = 0; i < Mensagens.Count; i++)
                            {
                                if (Mensagens[i].DataEnvio > msg.DataEnvio)
                                {
                                    break;
                                }
                                index = i + 1;
                            }
                            Mensagens.Insert(index, msg);
                            DebugLogger.Log($"[CHAT] ➕ Nova mensagem ID={msg.Id} inserida na posição {index}, IsMinha={msg.IsMinhaMensagem}, Anexos={msg.Anexos?.Count ?? 0}");
                        }
                    }
                });
                
                DebugLogger.Log($"[CHAT] ✅ Total na collection: {Mensagens.Count}");
                DebugLogger.Log($"[CHAT] 📋 IDs finais: {string.Join(", ", Mensagens.Select(m => $"{m.Id}({(m.IsMinhaMensagem ? "EU" : "OUTRO")})"))}");
            }
            else
            {
                DebugLogger.Log($"[CHAT] ⚠️ msgs é NULL!");
            }
        }

        [RelayCommand]
        private async Task SelecionarArquivoAsync()
        {
            try
            {
                var resultado = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecione um arquivo",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.Android, new[] { "image/*", "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" } },
                        { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv", ".zip", ".rar" } }
                    })
                });

                if (resultado != null)
                {
                    AnexosSelecionados.Add(resultado);
                    TemAnexos = AnexosSelecionados.Any();
                    DebugLogger.Log($"[CHAT] 📎 Arquivo selecionado: {resultado.FileName}");
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"[CHAT] ❌ Erro ao selecionar arquivo: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Erro ao selecionar arquivo", "OK");
            }
        }

        [RelayCommand]
        private async Task TirarFotoAsync()
        {
            try
            {
                DebugLogger.Log("[CHAT] 📷 Iniciando captura de foto...");
                
#if ANDROID
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    DebugLogger.Log("[CHAT] 📷 Câmera suportada, capturando...");
                    
                    var foto = await MediaPicker.Default.CapturePhotoAsync();
                    
                    if (foto != null)
                    {
                        AnexosSelecionados.Add(foto);
                        TemAnexos = AnexosSelecionados.Any();
                        DebugLogger.Log($"[CHAT] 📷 Foto capturada: {foto.FileName}");
                    }
                    else
                    {
                        DebugLogger.Log("[CHAT] ⚠️ Captura cancelada pelo usuário");
                    }
                }
                else
                {
                    DebugLogger.Log("[CHAT] ❌ Câmera não suportada");
                    await Shell.Current.DisplayAlert("Não suportado", "Câmera não disponível neste dispositivo", "OK");
                }
#else
                // No Desktop, usa FilePicker
                await SelecionarArquivoAsync();
#endif
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"[CHAT] ❌ Erro ao tirar foto: {ex.GetType().Name} - {ex.Message}");
                DebugLogger.Log($"[CHAT] ❌ Stack: {ex.StackTrace}");
                await Shell.Current.DisplayAlert("Erro", $"Erro ao tirar foto: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task SelecionarFotoGaleriaAsync()
        {
            try
            {
#if ANDROID
                var foto = await MediaPicker.Default.PickPhotoAsync();
                
                if (foto != null)
                {
                    AnexosSelecionados.Add(foto);
                    TemAnexos = AnexosSelecionados.Any();
                    DebugLogger.Log($"[CHAT] 🖼️ Foto selecionada da galeria: {foto.FileName}");
                }
#else
                // No Desktop, usa FilePicker
                await SelecionarArquivoAsync();
#endif
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"[CHAT] ❌ Erro ao selecionar foto: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Erro ao selecionar foto da galeria", "OK");
            }
        }

        [RelayCommand]
        private void RemoverAnexo(FileResult arquivo)
        {
            AnexosSelecionados.Remove(arquivo);
            TemAnexos = AnexosSelecionados.Any();
        }

        private async Task<string?> UploadArquivoAsync(FileResult arquivo)
        {
            try
            {
                using var stream = await arquivo.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();

                using var content = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                content.Add(fileContent, "file", arquivo.FileName);

                var token = await _authService.GetTokenAsync();
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = await _authService.GetApiUrlAsync();
                var response = await client.PostAsync($"{apiUrl}/api/Upload", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var uploadResponse = System.Text.Json.JsonSerializer.Deserialize<UploadResponse>(responseContent, 
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    return uploadResponse?.Url;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"[CHAT] ❌ Erro no upload: {ex.Message}");
                throw;
            }
        }

        [RelayCommand]
        private async Task AssumirAsync()
        {
            if (Chamado == null) return;

            var confirmar = await Shell.Current.DisplayAlert(
                "Confirmar",
                "Deseja assumir este chamado?",
                "Sim",
                "Não");

            if (!confirmar) return;

            IsLoading = true;
            try
            {
                var (success, message) = await _chamadoService.AssumirChamadoAsync(ChamadoId);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Sucesso", message, "OK");
                    await LoadChamadoAsync();
                    OnPropertyChanged(nameof(MostrarAvisoAssumirChamado));
                    OnPropertyChanged(nameof(PodeAssumir));
                    OnPropertyChanged(nameof(PodeEnviar));
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task FinalizarAsync()
        {
            if (Chamado == null) return;

            var confirmar = await Shell.Current.DisplayAlert(
                "Confirmar",
                "Deseja finalizar este chamado?",
                "Sim",
                "Não");

            if (!confirmar) return;

            IsLoading = true;
            try
            {
                var (success, message) = await _chamadoService.FinalizarChamadoAsync(ChamadoId);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Sucesso", message, "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task TransferirAsync()
        {
            if (Chamado == null) return;

            await Shell.Current.GoToAsync($"TransferirChamadoPage?ChamadoId={ChamadoId}");
        }

        private void IniciarPolling()
        {
            _pollingTimer = new System.Timers.Timer(5000); // 5 segundos
            _pollingTimer.Elapsed += async (sender, e) =>
            {
                try
                {
                    await CarregarMensagensAsync();
                }
                catch
                {
                    // Ignora erros de polling silenciosamente
                }
            };
            _pollingTimer.Start();
        }

        private void PararPolling()
        {
            _pollingTimer?.Stop();
            _pollingTimer?.Dispose();
            _pollingTimer = null;
        }

        private string ObterDescricaoStatus()
        {
            return Chamado?.Status switch
            {
                "Aberto" => "Aguardando atendimento",
                "Em Atendimento" => $"Em atendimento com {Chamado.SuporteResponsavel?.Nome ?? "suporte"}",
                "Aguardando Cliente" => "Aguardando resposta do cliente",
                "Concluído" => "Chamado concluído",
                "Solucionado por IA" => "Solucionado automaticamente pela IA",
                _ => Chamado?.Status ?? ""
            };
        }

        private Color ObterCorStatus()
        {
            return Chamado?.Status switch
            {
                "Aberto" => Colors.Orange,
                "Em Atendimento" => Colors.Blue,
                "Aguardando Cliente" => Colors.Purple,
                "Concluído" => Colors.Green,
                "Solucionado por IA" => Colors.Green,
                _ => Colors.Gray
            };
        }

        partial void OnChamadoIdChanged(int value)
        {
            if (value > 0)
            {
                _ = LoadChamadoAsync();
            }
        }

        partial void OnNovaMensagemChanged(string value)
        {
            OnPropertyChanged(nameof(PodeEnviar));
        }

        public void OnDisappearing()
        {
            PararPolling();
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
                System.Diagnostics.Debug.WriteLine($"Erro ao voltar do chat: {ex.Message}");
                await Shell.Current.GoToAsync("//ChamadosListPage");
            }
        }

        [RelayCommand]
        private async Task AbrirAnexoAsync(AnexoMensagem anexo)
        {
            try
            {
                if (anexo == null || string.IsNullOrEmpty(anexo.Url))
                    return;

                Console.WriteLine($"[CHAT] 📎 Abrindo anexo: {anexo.NomeArquivo}");
                Console.WriteLine($"[CHAT] 🔗 URL: {anexo.Url}");

                // Tentar abrir no navegador padrão
                await Launcher.OpenAsync(new Uri(anexo.Url));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHAT] ❌ Erro ao abrir anexo: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível abrir o anexo: {ex.Message}", "OK");
            }
        }
    }
}
