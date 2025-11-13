using Newtonsoft.Json;

namespace GestaoChamadosAI_MAUI.Models
{
    public class Usuario
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("nome")]
        public string Nome { get; set; } = string.Empty;
        
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;
        
        [JsonProperty("tipo")]
        public string Tipo { get; set; } = string.Empty; // Cliente, Suporte, Administrador
        
        [JsonProperty("dataCadastro")]
        public DateTime DataCadastro { get; set; }
        
        [JsonProperty("totalChamados")]
        public int TotalChamados { get; set; }
    }

    public class UsuarioDetalhado : Usuario
    {
        [JsonProperty("chamadosAbertos")]
        public int ChamadosAbertos { get; set; }
        
        [JsonProperty("chamadosAtendidos")]
        public int ChamadosAtendidos { get; set; }
    }

    public class CreateUsuarioRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class UpdateUsuarioRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class AlterarSenhaRequest
    {
        public string NovaSenha { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public Usuario Usuario { get; set; } = new();
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }

    // Modelos para Relatórios
    public class RelatorioUsuarios
    {
        public int TotalUsuarios { get; set; }
        public int Clientes { get; set; }
        public int Suportes { get; set; }
        public int Administradores { get; set; }
        public List<UsuarioDetalhado> Usuarios { get; set; } = new();
    }

    public class RelatorioChamadosPeriodo
    {
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public int Total { get; set; }
        public int Abertos { get; set; }
        public int EmAtendimento { get; set; }
        public int AguardandoCliente { get; set; }
        public int Fechados { get; set; }
        public List<PrioridadeCount> PorPrioridade { get; set; } = new();
        public List<CategoriaCount> PorCategoria { get; set; } = new();
        public List<ChamadoResumo> Chamados { get; set; } = new();
    }

    public class PrioridadeCount
    {
        public string Prioridade { get; set; } = string.Empty;
        public int Total { get; set; }
    }

    public class CategoriaCount
    {
        public string Categoria { get; set; } = string.Empty;
        public int Total { get; set; }
    }

    public class ChamadoResumo
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public string CategoriaIA { get; set; } = string.Empty;
        public DateTime DataAbertura { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Suporte { get; set; } = string.Empty;
    }

    public class RelatorioSuportes
    {
        public int TotalSuportes { get; set; }
        public List<SuporteDetalhado> Suportes { get; set; } = new();
    }

    public class SuporteDetalhado
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public int ChamadosAtivos { get; set; }
        public int ChamadosFinalizados { get; set; }
        public int TotalChamados { get; set; }
        public List<PrioridadeCount> ChamadosPorPrioridade { get; set; } = new();
    }

    public class RelatorioCategorias
    {
        public List<CategoriaDetalhada> Categorias { get; set; } = new();
    }

    public class CategoriaDetalhada
    {
        public string Categoria { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Abertos { get; set; }
        public int EmAtendimento { get; set; }
        public int AguardandoCliente { get; set; }
        public int Fechados { get; set; }
        
        public double ProgressoFechamento => Total > 0 ? (double)Fechados / Total : 0;
    }
}
