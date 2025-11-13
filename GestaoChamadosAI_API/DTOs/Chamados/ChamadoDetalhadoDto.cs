namespace GestaoChamadosAI_API.DTOs.Chamados
{
    public class ChamadoDetalhadoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataAbertura { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CategoriaIA { get; set; }
        public string? SugestaoIA { get; set; }
        public string? Prioridade { get; set; }
        public string? RespostaIA { get; set; }
        public bool? FeedbackResolvido { get; set; }
        public DateTime? DataFeedback { get; set; }
        
        // Dados do usuário
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        
        // Dados do suporte
        public int? SuporteResponsavelId { get; set; }
        public string? SuporteNome { get; set; }
        public string? SuporteEmail { get; set; }
        
        // Mensagens do chamado
        public List<MensagemDto> Mensagens { get; set; } = new();
    }

    public class MensagemDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }
        public bool LidaPorCliente { get; set; }
        public bool LidaPorSuporte { get; set; }
    }
}
