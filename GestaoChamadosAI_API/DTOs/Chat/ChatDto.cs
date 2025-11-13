namespace GestaoChamadosAI_API.DTOs.Chat
{
    public class ChatDto
    {
        public ChamadoInfoDto Chamado { get; set; } = null!;
        public List<MensagemChatDto> Mensagens { get; set; } = new();
    }

    public class ChamadoInfoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string UsuarioNome { get; set; } = string.Empty;
        public string? SuporteNome { get; set; }
    }

    public class MensagemChatDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }
        public bool LidaPorCliente { get; set; }
        public bool LidaPorSuporte { get; set; }
        public bool IsUsuarioAtual { get; set; }
    }
}
