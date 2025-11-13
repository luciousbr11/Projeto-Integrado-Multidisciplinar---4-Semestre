using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoChamadosAI_API.Services
{
    /// <summary>
    /// Serviço de Inteligência Artificial para análise de chamados e sugestão de soluções.
    /// Este serviço utiliza análise de palavras-chave para sugerir soluções automáticas.
    /// </summary>
    public class IAService
    {
        // Base de conhecimento com palavras-chave e suas respectivas soluções
        private readonly Dictionary<string[], string> _baseConhecimento;

        public IAService()
        {
            // Inicializa a base de conhecimento com problemas comuns e suas soluções
            _baseConhecimento = new Dictionary<string[], string>
            {
                {
                    new[] { "senha", "login", "acesso", "autenticação", "entrar" },
                    "Solução Sugerida: Problema de acesso detectado. Tente redefinir sua senha através da opção 'Esqueci minha senha' ou entre em contato com o administrador do sistema para verificar as permissões de sua conta."
                },
                {
                    new[] { "lento", "travando", "performance", "demora", "carregando" },
                    "Solução Sugerida: Problema de performance identificado. Recomendamos: 1) Limpar cache do navegador, 2) Verificar conexão de internet, 3) Fechar outras abas/programas que possam estar consumindo recursos."
                },
                {
                    new[] { "erro", "bug", "falha", "quebrou", "não funciona" },
                    "Solução Sugerida: Erro no sistema detectado. Tente: 1) Recarregar a página (F5), 2) Limpar cookies do navegador, 3) Tentar em outro navegador. Se o problema persistir, a equipe técnica irá analisar o log de erros."
                },
                {
                    new[] { "impressora", "imprimir", "impressão" },
                    "Solução Sugerida: Problema com impressão. Verifique: 1) Se a impressora está ligada e conectada, 2) Se há papel e tinta/toner, 3) Se os drivers estão atualizados, 4) Tente reiniciar o serviço de spooler de impressão."
                },
                {
                    new[] { "email", "e-mail", "mensagem", "correio" },
                    "Solução Sugerida: Problema com e-mail. Verifique: 1) Configurações de servidor SMTP/IMAP, 2) Se a caixa de entrada não está cheia, 3) Filtros de spam, 4) Credenciais de acesso estão corretas."
                },
                {
                    new[] { "rede", "internet", "conexão", "wifi", "conectar" },
                    "Solução Sugerida: Problema de conexão detectado. Tente: 1) Verificar se o cabo de rede está conectado ou reconectar ao WiFi, 2) Reiniciar o modem/roteador, 3) Executar diagnóstico de rede do Windows, 4) Verificar com o provedor de internet."
                },
                {
                    new[] { "instalação", "instalar", "software", "programa", "aplicativo" },
                    "Solução Sugerida: Problema com instalação. Verifique: 1) Se possui permissões de administrador, 2) Se há espaço em disco suficiente, 3) Se o antivírus não está bloqueando, 4) Compatibilidade com o sistema operacional."
                },
                {
                    new[] { "backup", "recuperação", "perdeu", "arquivo", "documento" },
                    "Solução Sugerida: Problema com arquivos. Tente: 1) Verificar lixeira do sistema, 2) Verificar backup automático (OneDrive, Google Drive), 3) Usar ferramentas de recuperação de arquivos, 4) Contatar suporte para restauração de backup do servidor."
                },
                {
                    new[] { "tela", "monitor", "display", "vídeo", "imagem" },
                    "Solução Sugerida: Problema de vídeo/display. Verifique: 1) Cabos de vídeo estão conectados corretamente, 2) Resolução de tela configurada adequadamente, 3) Drivers de vídeo atualizados, 4) Tente outro monitor para isolar o problema."
                },
                {
                    new[] { "mouse", "teclado", "periférico" },
                    "Solução Sugerida: Problema com periféricos. Tente: 1) Reconectar o dispositivo (USB), 2) Testar em outra porta USB, 3) Substituir baterias (se sem fio), 4) Atualizar drivers do dispositivo."
                }
            };
        }

        /// <summary>
        /// Analisa o título e descrição de um chamado e retorna uma sugestão de solução baseada na base de conhecimento.
        /// O método procura por palavras-chave no texto e retorna a solução mais relevante.
        /// </summary>
        public string AnalisarChamado(string titulo, string descricao)
        {
            // Verifica se os parâmetros não são nulos ou vazios
            if (string.IsNullOrWhiteSpace(titulo) && string.IsNullOrWhiteSpace(descricao))
            {
                return "Por favor, forneça mais detalhes sobre o problema para que possamos sugerir uma solução adequada.";
            }

            // Combina título e descrição e converte para minúsculas para facilitar a comparação
            string textoCompleto = $"{titulo} {descricao}".ToLower();

            // Lista para armazenar sugestões encontradas com sua pontuação (quantidade de palavras-chave encontradas)
            var sugestoesEncontradas = new List<(string solucao, int pontuacao)>();

            // Percorre toda a base de conhecimento
            foreach (var entrada in _baseConhecimento)
            {
                var palavrasChave = entrada.Key;
                var solucao = entrada.Value;
                int pontuacao = 0;

                // Conta quantas palavras-chave foram encontradas no texto
                foreach (var palavra in palavrasChave)
                {
                    if (textoCompleto.Contains(palavra.ToLower()))
                    {
                        pontuacao++;
                    }
                }

                // Se encontrou pelo menos uma palavra-chave, adiciona à lista de sugestões
                if (pontuacao > 0)
                {
                    sugestoesEncontradas.Add((solucao, pontuacao));
                }
            }

            // Se encontrou sugestões, retorna a com maior pontuação (mais palavras-chave encontradas)
            if (sugestoesEncontradas.Any())
            {
                var melhorSugestao = sugestoesEncontradas.OrderByDescending(s => s.pontuacao).First();
                return melhorSugestao.solucao;
            }

            // Retorna mensagem padrão se nenhuma palavra-chave foi encontrada
            return "Sugestão da IA: Não foi possível identificar automaticamente uma solução para este problema. " +
                   "Um técnico especializado irá analisar seu chamado e entrar em contato em breve. " +
                   "Tempo estimado de resposta: 24-48 horas.";
        }

        /// <summary>
        /// Retorna uma análise de prioridade baseada em palavras-chave críticas.
        /// Ajuda a classificar a urgência do chamado automaticamente.
        /// </summary>
        public string ClassificarPrioridade(string titulo, string descricao)
        {
            string textoCompleto = $"{titulo} {descricao}".ToLower();

            // Palavras-chave que indicam alta prioridade
            string[] palavrasAltaPrioridade = { "urgente", "crítico", "parado", "produção", "sistema fora", "emergência", "travado", "não consigo trabalhar" };

            // Palavras-chave que indicam média prioridade
            string[] palavrasMediaPrioridade = { "lento", "erro", "problema", "falha", "bug", "intermitente" };

            // Verifica prioridade alta
            foreach (var palavra in palavrasAltaPrioridade)
            {
                if (textoCompleto.Contains(palavra))
                {
                    return "Alta";
                }
            }

            // Verifica prioridade média
            foreach (var palavra in palavrasMediaPrioridade)
            {
                if (textoCompleto.Contains(palavra))
                {
                    return "Média";
                }
            }

            // Se não encontrou palavras-chave específicas, retorna prioridade baixa
            return "Baixa";
        }

        /// <summary>
        /// Retorna estatísticas sobre a eficácia da IA (para uso futuro em dashboard).
        /// </summary>
        public string ObterEstatisticas()
        {
            int totalCategorias = _baseConhecimento.Count;
            int totalPalavrasChave = _baseConhecimento.Sum(x => x.Key.Length);

            return $"Base de Conhecimento: {totalCategorias} categorias de problemas com {totalPalavrasChave} palavras-chave cadastradas.";
        }
    }
}
