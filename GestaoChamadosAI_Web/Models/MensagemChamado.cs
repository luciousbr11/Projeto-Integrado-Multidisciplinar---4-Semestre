using System;
using System.Collections.Generic;

namespace GestaoChamadosAI_Web.Models
{
    public class MensagemChamado
    {
        public int Id { get; set; }

        public int ChamadoId { get; set; }
        public Chamado? Chamado { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public string Mensagem { get; set; } = string.Empty;

        public DateTime DataEnvio { get; set; } = DateTime.Now;

        public bool LidaPorCliente { get; set; } = false;

        public bool LidaPorSuporte { get; set; } = false;

        // Anexos
        public ICollection<AnexoMensagem> Anexos { get; set; } = new List<AnexoMensagem>();
    }
}
