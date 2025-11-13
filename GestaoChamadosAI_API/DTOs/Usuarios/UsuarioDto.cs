namespace GestaoChamadosAI_API.DTOs.Usuarios
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public int TotalChamados { get; set; }
    }
}
