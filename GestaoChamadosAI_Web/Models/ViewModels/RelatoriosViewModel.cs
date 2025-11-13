namespace GestaoChamadosAI_Web.Models.ViewModels
{
    /// <summary>
    /// ViewModel para consolidar dados de relatórios administrativos.
    /// </summary>
    public class RelatoriosViewModel
    {
        // Relatório de Usuários Cadastrados
        public List<UsuarioRelatorio>? Usuarios { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalClientes { get; set; }
        public int TotalSuportes { get; set; }
        public int TotalAdministradores { get; set; }

        // Relatório de Chamados por Período
        public List<ChamadoRelatorio>? ChamadosPorPeriodo { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int TotalChamadosPeriodo { get; set; }
        public int ChamadosAbertos { get; set; }
        public int ChamadosEmAndamento { get; set; }
        public int ChamadosConcluidos { get; set; }

        // Relatório de Chamados por Suporte
        public List<SuporteRelatorio>? ChamadosPorSuporte { get; set; }
        public int TotalChamadosAtendidos { get; set; }
    }

    /// <summary>
    /// Representa um usuário no relatório.
    /// </summary>
    public class UsuarioRelatorio
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public int TotalChamados { get; set; }
    }

    /// <summary>
    /// Representa um chamado no relatório de período.
    /// </summary>
    public class ChamadoRelatorio
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Prioridade { get; set; }
        public string? CategoriaIA { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public string? SuporteNome { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataResolucao { get; set; }
    }

    /// <summary>
    /// Representa estatísticas de atendimento de um suporte.
    /// </summary>
    public class SuporteRelatorio
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalChamados { get; set; }
        public int ChamadosAbertos { get; set; }
        public int ChamadosEmAndamento { get; set; }
        public int ChamadosConcluidos { get; set; }
        public double? TempoMedioResolucao { get; set; } // Em horas
        public DateTime? UltimoAtendimento { get; set; }
    }
}
