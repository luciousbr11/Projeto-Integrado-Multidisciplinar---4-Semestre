#if ANDROID
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using GestaoChamadosAI_MAUI.Models;
#else
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using GestaoChamadosAI_MAUI.Models;
using PdfColors = QuestPDF.Helpers.Colors;
#endif

namespace GestaoChamadosAI_MAUI.Services
{
    public interface IPdfService
    {
        Task<string> GerarRelatorioUsuariosPdfAsync(RelatorioUsuarios relatorio);
        Task<string> GerarRelatorioChamadosPdfAsync(RelatorioChamadosPeriodo relatorio);
        Task<string> GerarRelatorioSuportesPdfAsync(RelatorioSuportes relatorio);
        Task<string> GerarRelatorioCategoriasAsync(RelatorioCategorias relatorio);
    }

#if ANDROID
    // Implementação iText7 para Android
    public class PdfService : IPdfService
    {
        public async Task<string> GerarRelatorioUsuariosPdfAsync(RelatorioUsuarios relatorio)
        {
            return await Task.Run(() =>
            {
                string? filePath = null;
                MemoryStream? memStream = null;
                FileStream? fs = null;
                PdfWriter? writer = null;
                PdfDocument? pdf = null;
                Document? document = null;
                
                try
                {
                    if (relatorio == null)
                        throw new ArgumentNullException(nameof(relatorio), "Relatório não pode ser nulo");

                    var fileName = $"Relatorio_Usuarios_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                    // Criar PDF em memória primeiro
                    memStream = new MemoryStream();
                    // IMPORTANTE: leaveOpen = true para não fechar o stream ao fechar o writer
                    var writerProperties = new iText.Kernel.Pdf.WriterProperties();
                    writer = new PdfWriter(memStream, writerProperties);
                    writer.SetCloseStream(false); // Não fechar o stream
                    pdf = new PdfDocument(writer);
                    document = new Document(pdf);

                    // Cabeçalho
                    document.Add(new Paragraph("Gestão de Chamados AI")
                        .SetFontSize(20)
                        .SetBold()
                        .SetFontColor(ColorConstants.CYAN));

                    document.Add(new Paragraph("Relatório de Usuários Cadastrados")
                        .SetFontSize(14)
                        .SetFontColor(ColorConstants.DARK_GRAY));

                    document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .SetFontSize(9)
                        .SetFontColor(ColorConstants.GRAY)
                        .SetMarginBottom(20));

                    // Resumo
                    var summaryTable = new Table(4).UseAllAvailableWidth();
                    summaryTable.AddCell(CreateHeaderCell("Total de Usuários"));
                    summaryTable.AddCell(CreateHeaderCell("Clientes"));
                    summaryTable.AddCell(CreateHeaderCell("Suportes"));
                    summaryTable.AddCell(CreateHeaderCell("Administradores"));
                    
                    summaryTable.AddCell(CreateValueCell(relatorio.TotalUsuarios.ToString()));
                    summaryTable.AddCell(CreateValueCell(relatorio.Clientes.ToString()));
                    summaryTable.AddCell(CreateValueCell(relatorio.Suportes.ToString()));
                    summaryTable.AddCell(CreateValueCell(relatorio.Administradores.ToString()));
                    
                    document.Add(summaryTable.SetMarginBottom(20));

                    // Tabela de usuários
                    if (relatorio.Usuarios != null && relatorio.Usuarios.Any())
                    {
                        document.Add(new Paragraph("Lista Detalhada de Usuários")
                            .SetFontSize(14)
                            .SetBold()
                            .SetMarginTop(10)
                            .SetMarginBottom(10));

                        var table = new Table(new float[] { 2, 2, 1, 1, 1, 1 }).UseAllAvailableWidth();
                        
                        // Cabeçalho
                        table.AddHeaderCell(CreateTableHeader("Nome"));
                        table.AddHeaderCell(CreateTableHeader("E-mail"));
                        table.AddHeaderCell(CreateTableHeader("Tipo"));
                        table.AddHeaderCell(CreateTableHeader("Total"));
                        table.AddHeaderCell(CreateTableHeader("Abertos"));
                        table.AddHeaderCell(CreateTableHeader("Atendidos"));

                        // Dados
                        foreach (var usuario in relatorio.Usuarios)
                        {
                            table.AddCell(CreateTableCell(usuario?.Nome ?? "N/A"));
                            table.AddCell(CreateTableCell(usuario?.Email ?? "N/A"));
                            table.AddCell(CreateTableCell(usuario?.Tipo ?? "N/A"));
                            table.AddCell(CreateTableCell((usuario?.TotalChamados ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                            table.AddCell(CreateTableCell((usuario?.ChamadosAbertos ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                            table.AddCell(CreateTableCell((usuario?.ChamadosAtendidos ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                        }

                        document.Add(table);
                    }

                    // IMPORTANTE: Fechar documento ANTES de gravar no arquivo
                    document.Close();
                    pdf.Close();
                    writer.Close();

                    // Agora gravar o MemoryStream no arquivo
                    memStream.Position = 0; // Voltar para o início
                    fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    memStream.CopyTo(fs);
                    fs.Flush();
                    fs.Close();

                    return filePath;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao gerar PDF de Usuários: {ex.Message}\nStack: {ex.StackTrace}", ex);
                }
                finally
                {
                    try
                    {
                        fs?.Dispose();
                        memStream?.Dispose();
                    }
                    catch { /* Ignora erros ao fechar */ }
                }
            });
        }

        public async Task<string> GerarRelatorioChamadosPdfAsync(RelatorioChamadosPeriodo relatorio)
        {
            return await Task.Run(() =>
            {
                string? filePath = null;
                MemoryStream? memStream = null;
                FileStream? fs = null;
                PdfWriter? writer = null;
                PdfDocument? pdf = null;
                Document? document = null;
                
                try
                {
                    if (relatorio == null)
                        throw new ArgumentNullException(nameof(relatorio), "Relatório não pode ser nulo");

                    var fileName = $"Relatorio_Chamados_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                    memStream = new MemoryStream();
                    var writerProperties = new iText.Kernel.Pdf.WriterProperties();
                    writer = new PdfWriter(memStream, writerProperties);
                    writer.SetCloseStream(false);
                    pdf = new PdfDocument(writer);
                    document = new Document(pdf);

                    // Cabeçalho
                    document.Add(new Paragraph("Gestão de Chamados AI")
                        .SetFontSize(20)
                        .SetBold()
                        .SetFontColor(ColorConstants.CYAN));

                    document.Add(new Paragraph("Relatório de Chamados por Período")
                        .SetFontSize(14)
                        .SetFontColor(ColorConstants.DARK_GRAY));

                    document.Add(new Paragraph($"Período: {relatorio.Inicio:dd/MM/yyyy} até {relatorio.Fim:dd/MM/yyyy}")
                        .SetFontSize(11));

                    document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .SetFontSize(9)
                        .SetFontColor(ColorConstants.GRAY)
                        .SetMarginBottom(20));

                    // Resumo por Status
                    document.Add(new Paragraph("Resumo por Status")
                        .SetFontSize(14)
                        .SetBold()
                        .SetMarginBottom(10));

                    var statusTable = new Table(5).UseAllAvailableWidth();
                    statusTable.AddCell(CreateHeaderCell("Total"));
                    statusTable.AddCell(CreateHeaderCell("Abertos"));
                    statusTable.AddCell(CreateHeaderCell("Em Atendimento"));
                    statusTable.AddCell(CreateHeaderCell("Aguardando"));
                    statusTable.AddCell(CreateHeaderCell("Concluídos"));
                    
                    statusTable.AddCell(CreateValueCell(relatorio.Total.ToString()));
                    statusTable.AddCell(CreateValueCell(relatorio.Abertos.ToString()));
                    statusTable.AddCell(CreateValueCell(relatorio.EmAtendimento.ToString()));
                    statusTable.AddCell(CreateValueCell(relatorio.AguardandoCliente.ToString()));
                    statusTable.AddCell(CreateValueCell(relatorio.Fechados.ToString()));
                    
                    document.Add(statusTable.SetMarginBottom(20));

                    // Por Prioridade
                    if (relatorio.PorPrioridade != null && relatorio.PorPrioridade.Any())
                    {
                        document.Add(new Paragraph("Chamados por Prioridade")
                            .SetFontSize(14)
                            .SetBold()
                            .SetMarginBottom(10));

                        var prioridadeTable = new Table(2).UseAllAvailableWidth();
                        prioridadeTable.AddHeaderCell(CreateTableHeader("Prioridade"));
                        prioridadeTable.AddHeaderCell(CreateTableHeader("Total"));

                        foreach (var item in relatorio.PorPrioridade)
                        {
                            prioridadeTable.AddCell(CreateTableCell(item?.Prioridade ?? "N/A"));
                            prioridadeTable.AddCell(CreateTableCell((item?.Total ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                        }

                        document.Add(prioridadeTable.SetMarginBottom(20));
                    }

                    // Por Categoria
                    if (relatorio.PorCategoria != null && relatorio.PorCategoria.Any())
                    {
                        document.Add(new Paragraph("Chamados por Categoria")
                            .SetFontSize(14)
                            .SetBold()
                            .SetMarginBottom(10));

                        var categoriaTable = new Table(2).UseAllAvailableWidth();
                        categoriaTable.AddHeaderCell(CreateTableHeader("Categoria"));
                        categoriaTable.AddHeaderCell(CreateTableHeader("Total"));

                        foreach (var item in relatorio.PorCategoria)
                        {
                            categoriaTable.AddCell(CreateTableCell(item?.Categoria ?? "N/A"));
                            categoriaTable.AddCell(CreateTableCell((item?.Total ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                        }

                        document.Add(categoriaTable);
                    }

                    document.Close();
                    pdf.Close();
                    writer.Close();

                    memStream.Position = 0;
                    fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    memStream.CopyTo(fs);
                    fs.Flush();
                    fs.Close();

                    return filePath;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao gerar PDF de Chamados: {ex.Message}\nStack: {ex.StackTrace}", ex);
                }
                finally
                {
                    try
                    {
                        fs?.Dispose();
                        memStream?.Dispose();
                    }
                    catch { /* Ignora erros ao fechar */ }
                }
            });
        }

        public async Task<string> GerarRelatorioSuportesPdfAsync(RelatorioSuportes relatorio)
        {
            return await Task.Run(() =>
            {
                string? filePath = null;
                MemoryStream? memStream = null;
                FileStream? fs = null;
                PdfWriter? writer = null;
                PdfDocument? pdf = null;
                Document? document = null;
                
                try
                {
                    if (relatorio == null)
                        throw new ArgumentNullException(nameof(relatorio), "Relatório não pode ser nulo");

                    var fileName = $"Relatorio_Suportes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                    memStream = new MemoryStream();
                    var writerProperties = new iText.Kernel.Pdf.WriterProperties();
                    writer = new PdfWriter(memStream, writerProperties);
                    writer.SetCloseStream(false);
                    pdf = new PdfDocument(writer);
                    document = new Document(pdf);

                    // Cabeçalho
                    document.Add(new Paragraph("Gestão de Chamados AI")
                        .SetFontSize(20)
                        .SetBold()
                        .SetFontColor(ColorConstants.CYAN));

                    document.Add(new Paragraph("Relatório de Desempenho dos Suportes")
                        .SetFontSize(14)
                        .SetFontColor(ColorConstants.DARK_GRAY));

                    document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .SetFontSize(9)
                        .SetFontColor(ColorConstants.GRAY)
                        .SetMarginBottom(20));

                    document.Add(new Paragraph($"Total de Suportes: {relatorio.TotalSuportes}")
                        .SetFontSize(14)
                        .SetBold()
                        .SetMarginBottom(10));

                    if (relatorio.Suportes != null && relatorio.Suportes.Any())
                    {
                        var table = new Table(new float[] { 2, 2, 1, 1, 1 }).UseAllAvailableWidth();
                        
                        table.AddHeaderCell(CreateTableHeader("Nome"));
                        table.AddHeaderCell(CreateTableHeader("E-mail"));
                        table.AddHeaderCell(CreateTableHeader("Ativos"));
                        table.AddHeaderCell(CreateTableHeader("Finalizados"));
                        table.AddHeaderCell(CreateTableHeader("Total"));

                        foreach (var suporte in relatorio.Suportes)
                        {
                            table.AddCell(CreateTableCell(suporte?.Nome ?? "N/A"));
                            table.AddCell(CreateTableCell(suporte?.Email ?? "N/A"));
                            table.AddCell(CreateTableCell((suporte?.ChamadosAtivos ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                            table.AddCell(CreateTableCell((suporte?.ChamadosFinalizados ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                            table.AddCell(CreateTableCell((suporte?.TotalChamados ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                        }

                        document.Add(table);
                    }

                    document.Close();
                    pdf.Close();
                    writer.Close();

                    memStream.Position = 0;
                    fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    memStream.CopyTo(fs);
                    fs.Flush();
                    fs.Close();

                    return filePath;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao gerar PDF de Suportes: {ex.Message}\nStack: {ex.StackTrace}", ex);
                }
                finally
                {
                    try
                    {
                        fs?.Dispose();
                        memStream?.Dispose();
                    }
                    catch { /* Ignora erros ao fechar */ }
                }
            });
        }

        public async Task<string> GerarRelatorioCategoriasAsync(RelatorioCategorias relatorio)
        {
            return await Task.Run(() =>
            {
                string? filePath = null;
                MemoryStream? memStream = null;
                FileStream? fs = null;
                PdfWriter? writer = null;
                PdfDocument? pdf = null;
                Document? document = null;
                
                try
                {
                    if (relatorio == null)
                        throw new ArgumentNullException(nameof(relatorio), "Relatório não pode ser nulo");

                    var fileName = $"Relatorio_Categorias_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                    memStream = new MemoryStream();
                    var writerProperties = new iText.Kernel.Pdf.WriterProperties();
                    writer = new PdfWriter(memStream, writerProperties);
                    writer.SetCloseStream(false);
                    pdf = new PdfDocument(writer);
                    document = new Document(pdf);

                    // Cabeçalho
                    document.Add(new Paragraph("Gestão de Chamados AI")
                        .SetFontSize(20)
                        .SetBold()
                        .SetFontColor(ColorConstants.CYAN));

                    document.Add(new Paragraph("Relatório de Chamados por Categoria")
                        .SetFontSize(14)
                        .SetFontColor(ColorConstants.DARK_GRAY));

                    document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .SetFontSize(9)
                        .SetFontColor(ColorConstants.GRAY)
                        .SetMarginBottom(20));

                    if (relatorio.Categorias != null && relatorio.Categorias.Any())
                    {
                        var table = new Table(new float[] { 2, 1, 1, 1, 1, 1 }).UseAllAvailableWidth();
                        
                        table.AddHeaderCell(CreateTableHeader("Categoria"));
                        table.AddHeaderCell(CreateTableHeader("Total"));
                        table.AddHeaderCell(CreateTableHeader("Abertos"));
                        table.AddHeaderCell(CreateTableHeader("Atendimento"));
                        table.AddHeaderCell(CreateTableHeader("Aguardando"));
                        table.AddHeaderCell(CreateTableHeader("Concluídos"));

                        foreach (var categoria in relatorio.Categorias)
                        {
                            table.AddCell(CreateTableCell(categoria?.Categoria ?? "N/A"));
                            table.AddCell(CreateTableCell((categoria?.Total ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                            table.AddCell(CreateTableCell((categoria?.Abertos ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                            table.AddCell(CreateTableCell((categoria?.EmAtendimento ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                            table.AddCell(CreateTableCell((categoria?.AguardandoCliente ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                            table.AddCell(CreateTableCell((categoria?.Fechados ?? 0).ToString(), iText.Layout.Properties.TextAlignment.CENTER));
                        }

                        document.Add(table);
                    }

                    document.Close();
                    pdf.Close();
                    writer.Close();

                    memStream.Position = 0;
                    fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    memStream.CopyTo(fs);
                    fs.Flush();
                    fs.Close();

                    return filePath;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao gerar PDF de Categorias: {ex.Message}\nStack: {ex.StackTrace}", ex);
                }
                finally
                {
                    try
                    {
                        fs?.Dispose();
                        memStream?.Dispose();
                    }
                    catch { /* Ignora erros ao fechar */ }
                }
            });
        }

        private static iText.Layout.Element.Cell CreateHeaderCell(string text)
        {
            return new iText.Layout.Element.Cell()
                .Add(new Paragraph(text)
                    .SetBold()
                    .SetFontColor(ColorConstants.WHITE)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER))
                .SetBackgroundColor(ColorConstants.CYAN)
                .SetPadding(8);
        }

        private static iText.Layout.Element.Cell CreateValueCell(string text)
        {
            return new iText.Layout.Element.Cell()
                .Add(new Paragraph(text)
                    .SetFontSize(16)
                    .SetBold()
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER))
                .SetBackgroundColor(new DeviceRgb(230, 247, 255))
                .SetPadding(10);
        }

        private static iText.Layout.Element.Cell CreateTableHeader(string text)
        {
            return new iText.Layout.Element.Cell()
                .Add(new Paragraph(text)
                    .SetBold()
                    .SetFontColor(ColorConstants.WHITE)
                    .SetFontSize(10))
                .SetBackgroundColor(ColorConstants.CYAN)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetPadding(5);
        }

        private static iText.Layout.Element.Cell CreateTableCell(string text, iText.Layout.Properties.TextAlignment? alignment = null)
        {
            var paragraph = new Paragraph(text).SetFontSize(9);
            if (alignment.HasValue)
                paragraph.SetTextAlignment(alignment.Value);
                
            return new iText.Layout.Element.Cell()
                .Add(paragraph)
                .SetPadding(5)
                .SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.LIGHT_GRAY, 1));
        }
    }
#else
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            // Configurar licença QuestPDF (Community License para uso não comercial)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<string> GerarRelatorioUsuariosPdfAsync(RelatorioUsuarios relatorio)
        {
            return await Task.Run(() =>
            {
                var fileName = $"Relatorio_Usuarios_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(PdfColors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // Cabeçalho
                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Gestão de Chamados AI")
                                    .FontSize(20)
                                    .SemiBold()
                                    .FontColor(PdfColors.Teal.Medium);

                                column.Item().Text("Relatório de Usuários Cadastrados")
                                    .FontSize(14)
                                    .FontColor(PdfColors.Grey.Darken2);

                                column.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .FontSize(9)
                                    .FontColor(PdfColors.Grey.Medium);
                            });
                        });

                        // Conteúdo
                        page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                        {
                            column.Spacing(20);

                            // Resumo
                            column.Item().Row(row =>
                            {
                                row.Spacing(10);

                                AddCard(row.RelativeItem(), "Total de Usuários", relatorio.TotalUsuarios.ToString(), PdfColors.Teal.Medium);
                                AddCard(row.RelativeItem(), "Clientes", relatorio.Clientes.ToString(), PdfColors.Blue.Medium);
                                AddCard(row.RelativeItem(), "Suportes", relatorio.Suportes.ToString(), PdfColors.Orange.Medium);
                                AddCard(row.RelativeItem(), "Administradores", relatorio.Administradores.ToString(), PdfColors.Red.Medium);
                            });

                            // Tabela de usuários
                            column.Item().Text("Lista Detalhada de Usuários")
                                .FontSize(14)
                                .SemiBold()
                                .FontColor(PdfColors.Grey.Darken2);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2); // Nome
                                    columns.RelativeColumn(2); // Email
                                    columns.RelativeColumn(1); // Tipo
                                    columns.RelativeColumn(1); // Total Chamados
                                    columns.RelativeColumn(1); // Abertos
                                    columns.RelativeColumn(1); // Atendidos
                                });

                                // Cabeçalho da tabela
                                table.Header(header =>
                                {
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Nome").FontColor(PdfColors.White).SemiBold();

                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("E-mail").FontColor(PdfColors.White).SemiBold();

                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Tipo").FontColor(PdfColors.White).SemiBold();

                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Total").FontColor(PdfColors.White).SemiBold();

                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Abertos").FontColor(PdfColors.White).SemiBold();

                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Atendidos").FontColor(PdfColors.White).SemiBold();
                                });

                                // Linhas da tabela
                                foreach (var usuario in relatorio.Usuarios)
                                {
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(usuario.Nome);

                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(usuario.Email);

                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(usuario.Tipo);

                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(usuario.TotalChamados.ToString()).AlignCenter();

                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(usuario.ChamadosAbertos.ToString()).AlignCenter();

                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(usuario.ChamadosAtendidos.ToString()).AlignCenter();
                                }
                            });
                        });

                        // Rodapé
                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                    });
                }).GeneratePdf(filePath);

                return filePath;
            });
        }

        public async Task<string> GerarRelatorioChamadosPdfAsync(RelatorioChamadosPeriodo relatorio)
        {
            return await Task.Run(() =>
            {
                var fileName = $"Relatorio_Chamados_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(PdfColors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // Cabeçalho
                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Gestão de Chamados AI")
                                    .FontSize(20)
                                    .SemiBold()
                                    .FontColor(PdfColors.Teal.Medium);

                                column.Item().Text("Relatório de Chamados por Período")
                                    .FontSize(14)
                                    .FontColor(PdfColors.Grey.Darken2);

                                column.Item().Text($"Período: {relatorio.Inicio:dd/MM/yyyy} até {relatorio.Fim:dd/MM/yyyy}")
                                    .FontSize(11)
                                    .FontColor(PdfColors.Grey.Darken1);

                                column.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .FontSize(9)
                                    .FontColor(PdfColors.Grey.Medium);
                            });
                        });

                        // Conteúdo
                        page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                        {
                            column.Spacing(20);

                            // Resumo por Status
                            column.Item().Text("Resumo por Status")
                                .FontSize(14)
                                .SemiBold()
                                .FontColor(PdfColors.Grey.Darken2);

                            column.Item().Row(row =>
                            {
                                row.Spacing(10);
                                AddCard(row.RelativeItem(), "Total", relatorio.Total.ToString(), PdfColors.Teal.Medium);
                                AddCard(row.RelativeItem(), "Abertos", relatorio.Abertos.ToString(), PdfColors.Blue.Medium);
                                AddCard(row.RelativeItem(), "Em Atendimento", relatorio.EmAtendimento.ToString(), PdfColors.Orange.Medium);
                                AddCard(row.RelativeItem(), "Aguardando", relatorio.AguardandoCliente.ToString(), PdfColors.Purple.Medium);
                                AddCard(row.RelativeItem(), "Concluídos", relatorio.Fechados.ToString(), PdfColors.Green.Medium);
                            });

                            // Por Prioridade
                            if (relatorio.PorPrioridade.Any())
                            {
                                column.Item().Text("Chamados por Prioridade")
                                    .FontSize(14)
                                    .SemiBold()
                                    .FontColor(PdfColors.Grey.Darken2);

                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(PdfColors.Teal.Medium)
                                            .Padding(5).Text("Prioridade").FontColor(PdfColors.White).SemiBold();
                                        header.Cell().Background(PdfColors.Teal.Medium)
                                            .Padding(5).Text("Total").FontColor(PdfColors.White).SemiBold();
                                    });

                                    foreach (var item in relatorio.PorPrioridade)
                                    {
                                        table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                            .Padding(5).Text(item.Prioridade);
                                        table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                            .Padding(5).Text(item.Total.ToString()).AlignCenter();
                                    }
                                });
                            }

                            // Por Categoria
                            if (relatorio.PorCategoria.Any())
                            {
                                column.Item().Text("Chamados por Categoria")
                                    .FontSize(14)
                                    .SemiBold()
                                    .FontColor(PdfColors.Grey.Darken2);

                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(PdfColors.Teal.Medium)
                                            .Padding(5).Text("Categoria").FontColor(PdfColors.White).SemiBold();
                                        header.Cell().Background(PdfColors.Teal.Medium)
                                            .Padding(5).Text("Total").FontColor(PdfColors.White).SemiBold();
                                    });

                                    foreach (var item in relatorio.PorCategoria)
                                    {
                                        table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                            .Padding(5).Text(item.Categoria);
                                        table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                            .Padding(5).Text(item.Total.ToString()).AlignCenter();
                                    }
                                });
                            }
                        });

                        // Rodapé
                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                    });
                }).GeneratePdf(filePath);

