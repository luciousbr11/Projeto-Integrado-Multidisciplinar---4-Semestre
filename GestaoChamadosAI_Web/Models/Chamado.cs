using System;
using System.Collections.Generic;

namespace GestaoChamadosAI_Web.Models
{
    public class Chamado
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public DateTime DataAbertura { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Aberto";

        // Campos da Análise de IA
        public string? CategoriaIA { get; set; }
        
        public string? SugestaoIA { get; set; }
        
        public string? Prioridade { get; set; }

        // Resposta gerada pela IA (Gemini)
        public string? RespostaIA { get; set; }

        // Feedback do usuário sobre a resposta da IA
        public bool? FeedbackResolvido { get; set; } // true = resolveu, false = náo resolveu, null = náo respondeu ainda
        public DateTime? DataFeedback { get; set; }

        // Suporte responsável pelo atendimento
        public int? SuporteResponsavelId { get; set; }
        public Usuario? SuporteResponsavel { get; set; }

        // Relacionamentos
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<MensagemChamado> Mensagens { get; set; } = new List<MensagemChamado>();
    }
}
