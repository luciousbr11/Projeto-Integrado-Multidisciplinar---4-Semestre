using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_Web.Data;
using GestaoChamadosAI_Web.Models.ViewModels;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;

namespace GestaoChamadosAI_Web.Controllers
{
    /// <summary>
    /// Controller para gerenciar relatórios administrativos do sistema.
    /// Acesso restrito apenas para usuários do tipo Administrador.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext _context;

        public RelatoriosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Página principal de relatórios com cards de acesso rápido.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Relatório de Usuários Cadastrados no sistema.
        /// Exibe lista completa de usuários com informações de tipo e total de chamados.
        /// </summary>
        public async Task<IActionResult> UsuariosCadastrados()
        {
        var usuarios = await _context.Usuarios
            .Select(u => new UsuarioRelatorio
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Tipo = u.Tipo,
                DataCadastro = u.DataCadastro,
                TotalChamados = u.Chamados != null ? u.Chamados.Count : 0
            })
            .OrderBy(u => u.Nome)
            .ToListAsync();            var model = new RelatoriosViewModel
            {
                Usuarios = usuarios,
                TotalUsuarios = usuarios.Count,
                TotalClientes = usuarios.Count(u => u.Tipo == "Cliente"),
                TotalSuportes = usuarios.Count(u => u.Tipo == "Suporte"),
                TotalAdministradores = usuarios.Count(u => u.Tipo == "Administrador")
            };

