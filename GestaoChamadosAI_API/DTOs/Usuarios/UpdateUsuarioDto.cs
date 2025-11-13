using System.ComponentModel.DataAnnotations;

namespace GestaoChamadosAI_API.DTOs.Usuarios
{
    public class UpdateUsuarioDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;

        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        public string? Senha { get; set; } // Opcional na atualização

        [Required(ErrorMessage = "Tipo é obrigatório")]
        [RegularExpression("^(Cliente|Suporte|Administrador)$", ErrorMessage = "Tipo inválido")]
        public string Tipo { get; set; } = string.Empty;
    }
}
