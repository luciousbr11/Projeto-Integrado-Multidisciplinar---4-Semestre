namespace GestaoChamadosAI_API.DTOs.Chamados
{
    public class ChamadoDto
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
        
        // Dados do usuário cliente
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        
        // Dados do suporte
        public int? SuporteResponsavelId { get; set; }
        public string? SuporteNome { get; set; }
        public string? SuporteEmail { get; set; }
        
        // Total de mensagens (útil para notificações)
        public int TotalMensagens { get; set; }
    }
}
