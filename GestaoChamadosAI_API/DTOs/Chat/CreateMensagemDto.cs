using System.ComponentModel.DataAnnotations;

namespace GestaoChamadosAI_API.DTOs.Chat
{
    public class CreateMensagemDto
    {
        [Required(ErrorMessage = "Mensagem é obrigatória")]
        [StringLength(1000, ErrorMessage = "Mensagem deve ter no máximo 1000 caracteres")]
        public string Mensagem { get; set; } = string.Empty;
    }
}