                return filePath;
            });
        }

        public async Task<string> GerarRelatorioSuportesPdfAsync(RelatorioSuportes relatorio)
        {
            return await Task.Run(() =>
            {
                var fileName = $"Relatorio_Suportes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(PdfColors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Gestão de Chamados AI")
                                    .FontSize(20)
                                    .SemiBold()
                                    .FontColor(PdfColors.Teal.Medium);

                                column.Item().Text("Relatório de Desempenho dos Suportes")
                                    .FontSize(14)
                                    .FontColor(PdfColors.Grey.Darken2);

                                column.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .FontSize(9)
                                    .FontColor(PdfColors.Grey.Medium);
                            });
                        });

                        page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                        {
                            column.Spacing(20);

                            column.Item().Text($"Total de Suportes: {relatorio.TotalSuportes}")
                                .FontSize(14)
                                .SemiBold()
                                .FontColor(PdfColors.Teal.Medium);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Nome").FontColor(PdfColors.White).SemiBold();
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("E-mail").FontColor(PdfColors.White).SemiBold();
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Ativos").FontColor(PdfColors.White).SemiBold();
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Finalizados").FontColor(PdfColors.White).SemiBold();
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Total").FontColor(PdfColors.White).SemiBold();
                                });

                                foreach (var suporte in relatorio.Suportes)
                                {
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(suporte.Nome);
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(suporte.Email);
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(suporte.ChamadosAtivos.ToString()).AlignCenter();
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(suporte.ChamadosFinalizados.ToString()).AlignCenter();
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(suporte.TotalChamados.ToString()).AlignCenter();
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                    });
                }).GeneratePdf(filePath);

                return filePath;
            });
        }

        public async Task<string> GerarRelatorioCategoriasAsync(RelatorioCategorias relatorio)
        {
            return await Task.Run(() =>
            {
                var fileName = $"Relatorio_Categorias_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(PdfColors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Gestão de Chamados AI")
                                    .FontSize(20)
                                    .SemiBold()
                                    .FontColor(PdfColors.Teal.Medium);

                                column.Item().Text("Relatório de Chamados por Categoria")
                                    .FontSize(14)
                                    .FontColor(PdfColors.Grey.Darken2);

                                column.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                    .FontSize(9)
                                    .FontColor(PdfColors.Grey.Medium);
                            });
                        });

                        page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                        {
                            column.Spacing(15);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Categoria").FontColor(PdfColors.White).SemiBold();
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Total").FontColor(PdfColors.White).SemiBold();
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Abertos").FontColor(PdfColors.White).SemiBold();
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Atendimento").FontColor(PdfColors.White).SemiBold();
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Aguardando").FontColor(PdfColors.White).SemiBold();
                                    header.Cell().Background(PdfColors.Teal.Medium)
                                        .Padding(5).Text("Concluídos").FontColor(PdfColors.White).SemiBold();
                                });

                                foreach (var categoria in relatorio.Categorias)
                                {
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(categoria.Categoria);
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(categoria.Total.ToString()).AlignCenter();
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(categoria.Abertos.ToString()).AlignCenter();
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(categoria.EmAtendimento.ToString()).AlignCenter();
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(categoria.AguardandoCliente.ToString()).AlignCenter();
                                    table.Cell().Border(1).BorderColor(PdfColors.Grey.Lighten2)
                                        .Padding(5).Text(categoria.Fechados.ToString()).AlignCenter();
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                    });
                }).GeneratePdf(filePath);

                return filePath;
            });
        }

        private static void AddCard(QuestPDF.Infrastructure.IContainer container, string label, string value, string color)
        {
            container.Background(color).Padding(10).Column(column =>
            {
                column.Item().Text(label)
                    .FontSize(9)
                    .FontColor(PdfColors.White)
                    .SemiBold();

                column.Item().Text(value)
                    .FontSize(18)
                    .FontColor(PdfColors.White)
                    .Bold();
            });
        }
    }
#endif
}
