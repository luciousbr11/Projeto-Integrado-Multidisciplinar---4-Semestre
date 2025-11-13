using GestaoChamadosAI_MAUI.Models;

namespace GestaoChamadosAI_MAUI.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IApiService _apiService;

        public RelatorioService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<RelatorioUsuarios?> GetRelatorioUsuariosAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("RelatorioService: Chamando API /api/Relatorios/usuarios");
                var response = await _apiService.GetAsync<ApiResponse<RelatorioUsuarios>>("/api/Relatorios/usuarios");
                
                System.Diagnostics.Debug.WriteLine($"RelatorioService: Response recebido - Success: {response?.Success}, Data: {response?.Data != null}");
                
                if (response?.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"RelatorioService: Total usuários: {response.Data.TotalUsuarios}");
                    System.Diagnostics.Debug.WriteLine($"RelatorioService: Lista usuários count: {response.Data.Usuarios?.Count ?? 0}");
                }
                
                return response?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RelatorioService: ERRO - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"RelatorioService: Stack - {ex.StackTrace}");
                throw; // Propagar a exceção em vez de retornar null
            }
        }

        public async Task<RelatorioChamadosPeriodo?> GetRelatorioChamadosPeriodoAsync(DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                var url = "/api/Relatorios/chamados-periodo";
                
                var queryParams = new List<string>();
                if (dataInicio.HasValue)
                    queryParams.Add($"dataInicio={dataInicio.Value:yyyy-MM-dd}");
                if (dataFim.HasValue)
                    queryParams.Add($"dataFim={dataFim.Value:yyyy-MM-dd}");

                if (queryParams.Any())
                    url += "?" + string.Join("&", queryParams);

                var response = await _apiService.GetAsync<ApiResponse<RelatorioChamadosPeriodo>>(url);
                
                // Ajustar estrutura do response
                if (response?.Data != null && response.Data.GetType().GetProperty("periodo") != null)
                {
                    var data = response.Data as dynamic;
                    var relatorio = new RelatorioChamadosPeriodo
                    {
                        Inicio = data.periodo.inicio,
                        Fim = data.periodo.fim,
                        Total = data.total,
                        Abertos = data.abertos,
                        EmAtendimento = data.emAtendimento,
                        AguardandoCliente = data.aguardandoCliente,
                        Fechados = data.fechados
                    };
                    
                    // Converter porPrioridade
                    if (data.porPrioridade != null)
                    {
                        foreach (var item in data.porPrioridade)
                        {
                            relatorio.PorPrioridade.Add(new PrioridadeCount 
                            { 
                                Prioridade = item.prioridade, 
                                Total = item.total 
                            });
                        }
                    }
                    
                    // Converter porCategoria
                    if (data.porCategoria != null)
                    {
                        foreach (var item in data.porCategoria)
                        {
                            relatorio.PorCategoria.Add(new CategoriaCount 
                            { 
                                Categoria = item.categoria, 
                                Total = item.total 
                            });
                        }
                    }
                    
                    // Converter chamados
                    if (data.chamados != null)
                    {
                        foreach (var chamado in data.chamados)
                        {
                            relatorio.Chamados.Add(new ChamadoResumo
                            {
                                Id = chamado.Id,
                                Titulo = chamado.Titulo,
                                Status = chamado.Status,
                                Prioridade = chamado.Prioridade,
                                CategoriaIA = chamado.CategoriaIA,
                                DataAbertura = chamado.DataAbertura,
                                Cliente = chamado.Cliente,
                                Suporte = chamado.Suporte
                            });
                        }
                    }
                    
                    return relatorio;
                }
                
                return response?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<RelatorioSuportes?> GetRelatorioSuportesAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<RelatorioSuportes>>("/api/Relatorios/suportes");
                return response?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<RelatorioCategorias?> GetRelatorioCategoriasAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<CategoriaDetalhada>>>("/api/Relatorios/categorias");
                
                if (response?.Data != null)
                {
                    return new RelatorioCategorias
                    {
                        Categorias = response.Data
                    };
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
