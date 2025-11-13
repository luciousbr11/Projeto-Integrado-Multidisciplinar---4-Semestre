using System.Linq;

namespace GestaoChamadosAI_MAUI.Models
{
    public class MensagemChamado
    {
        public int Id { get; set; }
        public int ChamadoId { get; set; }
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }
        
        // Propriedade para UI - indica se é mensagem do usuário atual
        public bool IsMinhaMensagem { get; set; }
        
        // Lista de anexos
        public List<AnexoMensagem> Anexos { get; set; } = new List<AnexoMensagem>();
        
        // Propriedade auxiliar para binding - null-safe
        public bool TemAnexos => Anexos != null && Anexos.Any();
    }

    public class AnexoMensagem
    {
        public int Id { get; set; }
        public string NomeArquivo { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        
        // Propriedades auxiliares para UI
        public bool IsImage
        {
            get
            {
                if (string.IsNullOrEmpty(Tipo))
                    return false;
                    
                var extensoes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", "jpg", "jpeg", "png", "gif", "bmp", "webp" };
                return extensoes.Any(ext => Tipo.ToLower().Contains(ext));
            }
        }
        
        public string IconeArquivo => IsImage ? "📷" : "📎";
    }

    public class EnviarMensagemRequest
    {
        public string Mensagem { get; set; } = string.Empty;
    }

    public class DashboardEstatisticas
    {
        public MeusChamadosStats MeusChamados { get; set; } = new();
        public PerformanceStats? Performance { get; set; }
    }

    public class MeusChamadosStats
    {
        public int Total { get; set; }
        public int Abertos { get; set; }
        public int EmAtendimento { get; set; }
        public int AguardandoMinhaResposta { get; set; }
        public int Fechados { get; set; }
    }

    public class PerformanceStats
    {
        public int ChamadosResolvidos { get; set; }
        public int ChamadosEmAtendimento { get; set; }
    }

    public class UploadResponse
    {
        public bool Success { get; set; }
        public string? NomeArquivo { get; set; }
        public string? Url { get; set; }
        public long Tamanho { get; set; }
        public string? Tipo { get; set; }
    }
}
