using System;
using System.Text.Json.Serialization;

namespace GestaoChamadosAI_API.Models
{
    public class MensagemChamado
    {
        public int Id { get; set; }

        public int ChamadoId { get; set; }
        
        [JsonIgnore]
        public Chamado? Chamado { get; set; }

        public int UsuarioId { get; set; }
        
        [JsonIgnore]
        public Usuario? Usuario { get; set; }

        public string Mensagem { get; set; } = string.Empty;

        public DateTime DataEnvio { get; set; } = DateTime.Now;

        public bool LidaPorCliente { get; set; } = false;

        public bool LidaPorSuporte { get; set; } = false;

        // Relacionamento: Uma mensagem pode ter vários anexos
        [JsonIgnore]
        public List<AnexoMensagem> Anexos { get; set; } = new();
    }
}
