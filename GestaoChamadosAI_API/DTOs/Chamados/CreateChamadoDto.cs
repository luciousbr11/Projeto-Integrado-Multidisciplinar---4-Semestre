using System.ComponentModel.DataAnnotations;

namespace GestaoChamadosAI_API.DTOs.Chamados
{
    public class CreateChamadoDto
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(2000, ErrorMessage = "Descrição deve ter no máximo 2000 caracteres")]
        public string Descricao { get; set; } = string.Empty;
    }
}
