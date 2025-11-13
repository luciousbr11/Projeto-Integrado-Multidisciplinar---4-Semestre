using System.Text;
using System.Text.Json;

namespace GestaoChamadosAI_API.Services
{
    /// <summary>
    /// Serviço para integração com Google Gemini AI via API REST
    /// Responsável por categorização automática e geração de respostas
    /// </summary>
    public class GeminiService
    {
        private readonly string _apiKey;
        private string _modelName;
        private readonly ILogger<GeminiService> _logger;
        private readonly HttpClient _httpClient;
        private static readonly string[] ModelosFallback = { "gemini-2.0-flash", "gemini-2.5-flash", "gemini-pro-latest" };

        public GeminiService(IConfiguration configuration, ILogger<GeminiService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _apiKey = configuration["GeminiAI:ApiKey"] ?? throw new ArgumentNullException("GeminiAI:ApiKey não configurada");
            _modelName = configuration["GeminiAI:Model"] ?? "gemini-2.0-flash";
            _httpClient = httpClientFactory.CreateClient();
            
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogError("API Key do Gemini está vazia ou nula!");
                throw new ArgumentException("API Key do Gemini não pode estar vazia");
            }
            
            _logger.LogInformation($"[GEMINI] Serviço inicializado com modelo: {_modelName}");
        }

        private async Task<string> ChamarGeminiApiAsync(string prompt)
        {
            var modelosParaTestar = new[] { _modelName }.Concat(ModelosFallback.Where(m => m != _modelName)).ToArray();
            var versoes = new[] { "v1beta", "v1" };

            foreach (var versao in versoes)
            {
                foreach (var modelo in modelosParaTestar)
                {
                    try
                    {
                        var url = $"https://generativelanguage.googleapis.com/{versao}/models/{modelo}:generateContent?key={_apiKey}";
                        
                        var requestBody = new
                        {
                            contents = new[]
                            {
                                new
                                {
                                    parts = new[]
                                    {
                                        new { text = prompt }
                                    }
                                }
                            }
                        };

                        var json = JsonSerializer.Serialize(requestBody);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        
                        var response = await _httpClient.PostAsync(url, content);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                                continue;
                            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                                continue;
                            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                                continue;
                            
                            continue;
                        }

                        var jsonResponse = JsonDocument.Parse(responseBody);
                        var textResponse = jsonResponse.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text")
                            .GetString();

                        if (_modelName != modelo)
                        {
                            _modelName = modelo;
                        }

                        return textResponse ?? string.Empty;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Erro com {modelo} ({versao}): {ex.Message}");
                    }
                }
            }

            throw new Exception("Nenhum modelo Gemini funcionou");
        }

        public async Task<bool> TestarConexaoAsync()
        {
            try
            {
                var resposta = await ChamarGeminiApiAsync("Responda apenas com 'OK'");
                return !string.IsNullOrEmpty(resposta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar conexão com Gemini");
                return false;
            }
        }

        public async Task<string> CategorizarChamadoAsync(string titulo, string descricao)
        {
            try
            {
                var prompt = $@"Você é um especialista em categorização de chamados de suporte técnico de TI.
Analise o problema descrito abaixo e crie UMA categoria ESPECÍFICA e DESCRITIVA que melhor representa o problema.

REGRAS:
1. Seja ESPECÍFICO - exemplo: em vez de ""Hardware"", use ""Problema de Performance"", ""Falha de Impressora"", ""Teclado/Mouse com Defeito"", etc.
2. Use no MÁXIMO 3-4 palavras
3. Seja claro e direto
4. Retorne APENAS o nome da categoria, nada mais
5. Use maiúsculas nas iniciais (exemplo: ""Problema de Rede Wi-Fi"")

Título: {titulo}
Descrição: {descricao}

Categoria:";

                var resposta = await ChamarGeminiApiAsync(prompt);
                var categoria = resposta?.Trim() ?? "Problema Não Identificado";
                categoria = categoria.Replace("\"", "").Replace("'", "").Trim();
                
                if (categoria.Length > 50)
                {
                    categoria = categoria.Substring(0, 47) + "...";
                }

                _logger.LogInformation($"Chamado categorizado como: {categoria}");
                return categoria;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao categorizar chamado");
                return "Problema Não Identificado";
            }
        }

        public async Task<string> GerarRespostaAsync(string titulo, string descricao, string categoria)
        {
            try
            {
                var prompt = $@"Você é um assistente de suporte técnico.
Crie uma resposta SIMPLES, DIRETA e PRÁTICA para o problema abaixo.

REGRAS IMPORTANTES:
1. Use linguagem SIMPLES e DIRETA (sem termos técnicos complexos)
2. Organize a resposta em TÓPICOS NUMERADOS
3. NÃO use ""Prezado Cliente"" ou ""Atenciosamente""
4. Seja objetivo e vá direto ao ponto
5. Use emojis quando apropriado para facilitar visualização (🔧 ⚡ 📱 💡 ✅ etc)
6. Dê quantos passos forem necessários para resolver o problema

Categoria: {categoria}
Problema: {titulo}
Detalhes: {descricao}

Sua resposta:";

                var resposta = await ChamarGeminiApiAsync(prompt);
                _logger.LogInformation("Resposta gerada com sucesso pela IA");
                return resposta?.Trim() ?? "Não foi possível gerar uma resposta automática no momento.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao gerar resposta");
                throw;
            }
        }

        public async Task<string> AnalisarPrioridadeAsync(string titulo, string descricao)
        {
            try
            {
                var prompt = $@"Analise a urgência e impacto do seguinte chamado de suporte.
Retorne APENAS uma das seguintes prioridades:
- Baixa (problemas menores, sem impacto crítico)
- Média (problemas que afetam produtividade mas têm workarounds)
- Alta (problemas críticos que impedem o trabalho)

IMPORTANTE: Retorne APENAS a palavra: Baixa, Média ou Alta.

Título: {titulo}
Descrição: {descricao}

Prioridade:";

                var resposta = await ChamarGeminiApiAsync(prompt);
                var prioridade = resposta?.Trim() ?? "Média";

                var prioridadesValidas = new[] { "Baixa", "Média", "Alta" };
                if (!prioridadesValidas.Contains(prioridade))
                {
                    _logger.LogWarning($"Prioridade inválida retornada pela IA: {prioridade}. Usando 'Média'");
                    prioridade = "Média";
                }

                _logger.LogInformation($"Prioridade analisada: {prioridade}");
                return prioridade;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao analisar prioridade");
                return "Média";
            }
        }
    }
}
