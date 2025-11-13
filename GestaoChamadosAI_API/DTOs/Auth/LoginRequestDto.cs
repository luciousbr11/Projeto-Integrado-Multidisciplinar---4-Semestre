using System.ComponentModel.DataAnnotations;

namespace GestaoChamadosAI_API.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;
    }
}
