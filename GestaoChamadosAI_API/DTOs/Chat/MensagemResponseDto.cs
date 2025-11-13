namespace GestaoChamadosAI_API.DTOs.Chat
{
    /// <summary>
    /// DTO de resposta com dados de uma mensagem
    /// </summary>
    public class MensagemResponseDto
    {
        /// <summary>
        /// ID da mensagem
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID do chamado
        /// </summary>
        public int ChamadoId { get; set; }

        /// <summary>
        /// ID do usuário que enviou
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Nome do usuário que enviou
        /// </summary>
        public string NomeUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do usuário (Cliente, Suporte, Administrador)
        /// </summary>
        public string TipoUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Conteúdo da mensagem
        /// </summary>
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora da mensagem
        /// </summary>
        public DateTime DataEnvio { get; set; }

        /// <summary>
        /// Lista de anexos da mensagem
        /// </summary>
        public List<AnexoResponseDto> Anexos { get; set; } = new();
    }

    /// <summary>
    /// DTO de resposta com dados de um anexo
    /// </summary>
    public class AnexoResponseDto
    {
        /// <summary>
        /// ID do anexo
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do arquivo
        /// </summary>
        public string NomeArquivo { get; set; } = string.Empty;

        /// <summary>
        /// URL completa para acessar o arquivo
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do arquivo (extensão ou MIME type)
        /// </summary>
        public string Tipo { get; set; } = string.Empty;
    }
}
