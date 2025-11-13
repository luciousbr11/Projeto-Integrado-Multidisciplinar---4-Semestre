using System.ComponentModel.DataAnnotations;

namespace GestaoChamadosAI_API.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Token é obrigatório")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Refresh Token é obrigatório")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
