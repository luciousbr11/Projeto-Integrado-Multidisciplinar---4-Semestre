using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoChamadosAI_Web.Services
{
    /// <summary>
    /// Serviço de Inteligência Artificial para análise de chamados e sugestáo de soluções.
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
                    new[] { "senha", "login", "acesso", "autenticaçáo", "entrar" },
                    "Soluçáo Sugerida: Problema de acesso detectado. Tente redefinir sua senha através da opçáo 'Esqueci minha senha' ou entre em contato com o administrador do sistema para verificar as permissões de sua conta."
                },
                {
                    new[] { "lento", "travando", "performance", "demora", "carregando" },
                    "Soluçáo Sugerida: Problema de performance identificado. Recomendamos: 1) Limpar cache do navegador, 2) Verificar conexáo de internet, 3) Fechar outras abas/programas que possam estar consumindo recursos."
                },
                {
                    new[] { "erro", "bug", "falha", "quebrou", "náo funciona" },
                    "Soluçáo Sugerida: Erro no sistema detectado. Tente: 1) Recarregar a página (F5), 2) Limpar cookies do navegador, 3) Tentar em outro navegador. Se o problema persistir, a equipe técnica irá analisar o log de erros."
                },
                {
                    new[] { "impressora", "imprimir", "impressáo" },
                    "Soluçáo Sugerida: Problema com impressáo. Verifique: 1) Se a impressora está ligada e conectada, 2) Se há papel e tinta/toner, 3) Se os drivers estáo atualizados, 4) Tente reiniciar o serviço de spooler de impressáo."
                },
                {
                    new[] { "email", "e-mail", "mensagem", "correio" },
                    "Soluçáo Sugerida: Problema com e-mail. Verifique: 1) Configurações de servidor SMTP/IMAP, 2) Se a caixa de entrada náo está cheia, 3) Filtros de spam, 4) Credenciais de acesso estáo corretas."
                },
                {
                    new[] { "rede", "internet", "conexáo", "wifi", "conectar" },
                    "Soluçáo Sugerida: Problema de conexáo detectado. Tente: 1) Verificar se o cabo de rede está conectado ou reconectar ao WiFi, 2) Reiniciar o modem/roteador, 3) Executar diagnóstico de rede do Windows, 4) Verificar com o provedor de internet."
                },
                {
                    new[] { "instalaçáo", "instalar", "software", "programa", "aplicativo" },
                    "Soluçáo Sugerida: Problema com instalaçáo. Verifique: 1) Se possui permissões de administrador, 2) Se há espaço em disco suficiente, 3) Se o antivírus náo está bloqueando, 4) Compatibilidade com o sistema operacional."
                },
                {
                    new[] { "backup", "recuperaçáo", "perdeu", "arquivo", "documento" },
                    "Soluçáo Sugerida: Problema com arquivos. Tente: 1) Verificar lixeira do sistema, 2) Verificar backup automático (OneDrive, Google Drive), 3) Usar ferramentas de recuperaçáo de arquivos, 4) Contatar suporte para restauraçáo de backup do servidor."
                },
                {
                    new[] { "tela", "monitor", "display", "vídeo", "imagem" },
                    "Soluçáo Sugerida: Problema de vídeo/display. Verifique: 1) Cabos de vídeo estáo conectados corretamente, 2) Resoluçáo de tela configurada adequadamente, 3) Drivers de vídeo atualizados, 4) Tente outro monitor para isolar o problema."
                },
                {
                    new[] { "mouse", "teclado", "periférico" },
                    "Soluçáo Sugerida: Problema com periféricos. Tente: 1) Reconectar o dispositivo (USB), 2) Testar em outra porta USB, 3) Substituir baterias (se sem fio), 4) Atualizar drivers do dispositivo."
                }
            };
        }

        /// <summary>
        /// Analisa o título e descriçáo de um chamado e retorna uma sugestáo de soluçáo baseada na base de conhecimento.
        /// O método procura por palavras-chave no texto e retorna a soluçáo mais relevante.
        /// </summary>
        /// <param name="titulo">Título do chamado</param>
        /// <param name="descricao">Descriçáo detalhada do problema</param>
        /// <returns>String com a sugestáo de soluçáo ou mensagem padráo se nenhuma correspondência for encontrada</returns>
        public string AnalisarChamado(string titulo, string descricao)
        {
            // Verifica se os parâmetros náo sáo nulos ou vazios
            if (string.IsNullOrWhiteSpace(titulo) && string.IsNullOrWhiteSpace(descricao))
            {
                return "Por favor, forneça mais detalhes sobre o problema para que possamos sugerir uma soluçáo adequada.";
            }

            // Combina título e descriçáo e converte para minúsculas para facilitar a comparaçáo
            string textoCompleto = $"{titulo} {descricao}".ToLower();

            // Lista para armazenar sugestões encontradas com sua pontuaçáo (quantidade de palavras-chave encontradas)
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

            // Se encontrou sugestões, retorna a com maior pontuaçáo (mais palavras-chave encontradas)
            if (sugestoesEncontradas.Any())
            {
                var melhorSugestao = sugestoesEncontradas.OrderByDescending(s => s.pontuacao).First();
                return melhorSugestao.solucao;
            }

            // Retorna mensagem padráo se nenhuma palavra-chave foi encontrada
            return "Sugestáo da IA: Náo foi possível identificar automaticamente uma soluçáo para este problema. " +
                   "Um técnico especializado irá analisar seu chamado e entrar em contato em breve. " +
                   "Tempo estimado de resposta: 24-48 horas.";
        }

        /// <summary>
        /// Retorna uma análise de prioridade baseada em palavras-chave críticas.
        /// Ajuda a classificar a urgência do chamado automaticamente.
        /// </summary>
        /// <param name="titulo">Título do chamado</param>
        /// <param name="descricao">Descriçáo do problema</param>
        /// <returns>String indicando o nível de prioridade: "Alta", "Média" ou "Baixa"</returns>
        public string ClassificarPrioridade(string titulo, string descricao)
        {
            string textoCompleto = $"{titulo} {descricao}".ToLower();

            // Palavras-chave que indicam alta prioridade
            string[] palavrasAltaPrioridade = { "urgente", "crítico", "parado", "produçáo", "sistema fora", "emergência", "travado", "náo consigo trabalhar" };

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

            // Se náo encontrou palavras-chave específicas, retorna prioridade baixa
            return "Baixa";
        }

        /// <summary>
        /// Retorna estatísticas sobre a eficácia da IA (para uso futuro em dashboard).
        /// </summary>
        /// <returns>String com informações sobre a base de conhecimento</returns>
        public string ObterEstatisticas()
        {
            int totalCategorias = _baseConhecimento.Count;
            int totalPalavrasChave = _baseConhecimento.Sum(x => x.Key.Length);

            return $"Base de Conhecimento: {totalCategorias} categorias de problemas com {totalPalavrasChave} palavras-chave cadastradas.";
        }
    }
}
