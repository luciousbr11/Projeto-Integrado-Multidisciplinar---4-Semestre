using System.ComponentModel.DataAnnotations;

namespace GestaoChamadosAI_API.DTOs.Chamados
{
    public class TransferirChamadoDto
    {
        [Required(ErrorMessage = "ID do novo suporte é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "ID do suporte inválido")]
        public int NovoSuporteId { get; set; }
    }
}
