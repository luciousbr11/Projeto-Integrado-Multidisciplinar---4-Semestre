using System.Text;
using System.Text.Json;

namespace GestaoChamadosAI_Web.Services
{
    /// <summary>
    /// Serviço para integraçáo com Google Gemini AI via API REST
    /// Responsável por categorizaçáo automática e geraçáo de respostas
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
            _apiKey = configuration["GeminiAI:ApiKey"] ?? throw new ArgumentNullException("GeminiAI:ApiKey náo configurada");
            _modelName = configuration["GeminiAI:Model"] ?? "gemini-2.0-flash";
            _httpClient = httpClientFactory.CreateClient();
            
            // Validações
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogError("API Key do Gemini está vazia ou nula!");
                throw new ArgumentException("API Key do Gemini náo pode estar vazia");
            }
            
            // Log de inicializaçáo
            _logger.LogInformation($"[GEMINI] Serviço inicializado com modelo: {_modelName}");
            _logger.LogInformation($"[GEMINI] API Key configurada (tamanho: {_apiKey.Length})");
            Console.WriteLine($"[GEMINI] Inicializado - Modelo primário: {_modelName}");
            Console.WriteLine($"[GEMINI] Modelos de fallback: {string.Join(", ", ModelosFallback)}");
        }

        /// <summary>
        /// Faz uma chamada à API do Gemini com fallback automático de modelos e versões de API
        /// </summary>
        private async Task<string> ChamarGeminiApiAsync(string prompt)
        {
            Exception? ultimoErro = null;
            var tentativas = new List<string>();
            
            // Tenta o modelo configurado primeiro
            var modelosParaTestar = new[] { _modelName }.Concat(ModelosFallback.Where(m => m != _modelName)).ToArray();
            
            // Tenta ambas versões da API (v1beta é mais compatível com APIs antigas)
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

                        Console.WriteLine($"[GEMINI] Tentando: {modelo} (API {versao})");
                        
                        var response = await _httpClient.PostAsync(url, content);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        Console.WriteLine($"[GEMINI] Status: {response.StatusCode}");

                        if (!response.IsSuccessStatusCode)
                        {
                            tentativas.Add($"{modelo} ({versao}): HTTP {response.StatusCode}");
                            
                            // Se for 404, tenta o próximo
                            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            {
                                Console.WriteLine($"[GEMINI] {modelo} náo disponível em {versao}, tentando próximo...");
                                continue;
                            }
                            
                            // Se for 400, pode ser formato errado
                            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                            {
                                Console.WriteLine($"[GEMINI] Bad Request (400): {responseBody.Substring(0, Math.Min(200, responseBody.Length))}");
                                continue;
                            }
                            
                            // Se for 403, API náo habilitada
                            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                            {
                                Console.WriteLine($"[GEMINI] Forbidden (403): API Key sem permissáo");
                                continue;
                            }
                            
                            Console.WriteLine($"[GEMINI] Erro: {responseBody.Substring(0, Math.Min(200, responseBody.Length))}");
                            continue;
                        }

                        var jsonResponse = JsonDocument.Parse(responseBody);
                        var textResponse = jsonResponse.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text")
                            .GetString();

                        // Sucesso! Atualiza o modelo padráo
                        if (_modelName != modelo)
                        {
                            Console.WriteLine($"[GEMINI] ✅ {modelo} ({versao}) funcionou! Usando como padráo.");
                            _modelName = modelo;
                        }

                        Console.WriteLine($"[GEMINI] Resposta: {(textResponse?.Length > 80 ? textResponse.Substring(0, 80) : textResponse)}...");
                        return textResponse ?? string.Empty;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[GEMINI] Erro com {modelo} ({versao}): {ex.Message}");
                        tentativas.Add($"{modelo} ({versao}): {ex.GetType().Name}");
                        ultimoErro = ex;
                    }
                }
            }

            // Se chegou aqui, nenhum modelo funcionou
            var mensagemDetalhada = $"Nenhum modelo Gemini funcionou.\nTentativas: {string.Join(", ", tentativas)}";
            Console.WriteLine($"[GEMINI] {mensagemDetalhada}");
            throw new Exception(mensagemDetalhada);
        }

        /// <summary>
        /// Testa a conexáo com a API do Gemini
        /// </summary>
        public async Task<bool> TestarConexaoAsync()
        {
            try
            {
                Console.WriteLine("[GEMINI] Testando conexáo...");
                var resposta = await ChamarGeminiApiAsync("Responda apenas com 'OK'");
                var sucesso = !string.IsNullOrEmpty(resposta);
                Console.WriteLine($"[GEMINI] Teste finalizado - Sucesso: {sucesso}");
                return sucesso;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GEMINI] ERRO no teste: {ex.Message}");
                _logger.LogError(ex, "Erro ao testar conexáo com Gemini");
                return false;
            }
        }

        /// <summary>
        /// Categoriza automaticamente um chamado baseado no título e descriçáo
        /// </summary>
        public async Task<string> CategorizarChamadoAsync(string titulo, string descricao)
        {
            try
            {
                var prompt = $@"Você é um especialista em categorizaçáo de chamados de suporte técnico de TI.
Analise o problema descrito abaixo e crie UMA categoria ESPECÍFICA e DESCRITIVA que melhor representa o problema.

REGRAS:
1. Seja ESPECÍFICO - exemplo: em vez de ""Hardware"", use ""Problema de Performance"", ""Falha de Impressora"", ""Teclado/Mouse com Defeito"", etc.
2. Use no MÁXIMO 3-4 palavras
3. Seja claro e direto
4. Retorne APENAS o nome da categoria, nada mais
5. Use maiúsculas nas iniciais (exemplo: ""Problema de Rede Wi-Fi"")

Exemplos de boas categorias:
- ""Computador Lento""
- ""Erro de Impressão""
- ""Sem Acesso à Internet""
- ""Problema de Login""
- ""Instalação de Software""
- ""Tela com Defeito""
- ""Falha no E-mail""

Título: {titulo}
Descriçáo: {descricao}

Categoria:";

                var resposta = await ChamarGeminiApiAsync(prompt);
                var categoria = resposta?.Trim() ?? "Problema Não Identificado";

                // Remove aspas ou caracteres extras se houver
                categoria = categoria.Replace("\"", "").Replace("'", "").Trim();
                
                // Limita o tamanho da categoria
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
                Console.WriteLine($"[GEMINI] ERRO na categorizaçáo: {ex.Message}");
                return "Problema Não Identificado"; // Fallback em caso de erro
            }
        }

        /// <summary>
        /// Gera uma resposta automática para um chamado
        /// </summary>
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

FORMATO DA RESPOSTA:
[Uma linha explicando o problema de forma simples]

Como resolver:

1. [Primeiro passo - claro e simples]
2. [Segundo passo - claro e simples]
3. [Terceiro passo - claro e simples]
(continue com quantos passos forem necessários)

[Dica adicional se necessário]

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
                Console.WriteLine($"[GEMINI] ERRO ao gerar resposta: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Analisa a prioridade sugerida para um chamado
        /// </summary>
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
Descriçáo: {descricao}

Prioridade:";

                var resposta = await ChamarGeminiApiAsync(prompt);
                var prioridade = resposta?.Trim() ?? "Média";

                // Valida se a prioridade retornada é válida
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
                return "Média"; // Fallback em caso de erro
            }
        }
    }
}