            return View(model);
        }

        /// <summary>
        /// Relatório de Chamados por Período.
        /// Permite filtrar chamados por data de início e fim.
        /// </summary>
        /// <param name="dataInicio">Data inicial do período (opcional)</param>
        /// <param name="dataFim">Data final do período (opcional)</param>
        public async Task<IActionResult> ChamadosPorPeriodo(DateTime? dataInicio, DateTime? dataFim)
        {
            // Define período padráo: últimos 30 dias
            if (!dataInicio.HasValue)
            {
                dataInicio = DateTime.Now.AddDays(-30);
            }
            if (!dataFim.HasValue)
            {
                dataFim = DateTime.Now;
            }

            // Ajusta a hora para pegar o dia completo
            var inicio = dataInicio.Value.Date;
            var fim = dataFim.Value.Date.AddDays(1).AddSeconds(-1);

            var chamados = await _context.Chamados
                .Include(c => c.Usuario)
                .Where(c => c.DataAbertura >= inicio && c.DataAbertura <= fim)
                .Select(c => new ChamadoRelatorio
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Status = c.Status,
                    Prioridade = c.Prioridade,
                    CategoriaIA = c.CategoriaIA,
                    ClienteNome = c.Usuario != null ? c.Usuario.Nome : "N/A",
                    SuporteNome = c.SuporteResponsavelId != null 
                        ? _context.Usuarios
                            .Where(u => u.Id == c.SuporteResponsavelId)
                            .Select(u => u.Nome)
                            .FirstOrDefault()
                        : null,
                    DataAbertura = c.DataAbertura,
                    DataResolucao = c.Status == "Concluído" ? c.DataAbertura : null
                })
                .OrderByDescending(c => c.DataAbertura)
                .ToListAsync();

            var model = new RelatoriosViewModel
            {
                ChamadosPorPeriodo = chamados,
                DataInicio = dataInicio,
                DataFim = dataFim,
                TotalChamadosPeriodo = chamados.Count,
                ChamadosAbertos = chamados.Count(c => c.Status == "Aberto"),
                ChamadosEmAndamento = chamados.Count(c => c.Status == "Em Andamento"),
                ChamadosConcluidos = chamados.Count(c => c.Status == "Concluído")
            };

            return View(model);
        }

        /// <summary>
        /// Relatório de Chamados por Suporte.
        /// Exibe estatísticas de atendimento de cada usuário do tipo Suporte.
        /// </summary>
        public async Task<IActionResult> ChamadosPorSuporte()
        {
            var suportes = await _context.Usuarios
                .Where(u => u.Tipo == "Suporte" || u.Tipo == "Administrador")
                .Select(u => new SuporteRelatorio
                {
                    UsuarioId = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    TotalChamados = _context.Chamados.Count(c => c.SuporteResponsavelId == u.Id),
                    ChamadosAbertos = _context.Chamados.Count(c => c.SuporteResponsavelId == u.Id && c.Status == "Aberto"),
                    ChamadosEmAndamento = _context.Chamados.Count(c => c.SuporteResponsavelId == u.Id && c.Status == "Em Andamento"),
                    ChamadosConcluidos = _context.Chamados.Count(c => c.SuporteResponsavelId == u.Id && c.Status == "Concluído"),
                    UltimoAtendimento = _context.Chamados
                        .Where(c => c.SuporteResponsavelId == u.Id)
                        .OrderByDescending(c => c.DataAbertura)
                        .Select(c => c.DataAbertura)
                        .FirstOrDefault(),
                    TempoMedioResolucao = _context.Chamados
                        .Where(c => c.SuporteResponsavelId == u.Id && c.Status == "Concluído")
                        .Select(c => (double?)EF.Functions.DateDiffHour(c.DataAbertura, DateTime.Now))
                        .Average()
                })
                .OrderByDescending(s => s.TotalChamados)
                .ToListAsync();

            var model = new RelatoriosViewModel
            {
                ChamadosPorSuporte = suportes,
                TotalChamadosAtendidos = suportes.Sum(s => s.TotalChamados)
            };

            return View(model);
        }

        /// <summary>
        /// Gera PDF do relatório de usuários cadastrados.
        /// </summary>
        public async Task<IActionResult> ImprimirUsuariosPDF()
    {
        var usuarios = await _context.Usuarios
            .Select(u => new UsuarioRelatorio
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Tipo = u.Tipo,
                DataCadastro = u.DataCadastro,
                TotalChamados = u.Chamados != null ? u.Chamados.Count : 0
            })
            .OrderBy(u => u.Nome)
            .ToListAsync();

        using var stream = new MemoryStream();
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Título
        document.Add(new Paragraph("Relatório de Usuários Cadastrados")
            .SetFontSize(18)
            .SetBold()
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(20));

        // Resumo
        document.Add(new Paragraph($"Total de Usuários: {usuarios.Count}")
            .SetFontSize(12)
            .SetBold());
        document.Add(new Paragraph($"Clientes: {usuarios.Count(u => u.Tipo == "Cliente")}")
            .SetFontSize(10));
        document.Add(new Paragraph($"Suportes: {usuarios.Count(u => u.Tipo == "Suporte")}")
            .SetFontSize(10));
        document.Add(new Paragraph($"Administradores: {usuarios.Count(u => u.Tipo == "Administrador")}")
            .SetFontSize(10)
            .SetMarginBottom(20));

        // Tabela
        var table = new Table(new float[] { 1, 3, 4, 2, 2, 1 });
        table.SetWidth(UnitValue.CreatePercentValue(100));

        // Cabeçalho
        table.AddHeaderCell(new Cell().Add(new Paragraph("ID").SetBold().SetFontSize(10)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Nome").SetBold().SetFontSize(10)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Email").SetBold().SetFontSize(10)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Tipo").SetBold().SetFontSize(10)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Data Cadastro").SetBold().SetFontSize(10)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Chamados").SetBold().SetFontSize(10)));

        // Dados
        foreach (var usuario in usuarios)
        {
            table.AddCell(new Cell().Add(new Paragraph(usuario.Id.ToString()).SetFontSize(9)));
            table.AddCell(new Cell().Add(new Paragraph(usuario.Nome).SetFontSize(9)));
            table.AddCell(new Cell().Add(new Paragraph(usuario.Email).SetFontSize(9)));
            table.AddCell(new Cell().Add(new Paragraph(usuario.Tipo).SetFontSize(9)));
            table.AddCell(new Cell().Add(new Paragraph(usuario.DataCadastro.ToString("dd/MM/yyyy")).SetFontSize(9)));
            table.AddCell(new Cell().Add(new Paragraph(usuario.TotalChamados.ToString()).SetFontSize(9)));
        }

        document.Add(table);
        document.Close();

        return File(stream.ToArray(), "application/pdf", $"usuarios_{DateTime.Now:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Gera PDF do relatório de chamados por período.
    /// </summary>
    public async Task<IActionResult> ImprimirChamadosPeriodoPDF(DateTime? dataInicio, DateTime? dataFim)
    {
        if (!dataInicio.HasValue) dataInicio = DateTime.Now.AddDays(-30);
        if (!dataFim.HasValue) dataFim = DateTime.Now;

        var inicio = dataInicio.Value.Date;
        var fim = dataFim.Value.Date.AddDays(1).AddSeconds(-1);

        var chamados = await _context.Chamados
            .Include(c => c.Usuario)
            .Where(c => c.DataAbertura >= inicio && c.DataAbertura <= fim)
            .Select(c => new ChamadoRelatorio
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Status = c.Status,
                Prioridade = c.Prioridade,
                CategoriaIA = c.CategoriaIA,
                ClienteNome = c.Usuario != null ? c.Usuario.Nome : "N/A",
                SuporteNome = c.SuporteResponsavelId != null 
                    ? _context.Usuarios.Where(u => u.Id == c.SuporteResponsavelId).Select(u => u.Nome).FirstOrDefault()
                    : null,
                DataAbertura = c.DataAbertura
            })
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();

        using var stream = new MemoryStream();
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Título
        document.Add(new Paragraph("Relatório de Chamados por Período")
            .SetFontSize(18)
            .SetBold()
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph($"Período: {dataInicio:dd/MM/yyyy} - {dataFim:dd/MM/yyyy}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(20));

        // Resumo
        document.Add(new Paragraph($"Total de Chamados: {chamados.Count}")
            .SetFontSize(12)
            .SetBold());
        document.Add(new Paragraph($"Abertos: {chamados.Count(c => c.Status == "Aberto")}")
            .SetFontSize(10));
        document.Add(new Paragraph($"Em Andamento: {chamados.Count(c => c.Status == "Em Andamento")}")
            .SetFontSize(10));
        document.Add(new Paragraph($"Concluídos: {chamados.Count(c => c.Status == "Concluído")}")
            .SetFontSize(10)
            .SetMarginBottom(20));

        // Tabela
        var table = new Table(new float[] { 1, 4, 2, 2, 2, 2 });
        table.SetWidth(UnitValue.CreatePercentValue(100));

        // Cabeçalho
        table.AddHeaderCell(new Cell().Add(new Paragraph("ID").SetBold().SetFontSize(9)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Título").SetBold().SetFontSize(9)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Status").SetBold().SetFontSize(9)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Prioridade").SetBold().SetFontSize(9)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Categoria").SetBold().SetFontSize(9)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Data Abertura").SetBold().SetFontSize(9)));

        // Dados
        foreach (var chamado in chamados)
        {
            table.AddCell(new Cell().Add(new Paragraph(chamado.Id.ToString()).SetFontSize(8)));
            table.AddCell(new Cell().Add(new Paragraph(chamado.Titulo ?? "").SetFontSize(8)));
            table.AddCell(new Cell().Add(new Paragraph(chamado.Status ?? "").SetFontSize(8)));
            table.AddCell(new Cell().Add(new Paragraph(chamado.Prioridade ?? "N/A").SetFontSize(8)));
            table.AddCell(new Cell().Add(new Paragraph(chamado.CategoriaIA ?? "N/A").SetFontSize(8)));
            table.AddCell(new Cell().Add(new Paragraph(chamado.DataAbertura.ToString("dd/MM/yyyy HH:mm")).SetFontSize(8)));
        }

        document.Add(table);
        document.Close();

        return File(stream.ToArray(), "application/pdf", $"chamados_{dataInicio:yyyyMMdd}_{dataFim:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Gera PDF do relatório de chamados por suporte.
    /// </summary>
    public async Task<IActionResult> ImprimirChamadosSuportePDF()
    {
        var suportes = await _context.Usuarios
            .Where(u => u.Tipo == "Suporte" || u.Tipo == "Administrador")
            .Select(u => new SuporteRelatorio
            {
                UsuarioId = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                TotalChamados = _context.Chamados.Count(c => c.SuporteResponsavelId == u.Id),
                ChamadosAbertos = _context.Chamados.Count(c => c.SuporteResponsavelId == u.Id && c.Status == "Aberto"),
                ChamadosEmAndamento = _context.Chamados.Count(c => c.SuporteResponsavelId == u.Id && c.Status == "Em Andamento"),
                ChamadosConcluidos = _context.Chamados.Count(c => c.SuporteResponsavelId == u.Id && c.Status == "Concluído"),
                UltimoAtendimento = _context.Chamados
                    .Where(c => c.SuporteResponsavelId == u.Id)
                    .OrderByDescending(c => c.DataAbertura)
                    .Select(c => c.DataAbertura)
                    .FirstOrDefault()
            })
            .OrderByDescending(s => s.TotalChamados)
            .ToListAsync();

        using var stream = new MemoryStream();
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Título
        document.Add(new Paragraph("Relatório de Chamados por Suporte")
            .SetFontSize(18)
            .SetBold()
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(20));

        // Resumo
        document.Add(new Paragraph($"Total de Chamados Atendidos: {suportes.Sum(s => s.TotalChamados)}")
            .SetFontSize(12)
            .SetBold()
            .SetMarginBottom(20));

        // Tabela
        var table = new Table(new float[] { 3, 2, 2, 2, 2, 3 });
        table.SetWidth(UnitValue.CreatePercentValue(100));

        // Cabeçalho
        table.AddHeaderCell(new Cell().Add(new Paragraph("Suporte").SetBold().SetFontSize(9)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Total").SetBold().SetFontSize(9)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Abertos").SetBold().SetFontSize(9)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Em Andamento").SetBold().SetFontSize(9)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Concluídos").SetBold().SetFontSize(9)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Último Atendimento").SetBold().SetFontSize(9)));

        // Dados
        foreach (var suporte in suportes)
        {
            table.AddCell(new Cell().Add(new Paragraph(suporte.Nome).SetFontSize(8)));
            table.AddCell(new Cell().Add(new Paragraph(suporte.TotalChamados.ToString()).SetFontSize(8)));
            table.AddCell(new Cell().Add(new Paragraph(suporte.ChamadosAbertos.ToString()).SetFontSize(8)));
            table.AddCell(new Cell().Add(new Paragraph(suporte.ChamadosEmAndamento.ToString()).SetFontSize(8)));
            table.AddCell(new Cell().Add(new Paragraph(suporte.ChamadosConcluidos.ToString()).SetFontSize(8)));
            table.AddCell(new Cell().Add(new Paragraph(
                suporte.UltimoAtendimento.HasValue 
                    ? suporte.UltimoAtendimento.Value.ToString("dd/MM/yyyy HH:mm") 
                    : "N/A"
            ).SetFontSize(8)));
        }

        document.Add(table);
        document.Close();

        return File(stream.ToArray(), "application/pdf", $"suportes_{DateTime.Now:yyyyMMdd}.pdf");
    }
}
}
