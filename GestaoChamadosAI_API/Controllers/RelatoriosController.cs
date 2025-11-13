using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_API.Data;
using GestaoChamadosAI_API.Helpers;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;

namespace GestaoChamadosAI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class RelatoriosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RelatoriosController> _logger;

        public RelatoriosController(AppDbContext context, ILogger<RelatoriosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Relatório geral de usuários
        /// </summary>
        /// <returns>Lista de usuários com estatísticas</returns>
        [HttpGet("usuarios")]
        public async Task<ActionResult<ApiResponse<object>>> GetRelatorioUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .Select(u => new
                    {
                        u.Id,
                        u.Nome,
                        u.Email,
                        u.Tipo,
                        u.DataCadastro,
                        TotalChamados = _context.Chamados.Count(c => c.UsuarioId == u.Id),
                        ChamadosAbertos = _context.Chamados.Count(c => 
                            c.UsuarioId == u.Id && c.Status != "Fechado"),
                        ChamadosAtendidos = u.Tipo == "Suporte" 
                            ? _context.Chamados.Count(c => c.SuporteResponsavelId == u.Id)
                            : 0
                    })
                    .OrderBy(u => u.Tipo)
                    .ThenBy(u => u.Nome)
                    .ToListAsync();

                var resumo = new
                {
                    totalUsuarios = usuarios.Count,
                    clientes = usuarios.Count(u => u.Tipo == "Cliente"),
                    suportes = usuarios.Count(u => u.Tipo == "Suporte"),
                    administradores = usuarios.Count(u => u.Tipo == "Administrador"),
                    usuarios = usuarios
                };

                return Ok(ApiResponse<object>.SuccessResult(resumo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de usuários");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Erro ao gerar relatório"));
            }
        }

        /// <summary>
        /// Relatório de chamados por período
        /// </summary>
        /// <param name="dataInicio">Data inicial (formato: yyyy-MM-dd)</param>
        /// <param name="dataFim">Data final (formato: yyyy-MM-dd)</param>
        /// <returns>Chamados no período especificado</returns>
        [HttpGet("chamados-periodo")]
        public async Task<ActionResult<ApiResponse<object>>> GetRelatorioChamadosPorPeriodo(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                // Definir período padrão (últimos 30 dias)
                var inicio = dataInicio ?? DateTime.Now.AddDays(-30);
                var fim = dataFim ?? DateTime.Now;

                var chamados = await _context.Chamados
                    .Include(c => c.Usuario)
                    .Include(c => c.SuporteResponsavel)
                    .Where(c => c.DataAbertura >= inicio && c.DataAbertura <= fim)
                    .Select(c => new
                    {
                        c.Id,
                        c.Titulo,
                        c.Status,
                        c.Prioridade,
                        c.CategoriaIA,
                        c.DataAbertura,
                        Cliente = c.Usuario.Nome,
                        Suporte = c.SuporteResponsavel != null ? c.SuporteResponsavel.Nome : "Não atribuído"
                    })
                    .OrderByDescending(c => c.DataAbertura)
                    .ToListAsync();

                var estatisticas = new
                {
                    periodo = new { inicio, fim },
                    total = chamados.Count,
                    abertos = chamados.Count(c => c.Status == "Aberto"),
                    emAtendimento = chamados.Count(c => c.Status == "Em Atendimento"),
                    aguardandoCliente = chamados.Count(c => c.Status == "Aguardando Cliente"),
                    concluidos = chamados.Count(c => c.Status == "Concluído" || c.Status == "Solucionado por IA"),
                    porPrioridade = chamados
                        .GroupBy(c => c.Prioridade)
                        .Select(g => new { prioridade = g.Key, total = g.Count() })
                        .ToList(),
                    porCategoria = chamados
                        .GroupBy(c => c.CategoriaIA)
                        .Select(g => new { categoria = g.Key, total = g.Count() })
                        .OrderByDescending(x => x.total)
                        .ToList(),
                    chamados = chamados
                };

                return Ok(ApiResponse<object>.SuccessResult(estatisticas));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de chamados por período");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Erro ao gerar relatório"));
            }
        }

        /// <summary>
        /// Relatório de performance por suporte
        /// </summary>
        /// <returns>Estatísticas de cada suporte</returns>
        [HttpGet("suportes")]
        public async Task<ActionResult<ApiResponse<object>>> GetRelatorioSuportes()
        {
            try
            {
                var suportes = await _context.Usuarios
                    .Where(u => u.Tipo == "Suporte")
                    .Select(u => new
                    {
                        u.Id,
                        u.Nome,
                        u.Email,
                        u.DataCadastro,
                        chamadosAtivos = _context.Chamados
                            .Count(c => c.SuporteResponsavelId == u.Id && c.Status != "Concluído" && c.Status != "Solucionado por IA"),
                        chamadosFinalizados = _context.Chamados
                            .Count(c => c.SuporteResponsavelId == u.Id && (c.Status == "Concluído" || c.Status == "Solucionado por IA")),
                        totalChamados = _context.Chamados
                            .Count(c => c.SuporteResponsavelId == u.Id),
                        chamadosPorPrioridade = _context.Chamados
                            .Where(c => c.SuporteResponsavelId == u.Id)
                            .GroupBy(c => c.Prioridade)
                            .Select(g => new { prioridade = g.Key, total = g.Count() })
                            .ToList()
                    })
                    .ToListAsync();

                var resumo = new
                {
                    totalSuportes = suportes.Count,
                    suportes = suportes
                };

                return Ok(ApiResponse<object>.SuccessResult(resumo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de suportes");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Erro ao gerar relatório"));
            }
        }

        /// <summary>
        /// Relatório de categorias mais comuns
        /// </summary>
        /// <returns>Estatísticas por categoria</returns>
        [HttpGet("categorias")]
        public async Task<ActionResult<ApiResponse<object>>> GetRelatorioCategorias()
        {
            try
            {
                var categorias = await _context.Chamados
                    .GroupBy(c => c.CategoriaIA)
                    .Select(g => new
                    {
                        categoria = g.Key,
                        total = g.Count(),
                        abertos = g.Count(c => c.Status == "Aberto"),
                        emAtendimento = g.Count(c => c.Status == "Em Atendimento"),
                        concluidos = g.Count(c => c.Status == "Concluído" || c.Status == "Solucionado por IA")
                    })
                    .OrderByDescending(x => x.total)
                    .ToListAsync();

                return Ok(ApiResponse<object>.SuccessResult(categorias));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de categorias");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Erro ao gerar relatório"));
            }
        }

        /// <summary>
        /// Gera PDF do relatório de usuários
        /// </summary>
        /// <returns>Arquivo PDF</returns>
        [HttpGet("usuarios/pdf")]
        public async Task<IActionResult> GetRelatorioUsuariosPDF()
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .Select(u => new
                    {
                        u.Nome,
                        u.Email,
                        u.Tipo,
                        DataCadastro = u.DataCadastro.ToString("dd/MM/yyyy"),
                        TotalChamados = _context.Chamados.Count(c => c.UsuarioId == u.Id)
                    })
                    .OrderBy(u => u.Tipo)
                    .ThenBy(u => u.Nome)
                    .ToListAsync();

                using var memoryStream = new MemoryStream();
                using var writer = new PdfWriter(memoryStream);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf);

                // Título
                document.Add(new Paragraph("RELATÓRIO DE USUÁRIOS")
                    .SetFontSize(18)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // Resumo
                document.Add(new Paragraph("RESUMO")
                    .SetFontSize(14)
                    .SetBold());

                document.Add(new Paragraph($"Total de usuários: {usuarios.Count}"));
                document.Add(new Paragraph($"Clientes: {usuarios.Count(u => u.Tipo == "Cliente")}"));
                document.Add(new Paragraph($"Suportes: {usuarios.Count(u => u.Tipo == "Suporte")}"));
                document.Add(new Paragraph($"Administradores: {usuarios.Count(u => u.Tipo == "Administrador")}"));
                document.Add(new Paragraph("\n"));

                // Tabela
                var table = new Table(5).UseAllAvailableWidth();
                table.AddHeaderCell("Nome");
                table.AddHeaderCell("Email");
                table.AddHeaderCell("Tipo");
                table.AddHeaderCell("Data Cadastro");
                table.AddHeaderCell("Chamados");

                foreach (var usuario in usuarios)
                {
                    table.AddCell(usuario.Nome);
                    table.AddCell(usuario.Email);
                    table.AddCell(usuario.Tipo);
                    table.AddCell(usuario.DataCadastro);
                    table.AddCell(usuario.TotalChamados.ToString());
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", 
                    $"relatorio-usuarios-{DateTime.Now:yyyyMMdd-HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar PDF de usuários");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Gera PDF do relatório de chamados por período
        /// </summary>
        /// <param name="dataInicio">Data inicial</param>
        /// <param name="dataFim">Data final</param>
        /// <returns>Arquivo PDF</returns>
        [HttpGet("chamados-periodo/pdf")]
        public async Task<IActionResult> GetRelatorioChamadosPDF(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                var inicio = dataInicio ?? DateTime.Now.AddDays(-30);
                var fim = dataFim ?? DateTime.Now;

                var chamados = await _context.Chamados
                    .Include(c => c.Usuario)
                    .Include(c => c.SuporteResponsavel)
                    .Where(c => c.DataAbertura >= inicio && c.DataAbertura <= fim)
                    .Select(c => new
                    {
                        c.Titulo,
                        c.Status,
                        c.Prioridade,
                        c.CategoriaIA,
                        DataAbertura = c.DataAbertura.ToString("dd/MM/yyyy"),
                        Cliente = c.Usuario.Nome,
                        Suporte = c.SuporteResponsavel != null ? c.SuporteResponsavel.Nome : "Não atribuído"
                    })
                    .ToListAsync();

                using var memoryStream = new MemoryStream();
                using var writer = new PdfWriter(memoryStream);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf);

                // Título
                document.Add(new Paragraph("RELATÓRIO DE CHAMADOS POR PERÍODO")
                    .SetFontSize(18)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph($"Período: {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}")
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // Resumo
                document.Add(new Paragraph("RESUMO")
                    .SetFontSize(14)
                    .SetBold());

                document.Add(new Paragraph($"Total de chamados: {chamados.Count}"));
                document.Add(new Paragraph($"Abertos: {chamados.Count(c => c.Status == "Aberto")}"));
                document.Add(new Paragraph($"Em Atendimento: {chamados.Count(c => c.Status == "Em Atendimento")}"));
                document.Add(new Paragraph($"Concluídos: {chamados.Count(c => c.Status == "Concluído" || c.Status == "Solucionado por IA")}"));
                document.Add(new Paragraph("\n"));

                // Tabela
                var table = new Table(6).UseAllAvailableWidth();
                table.AddHeaderCell("Título");
                table.AddHeaderCell("Status");
                table.AddHeaderCell("Prioridade");
                table.AddHeaderCell("Categoria IA");
                table.AddHeaderCell("Data");
                table.AddHeaderCell("Cliente");

                foreach (var chamado in chamados)
                {
                    table.AddCell(chamado.Titulo);
                    table.AddCell(chamado.Status);
                    table.AddCell(chamado.Prioridade);
                    table.AddCell(chamado.CategoriaIA ?? "N/A");
                    table.AddCell(chamado.DataAbertura);
                    table.AddCell(chamado.Cliente);
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", 
                    $"relatorio-chamados-{DateTime.Now:yyyyMMdd-HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar PDF de chamados");
                return StatusCode(500);
            }
        }
    }
}
