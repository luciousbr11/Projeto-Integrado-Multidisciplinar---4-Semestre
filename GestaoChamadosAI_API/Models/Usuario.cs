using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GestaoChamadosAI_API.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty; // Hash BCrypt

        public string Tipo { get; set; } = string.Empty; // "Cliente", "Suporte", "Administrador"

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Relacionamento: Um usuário pode ter vários chamados
        [JsonIgnore]
        public ICollection<Chamado>? Chamados { get; set; }
    }
}
