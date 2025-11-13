using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GestaoChamadosAI_API.Models
{
    public class Chamado
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public DateTime DataAbertura { get; set; } = DateTime.Now;

        // Status possíveis: "Aberto", "Em Atendimento", "Aguardando Cliente", "Concluído", "Solucionado por IA"
        public string Status { get; set; } = "Aberto";

        // Campos da Análise de IA
        public string? CategoriaIA { get; set; }
        
        public string? SugestaoIA { get; set; }
        
        public string? Prioridade { get; set; } // "Baixa", "Média", "Alta"

        // Resposta gerada pela IA (Gemini)
        public string? RespostaIA { get; set; }

        // Feedback do usuário sobre a resposta da IA
        public bool? FeedbackResolvido { get; set; } // true = resolveu, false = não resolveu, null = não respondeu ainda
        public DateTime? DataFeedback { get; set; }

        // Suporte responsável pelo atendimento
        public int? SuporteResponsavelId { get; set; }
        
        [JsonIgnore]
        public Usuario? SuporteResponsavel { get; set; }

        // Relacionamentos
        public int UsuarioId { get; set; }
        
        [JsonIgnore]
        public Usuario? Usuario { get; set; }

        [JsonIgnore]
        public ICollection<MensagemChamado> Mensagens { get; set; } = new List<MensagemChamado>();
    }
}
