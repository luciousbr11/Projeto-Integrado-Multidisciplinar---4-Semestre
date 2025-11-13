using System.ComponentModel.DataAnnotations;

namespace GestaoChamadosAI_API.DTOs.Chat
{
    /// <summary>
    /// DTO para enviar uma mensagem em um chamado
    /// </summary>
    public class EnviarMensagemDto
    {
        /// <summary>
        /// Conteúdo da mensagem
        /// </summary>
        [StringLength(2000, ErrorMessage = "A mensagem deve ter no máximo 2000 caracteres")]
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// URLs dos anexos (já uploadados via /api/Upload)
        /// </summary>
        public List<string> AnexosUrls { get; set; } = new();
    }
}
