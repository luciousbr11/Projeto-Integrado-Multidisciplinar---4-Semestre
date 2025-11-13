using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestaoChamadosAI_API.Models
{
    public class AnexoMensagem
    {
        [Key]
        public int Id { get; set; }

        public int MensagemChamadoId { get; set; }
        
        [JsonIgnore]
        public MensagemChamado? MensagemChamado { get; set; }

        [Required]
        [StringLength(255)]
        public string NomeArquivo { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string CaminhoArquivo { get; set; } = string.Empty;

        [StringLength(100)]
        public string TipoArquivo { get; set; } = string.Empty;

        public long TamanhoBytes { get; set; }

        public DateTime DataUpload { get; set; } = DateTime.Now;
    }
}
