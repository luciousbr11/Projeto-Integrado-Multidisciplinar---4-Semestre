using GestaoChamadosAI_MAUI.Models;

namespace GestaoChamadosAI_MAUI.Services
{
    public interface IChamadoService
    {
        Task<PagedResult<ChamadoListItem>?> GetChamadosAsync(int page = 1, int pageSize = 20, string? status = null, int? suporteId = null, string? prioridade = null);
        Task<Chamado?> GetChamadoByIdAsync(int id);
        Task<(bool Success, string Message, Chamado? Chamado)> CreateChamadoAsync(NovoChamadoRequest request);
        Task<(bool Success, string Message)> EnviarMensagemAsync(int chamadoId, string mensagem);
        Task<(bool Success, string Message)> EnviarMensagemComAnexosAsync(int chamadoId, string mensagem, List<string> anexosUrls);
        Task<List<MensagemChamado>?> GetMensagensAsync(int chamadoId);
        Task<DashboardEstatisticas?> GetEstatisticasAsync();
        Task<bool> EnviarFeedbackAsync(int chamadoId, bool resolvido);
        Task<(bool Success, string Message)> AssumirChamadoAsync(int chamadoId);
        Task<(bool Success, string Message)> FinalizarChamadoAsync(int chamadoId);
        Task<(bool Success, string Message)> TransferirChamadoAsync(int chamadoId, int novoSuporteId);
        Task<(bool Success, string Message)> EditarChamadoAsync(int chamadoId, EditarChamadoRequest request);
        Task<(bool Success, string Message)> GerarRespostaIAAsync(int chamadoId);
        Task<List<Usuario>?> GetSuportesAsync();
    }

    public class ChamadoService : IChamadoService
    {
        private readonly IApiService _apiService;

        public ChamadoService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<PagedResult<ChamadoListItem>?> GetChamadosAsync(int page = 1, int pageSize = 20, string? status = null, int? suporteId = null, string? prioridade = null)
        {
            try
            {
                var endpoint = $"/api/Chamados?page={page}&pageSize={pageSize}";
                if (!string.IsNullOrEmpty(status))
                {
                    endpoint += $"&status={status}";
                }
                if (suporteId.HasValue)
                {
                    endpoint += $"&suporteId={suporteId.Value}";
                }
                if (!string.IsNullOrEmpty(prioridade))
                {
                    endpoint += $"&prioridade={prioridade}";
                }

                var response = await _apiService.GetAsync<ApiResponse<PagedResult<ChamadoListItem>>>(endpoint);
                return response?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting chamados: {ex.Message}");
                return null;
            }
        }

