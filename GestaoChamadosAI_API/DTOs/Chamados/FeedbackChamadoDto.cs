using System.ComponentModel.DataAnnotations;

namespace GestaoChamadosAI_API.DTOs.Chamados
{
    public class FeedbackChamadoDto
    {
        [Required(ErrorMessage = "Feedback é obrigatório")]
        public bool Resolvido { get; set; }
    }
}
