namespace GestaoChamadosAI_MAUI.Models
{
    public class Chamado
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Prioridade { get; set; }
        public string? CategoriaIA { get; set; }
        public string? SugestaoIA { get; set; }
        public DateTime DataAbertura { get; set; }
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public int? SuporteResponsavelId { get; set; }
        public string? NomeSuporteResponsavel { get; set; }
        public Usuario? SuporteResponsavel { get; set; }
        public string? RespostaIA { get; set; }
        public bool? FeedbackResolvido { get; set; }
        public List<MensagemChamado> Mensagens { get; set; } = new();
    }

    public class ChamadoListItem
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Prioridade { get; set; }
        public DateTime DataAbertura { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string? NomeSuporteResponsavel { get; set; }
        public int? SuporteResponsavelId { get; set; }
        public string UsuarioNome => NomeUsuario;
    }

    public class NovoChamadoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }

    public class EditarChamadoRequest
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public string? Status { get; set; }
        public string? Prioridade { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