        public async Task<Chamado?> GetChamadoByIdAsync(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<Chamado>>($"/api/Chamados/{id}");
                return response?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting chamado: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string Message, Chamado? Chamado)> CreateChamadoAsync(NovoChamadoRequest request)
        {
            try
            {
                var response = await _apiService.PostAsync<ApiResponse<Chamado>>("/api/Chamados", request);

                if (response != null && response.Success && response.Data != null)
                {
                    return (true, "Chamado criado com sucesso!", response.Data);
                }

                return (false, response?.Message ?? "Erro ao criar chamado", null);
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> EnviarMensagemAsync(int chamadoId, string mensagem)
        {
            try
            {
                var request = new EnviarMensagemRequest { Mensagem = mensagem };
                var response = await _apiService.PostAsync<ApiResponse<MensagemChamado>>($"/api/Chat/{chamadoId}/mensagens", request);

                if (response != null && response.Success)
                {
                    return (true, "Mensagem enviada com sucesso!");
                }

                return (false, response?.Message ?? "Erro ao enviar mensagem");
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> EnviarMensagemComAnexosAsync(int chamadoId, string mensagem, List<string> anexosUrls)
        {
            try
            {
                Console.WriteLine($"[CHAMADO-SERVICE] ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine($"[CHAMADO-SERVICE] 🚀 EnviarMensagemComAnexosAsync INICIADO");
                Console.WriteLine($"[CHAMADO-SERVICE] 🔢 ChamadoId: {chamadoId}");
                Console.WriteLine($"[CHAMADO-SERVICE] 📝 Mensagem: '{mensagem}'");
                Console.WriteLine($"[CHAMADO-SERVICE] 📎 Anexos Count: {anexosUrls.Count}");

                var formData = new Dictionary<string, string>
                {
                    { "mensagem", mensagem ?? "" }
                };

                for (int i = 0; i < anexosUrls.Count; i++)
                {
                    formData[$"anexosUrls[{i}]"] = anexosUrls[i];
                    Console.WriteLine($"[CHAMADO-SERVICE] 📎 Anexo [{i}]: {anexosUrls[i]}");
                }

                Console.WriteLine($"[CHAMADO-SERVICE] 🌐 URL: /api/Chat/{chamadoId}/mensagens");
                Console.WriteLine($"[CHAMADO-SERVICE] 📋 FormData keys: {string.Join(", ", formData.Keys)}");
                Console.WriteLine($"[CHAMADO-SERVICE] 📤 Chamando PostFormAsync...");

                var response = await _apiService.PostFormAsync<ApiResponse<object>>($"/api/Chat/{chamadoId}/mensagens", formData);

                Console.WriteLine($"[CHAMADO-SERVICE] 📬 Resposta recebida!");
                Console.WriteLine($"[CHAMADO-SERVICE] 📬 Response is null? {response == null}");
                
                if (response != null)
                {
                    Console.WriteLine($"[CHAMADO-SERVICE] 📬 Response.Success: {response.Success}");
                    Console.WriteLine($"[CHAMADO-SERVICE] 📬 Response.Message: '{response.Message}'");
                    Console.WriteLine($"[CHAMADO-SERVICE] 📬 Response.Data is null? {response.Data == null}");
                }

                if (response != null && response.Success)
                {
                    Console.WriteLine($"[CHAMADO-SERVICE] ✅ Retornando SUCCESS");
                    return (true, "Mensagem enviada com sucesso!");
                }

                Console.WriteLine($"[CHAMADO-SERVICE] ❌ Retornando FAILURE");
                Console.WriteLine($"[CHAMADO-SERVICE] ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                return (false, response?.Message ?? "Erro ao enviar mensagem");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHAMADO-SERVICE] ❌❌❌ EXCEÇÃO CAPTURADA!");
                Console.WriteLine($"[CHAMADO-SERVICE] ❌ Tipo: {ex.GetType().Name}");
                Console.WriteLine($"[CHAMADO-SERVICE] ❌ Mensagem: {ex.Message}");
                Console.WriteLine($"[CHAMADO-SERVICE] ❌ StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[CHAMADO-SERVICE] ❌ Inner Exception: {ex.InnerException.GetType().Name}");
                    Console.WriteLine($"[CHAMADO-SERVICE] ❌ Inner Message: {ex.InnerException.Message}");
                }
                Console.WriteLine($"[CHAMADO-SERVICE] ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<List<MensagemChamado>?> GetMensagensAsync(int chamadoId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<MensagemChamado>>>($"/api/Chat/{chamadoId}");
                
                // LOG: Ver quantos anexos cada mensagem tem
                if (response?.Data != null)
                {
                    Console.WriteLine($"[CHAT-SERVICE] 📨 Recebidas {response.Data.Count} mensagens da API");
                    foreach (var msg in response.Data)
                    {
                        Console.WriteLine($"[CHAT-SERVICE] MSG ID={msg.Id}: Anexos={msg.Anexos?.Count ?? 0}");
                        if (msg.Anexos != null && msg.Anexos.Any())
                        {
                            foreach (var anexo in msg.Anexos)
                            {
                                Console.WriteLine($"[CHAT-SERVICE]   - Anexo: {anexo.NomeArquivo}, URL={anexo.Url}, Tipo={anexo.Tipo}");
                            }
                        }
                    }
                }
                
                return response?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting mensagens: {ex.Message}");
                return null;
            }
        }

        public async Task<DashboardEstatisticas?> GetEstatisticasAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<DashboardEstatisticas>>("/api/Dashboard/estatisticas");
                return response?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting estatisticas: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> EnviarFeedbackAsync(int chamadoId, bool resolvido)
        {
            try
            {
                var request = new { Resolvido = resolvido };
                var response = await _apiService.PostAsync<ApiResponse<object>>(
                    $"/api/Chamados/{chamadoId}/feedback",
                    request
                );
                return response != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending feedback: {ex.Message}");
                throw;
            }
        }

        public async Task<(bool Success, string Message)> AssumirChamadoAsync(int chamadoId)
        {
            try
            {
                Console.WriteLine($"[DEBUG] AssumirChamado - ChamadoId: {chamadoId}");
                Console.WriteLine($"[DEBUG] URL: /api/Chat/{chamadoId}/assumir");
                
                var response = await _apiService.PostAsync<ApiResponse<object>>($"/api/Chat/{chamadoId}/assumir", null);
                
                Console.WriteLine($"[DEBUG] Response recebido: {response != null}");
                if (response != null)
                {
                    Console.WriteLine($"[DEBUG] Response.Success: {response.Success}");
                    Console.WriteLine($"[DEBUG] Response.Message: {response.Message}");
                }

                if (response != null && response.Success)
                {
                    return (true, "Chamado assumido com sucesso!");
                }

                return (false, response?.Message ?? "Erro ao assumir chamado");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Exception no AssumirChamado: {ex.GetType().Name}");
                Console.WriteLine($"[ERRO] Message: {ex.Message}");
                Console.WriteLine($"[ERRO] StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[ERRO] InnerException: {ex.InnerException.Message}");
                }
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> FinalizarChamadoAsync(int chamadoId)
        {
            try
            {
                var response = await _apiService.PostAsync<ApiResponse<object>>($"/api/Chat/{chamadoId}/finalizar", null);

                if (response != null && response.Success)
                {
                    return (true, "Chamado finalizado com sucesso!");
                }

                return (false, response?.Message ?? "Erro ao finalizar chamado");
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> TransferirChamadoAsync(int chamadoId, int novoSuporteId)
        {
            try
            {
                var request = new { NovoSuporteId = novoSuporteId };
                var response = await _apiService.PostAsync<ApiResponse<object>>($"/api/Chamados/{chamadoId}/transferir", request);

                if (response != null && response.Success)
                {
                    return (true, "Chamado transferido com sucesso!");
                }

                return (false, response?.Message ?? "Erro ao transferir chamado");
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> EditarChamadoAsync(int chamadoId, EditarChamadoRequest request)
        {
            try
            {
                var response = await _apiService.PutAsync<ApiResponse<object>>($"/api/Chamados/{chamadoId}", request);

                if (response != null && response.Success)
                {
                    return (true, "Chamado atualizado com sucesso!");
                }

                return (false, response?.Message ?? "Erro ao atualizar chamado");
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> GerarRespostaIAAsync(int chamadoId)
        {
            try
            {
                var response = await _apiService.PostAsync<ApiResponse<object>>($"/api/Chamados/{chamadoId}/gerar-resposta-ia", null);

                if (response != null && response.Success)
                {
                    return (true, "Resposta da IA gerada com sucesso!");
                }

                return (false, response?.Message ?? "Erro ao gerar resposta da IA");
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<List<Usuario>?> GetSuportesAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<Usuario>>>("/api/Usuarios/suportes");
                return response?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting suportes: {ex.Message}");
                return null;
            }
        }
    }
}
