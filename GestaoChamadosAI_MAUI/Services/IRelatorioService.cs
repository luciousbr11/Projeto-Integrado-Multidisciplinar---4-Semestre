using GestaoChamadosAI_MAUI.Models;

namespace GestaoChamadosAI_MAUI.Services
{
    public interface IRelatorioService
    {
        Task<RelatorioUsuarios?> GetRelatorioUsuariosAsync();
        Task<RelatorioChamadosPeriodo?> GetRelatorioChamadosPeriodoAsync(DateTime? dataInicio, DateTime? dataFim);
        Task<RelatorioSuportes?> GetRelatorioSuportesAsync();
        Task<RelatorioCategorias?> GetRelatorioCategoriasAsync();
    }
}
