using System.Collections.Generic;

namespace GestaoChamadosAI_Web.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string Tipo { get; set; } // Ex: "Cliente", "Suporte", "Administrador"

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Relacionamento: Um usuário pode ter vários chamados
        public ICollection<Chamado>? Chamados { get; set; }
    }
}
