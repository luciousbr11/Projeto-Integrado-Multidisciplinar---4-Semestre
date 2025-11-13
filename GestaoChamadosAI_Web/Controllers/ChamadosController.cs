using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_Web.Data;
using GestaoChamadosAI_Web.Models;
using GestaoChamadosAI_Web.Services;

namespace GestaoChamadosAI_Web.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento completo de chamados (CRUD).
    /// Integra com o IAService para fornecer sugestões automáticas de soluçáo.
    /// </summary>
    public class ChamadosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAService _iaService;
        private readonly GeminiService _geminiService;

        // Construtor com injeçáo de dependência do contexto do banco e dos serviços de IA
        public ChamadosController(AppDbContext context, IAService iaService, GeminiService geminiService)
        {
            _context = context;
            _iaService = iaService;
            _geminiService = geminiService;
        }

        /// <summary>
        /// Action GET: Exibe a lista de todos os chamados cadastrados no sistema.
        /// Inclui informações do usuário associado e ordena por data de abertura (mais recentes primeiro).
        /// </summary>
        public async Task<IActionResult> Index(string? status = null, int? suporteId = null, string? prioridade = null)
        {
            // Busca todos os chamados incluindo os dados do usuário, ordenados por data
            var chamadosQuery = _context.Chamados
                .Include(c => c.Usuario) // Inclui dados do usuário que criou o chamado
                .Include(c => c.SuporteResponsavel) // Inclui dados do suporte responsável
                .AsQueryable();

            // Se for cliente, filtra apenas seus chamados
            if (User.IsInRole("Cliente"))
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = int.Parse(userIdClaim.Value);
                    chamadosQuery = chamadosQuery.Where(c => c.UsuarioId == userId);
                }
            }

            // Filtra por status se fornecido
            if (!string.IsNullOrEmpty(status))
            {
                chamadosQuery = chamadosQuery.Where(c => c.Status == status);
            }

            // Filtra por responsável se fornecido
            if (suporteId.HasValue)
            {
                chamadosQuery = chamadosQuery.Where(c => c.SuporteResponsavelId == suporteId.Value);
            }

            // Filtra por prioridade se fornecido
            if (!string.IsNullOrEmpty(prioridade))
            {
                chamadosQuery = chamadosQuery.Where(c => c.Prioridade == prioridade);
            }

            var chamados = await chamadosQuery
                .OrderByDescending(c => c.DataAbertura) // Mais recentes primeiro
                .ToListAsync();

            // Carrega lista de suportes para o filtro
            var suportes = await _context.Usuarios
                .Where(u => u.Tipo == "Suporte" || u.Tipo == "Administrador")
                .OrderBy(u => u.Nome)
                .ToListAsync();

            ViewBag.StatusFiltro = status;
            ViewBag.SuporteIdFiltro = suporteId;
            ViewBag.PrioridadeFiltro = prioridade;
            ViewBag.Suportes = suportes;
            
            return View(chamados);
        }

        /// <summary>
        /// Action GET: Exibe os detalhes de um chamado específico.
        /// Mostra todas as informações incluindo dados do usuário e análise da IA.
        /// </summary>
        /// <param name="id">ID do chamado a ser exibido</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Busca o chamado incluindo os dados do usuário e suporte responsável
            var chamado = await _context.Chamados
                .Include(c => c.Usuario)
                .Include(c => c.SuporteResponsavel)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chamado == null)
            {
                return NotFound();
            }

            // Busca lista de suportes para transferência
            var suportes = await _context.Usuarios
                .Where(u => u.Tipo == "Suporte")
                .OrderBy(u => u.Nome)
                .ToListAsync();
            
            ViewBag.Suportes = suportes;

            // Passa o ID do usuário atual para a view
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            ViewBag.UsuarioAtualId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            return View(chamado);
        }

        /// <summary>
        /// Action GET: Exibe o formulário de criaçáo de um novo chamado.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Action POST: Processa o formulário de criaçáo de um novo chamado.
        /// Valida os dados, analisa com IA, gera resposta automática e salva o chamado no banco de dados.
        /// O usuário é obtido automaticamente da sessáo.
        /// </summary>
        /// <param name="chamado">Objeto chamado com os dados do formulário</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descricao")] Chamado chamado)
        {
            // Obtém o ID do usuário logado automaticamente
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            chamado.UsuarioId = int.Parse(userIdClaim.Value);

            // Verifica se o cliente já tem um chamado em aberto
            if (User.IsInRole("Cliente"))
            {
                var chamadoEmAberto = await _context.Chamados
                    .Where(c => c.UsuarioId == chamado.UsuarioId && 
                                (c.Status == "Aberto" || c.Status == "Em Atendimento"))
                    .FirstOrDefaultAsync();

                if (chamadoEmAberto != null)
                {
                    TempData["Erro"] = "Você já possui um chamado em aberto. Finalize-o antes de abrir outro.";
                    return View(chamado);
                }
            }

            if (ModelState.IsValid)
            {
                // Define a data de abertura como a data/hora atual
                chamado.DataAbertura = DateTime.Now;
                
                // Define o status inicial como "Aberto"
                chamado.Status = "Aberto";

                // Analisa com a IA antiga (mantém compatibilidade)
                chamado.SugestaoIA = _iaService.AnalisarChamado(chamado.Titulo, chamado.Descricao);
                
                // 🤖 NOVA: Usa Google Gemini para categorizaçáo, prioridade e RESPOSTA AUTOMÁTICA
                try
                {
                    chamado.CategoriaIA = await _geminiService.CategorizarChamadoAsync(chamado.Titulo, chamado.Descricao);
                    chamado.Prioridade = await _geminiService.AnalisarPrioridadeAsync(chamado.Titulo, chamado.Descricao);
                    
                    // ✨ NOVO: Gera resposta automática da IA
                    chamado.RespostaIA = await _geminiService.GerarRespostaAsync(
                        chamado.Titulo, 
                        chamado.Descricao, 
                        chamado.CategoriaIA ?? "Outros"
                    );
                }
                catch (Exception ex)
                {
                    // Fallback para métodos antigos em caso de erro
                    chamado.CategoriaIA = IdentificarCategoria(chamado.Titulo, chamado.Descricao);
                    chamado.Prioridade = _iaService.ClassificarPrioridade(chamado.Titulo, chamado.Descricao);
                    chamado.RespostaIA = "No momento, náo foi possível gerar uma resposta automática. Um especialista irá atendê-lo em breve.";
                    
                    // Log do erro
                    Console.WriteLine($"Erro ao usar Gemini AI: {ex.Message}");
                }

                // Adiciona o chamado ao contexto
                _context.Add(chamado);
                await _context.SaveChangesAsync();

                // ✨ NOVO: Redireciona para a página de feedback da IA
                return RedirectToAction(nameof(Feedback), new { id = chamado.Id });
            }

            // Se o modelo náo for válido, recarrega a lista de usuários e retorna ao formulário
            ViewBag.Usuarios = new SelectList(_context.Usuarios.OrderBy(u => u.Nome), "Id", "Nome", chamado.UsuarioId);
            return View(chamado);
        }

        /// <summary>
        /// Identifica a categoria do problema baseado nas palavras-chave.
        /// </summary>
        private string IdentificarCategoria(string titulo, string descricao)
        {
            string texto = (titulo + " " + descricao).ToLower();

            if (texto.Contains("senha") || texto.Contains("login") || texto.Contains("acesso") || texto.Contains("bloqueado"))
                return "Problemas de Acesso";
            if (texto.Contains("lento") || texto.Contains("travando") || texto.Contains("performance") || texto.Contains("demora"))
                return "Problemas de Performance";
            if (texto.Contains("erro") || texto.Contains("falha") || texto.Contains("crash") || texto.Contains("bug"))
                return "Erros Gerais";
            if (texto.Contains("impressora") || texto.Contains("imprim") || texto.Contains("papel"))
                return "Problemas de Impressáo";
            if (texto.Contains("email") || texto.Contains("e-mail") || texto.Contains("outlook") || texto.Contains("mensagem"))
                return "Problemas de E-mail";
            if (texto.Contains("internet") || texto.Contains("rede") || texto.Contains("wifi") || texto.Contains("conexáo"))
                return "Problemas de Rede";
            if (texto.Contains("instala") || texto.Contains("software") || texto.Contains("programa") || texto.Contains("aplicativo"))
                return "Instalaçáo de Software";
            if (texto.Contains("backup") || texto.Contains("recuper") || texto.Contains("arquivo perdido"))
                return "Backup e Recuperaçáo";
            if (texto.Contains("tela") || texto.Contains("monitor") || texto.Contains("vídeo") || texto.Contains("display"))
                return "Problemas de Vídeo";
            if (texto.Contains("mouse") || texto.Contains("teclado") || texto.Contains("usb") || texto.Contains("periférico"))
                return "Periféricos";

            return "Outros";
        }

        /// <summary>
        /// Action GET: Exibe o formulário de ediçáo de um chamado existente.
        /// Carrega os dados atuais do chamado e a lista de usuários.
        /// </summary>
        /// <param name="id">ID do chamado a ser editado</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chamado = await _context.Chamados.FindAsync(id);
            
            if (chamado == null)
            {
                return NotFound();
            }

            // Prepara SelectList com os usuários, marcando o atual como selecionado
            ViewBag.Usuarios = new SelectList(_context.Usuarios.OrderBy(u => u.Nome), "Id", "Nome", chamado.UsuarioId);
            
            // Gera a sugestáo atualizada da IA
            ViewBag.SugestaoIA = _iaService.AnalisarChamado(chamado.Titulo, chamado.Descricao);
            ViewBag.Prioridade = _iaService.ClassificarPrioridade(chamado.Titulo, chamado.Descricao);

            return View(chamado);
        }

        /// <summary>
        /// Action POST: Processa o formulário de ediçáo de um chamado.
        /// Atualiza os dados do chamado no banco de dados.
        /// </summary>
        /// <param name="id">ID do chamado sendo editado</param>
        /// <param name="chamado">Objeto com os novos dados do chamado</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,DataAbertura,Status,UsuarioId")] Chamado chamado)
        {
            if (id != chamado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Busca o chamado original do banco de dados
                    var chamadoOriginal = await _context.Chamados
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (chamadoOriginal == null)
                    {
                        return NotFound();
                    }

                    // Valida se o status está sendo alterado sem ter assumido atendimento
                    if (chamadoOriginal.Status != chamado.Status && 
                        chamadoOriginal.SuporteResponsavelId == null)
                    {
                        TempData["Erro"] = "Não é possível alterar o status do chamado sem antes assumir o atendimento!";
                        return RedirectToAction(nameof(Edit), new { id = id });
                    }

                    // Atualiza o chamado no contexto
                    _context.Update(chamado);
                    await _context.SaveChangesAsync();

                    TempData["Mensagem"] = "Chamado atualizado com sucesso!";
                    
                    // Gera nova sugestáo da IA após atualizaçáo
                    TempData["SugestaoIA"] = _iaService.AnalisarChamado(chamado.Titulo, chamado.Descricao);
                    TempData["Prioridade"] = _iaService.ClassificarPrioridade(chamado.Titulo, chamado.Descricao);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChamadoExists(chamado.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Details), new { id = chamado.Id });
            }

            // Se o modelo náo for válido, recarrega os dados necessários
            ViewBag.Usuarios = new SelectList(_context.Usuarios.OrderBy(u => u.Nome), "Id", "Nome", chamado.UsuarioId);
            ViewBag.SugestaoIA = _iaService.AnalisarChamado(chamado.Titulo, chamado.Descricao);
            ViewBag.Prioridade = _iaService.ClassificarPrioridade(chamado.Titulo, chamado.Descricao);
            
            return View(chamado);
        }

        /// <summary>
        /// Action GET: Exibe a página de confirmaçáo de exclusáo de um chamado.
        /// Mostra os dados do chamado antes de confirmar a exclusáo.
        /// </summary>
        /// <param name="id">ID do chamado a ser excluído</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Busca o chamado incluindo os dados do usuário
            var chamado = await _context.Chamados
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chamado == null)
            {
                return NotFound();
            }

            return View(chamado);
        }

        /// <summary>
        /// Action POST: Processa a exclusáo confirmada de um chamado.
        /// Remove o chamado do banco de dados.
        /// </summary>
        /// <param name="id">ID do chamado a ser excluído</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chamado = await _context.Chamados.FindAsync(id);
            
            if (chamado != null)
            {
                _context.Chamados.Remove(chamado);
                await _context.SaveChangesAsync();
                
                TempData["Mensagem"] = "Chamado excluído com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Action AJAX: Retorna uma sugestáo da IA em tempo real baseada no título e descriçáo.
        /// Útil para mostrar sugestões enquanto o usuário digita.
        /// </summary>
        /// <param name="titulo">Título do problema</param>
        /// <param name="descricao">Descriçáo do problema</param>
        /// <returns>JSON com a sugestáo e a prioridade</returns>
        [HttpPost]
        public IActionResult ObterSugestaoIA(string titulo, string descricao)
        {
            var sugestao = _iaService.AnalisarChamado(titulo, descricao);
            var prioridade = _iaService.ClassificarPrioridade(titulo, descricao);

            return Json(new { sugestao, prioridade });
    }

    /// <summary>
    /// Action GET: Filtra chamados por status.
    /// Permite visualizar apenas chamados abertos, em andamento ou concluídos.
    /// </summary>
    /// <param name="status">Status para filtrar (Aberto, Em Andamento, Concluído)</param>
    public async Task<IActionResult> FiltrarPorStatus(string status)
    {
        IQueryable<Chamado> query = _context.Chamados.Include(c => c.Usuario);            // Aplica filtro se um status foi especificado
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status == status);
                ViewBag.StatusFiltro = status;
            }

            var chamados = await query
                .OrderByDescending(c => c.DataAbertura)
                .ToListAsync();

            return View("Index", chamados);
        }

        /// <summary>
        /// Action GET: Filtra chamados por usuário.
        /// Permite visualizar todos os chamados de um usuário específico.
        /// </summary>
        /// <param name="usuarioId">ID do usuário para filtrar</param>
        public async Task<IActionResult> FiltrarPorUsuario(int? usuarioId)
        {
            IQueryable<Chamado> query = _context.Chamados.Include(c => c.Usuario);

            if (usuarioId.HasValue)
            {
                query = query.Where(c => c.UsuarioId == usuarioId.Value);
                var usuario = await _context.Usuarios.FindAsync(usuarioId.Value);
                ViewBag.UsuarioFiltro = usuario?.Nome;
            }

            var chamados = await query
                .OrderByDescending(c => c.DataAbertura)
                .ToListAsync();

            return View("Index", chamados);
        }

        /// <summary>
        /// 🤖 Action POST: Gera resposta automática usando Google Gemini AI.
        /// Analisa o chamado e cria uma resposta profissional sugerida.
        /// </summary>
        /// <param name="id">ID do chamado</param>
        [HttpPost]
        [Route("api/Chamados/{id}/gerar-resposta-ia")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GerarRespostaIA(int id)
        {
            var chamado = await _context.Chamados.FindAsync(id);
            
            if (chamado == null)
            {
                return NotFound();
            }

            try
            {
                // Log para debug
                Console.WriteLine($"Tentando gerar resposta para chamado #{id}");
                Console.WriteLine($"Título: {chamado.Titulo}");
                Console.WriteLine($"Categoria: {chamado.CategoriaIA}");
                
                // Gera resposta usando Google Gemini
                chamado.RespostaIA = await _geminiService.GerarRespostaAsync(
                    chamado.Titulo, 
                    chamado.Descricao, 
                    chamado.CategoriaIA ?? "Outros"
                );

                // Atualiza o chamado no banco
                _context.Update(chamado);
                await _context.SaveChangesAsync();

                TempData["Mensagem"] = "Resposta gerada com sucesso pela IA!";
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"ERRO NO CONTROLLER: {ex.GetType().Name}");
                Console.WriteLine($"Mensagem: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                
                TempData["Erro"] = $"Erro ao gerar resposta: {ex.Message}. Verifique sua API Key e conexáo com internet.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// 🧪 Action de teste para verificar conexáo com Google Gemini
        /// </summary>
        public async Task<IActionResult> TestarGemini()
        {
            try
            {
                var resultado = await _geminiService.TestarConexaoAsync();
                if (resultado)
                {
                    return Content("✅ Conexáo com Google Gemini OK! API funcionando corretamente.");
                }
                else
                {
                    return Content("❌ Erro: Náo foi possível conectar com Google Gemini. Verifique a API Key.");
                }
            }
            catch (Exception ex)
            {
                return Content($"❌ ERRO: {ex.GetType().Name} - {ex.Message}\n\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 🤖 Action GET: Exibe a página de feedback da resposta da IA
        /// Pergunta ao usuário se a resposta da IA resolveu o problema
        /// </summary>
        public async Task<IActionResult> Feedback(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chamado = await _context.Chamados
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chamado == null)
            {
                return NotFound();
            }

            // Se náo tem resposta da IA, redireciona para detalhes
            if (string.IsNullOrEmpty(chamado.RespostaIA))
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(chamado);
        }

        /// <summary>
        /// 🤖 Action POST: Processa o feedback do usuário sobre a resposta da IA
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessarFeedback(int id, bool resolvido)
        {
            var chamado = await _context.Chamados.FindAsync(id);
            
            if (chamado == null)
            {
                return NotFound();
            }

            // Registra o feedback do usuário
            chamado.FeedbackResolvido = resolvido;
            chamado.DataFeedback = DateTime.Now;

            if (resolvido)
            {
                // Se foi resolvido pela IA, muda o status
                chamado.Status = "Solucionado por IA";
                TempData["Mensagem"] = "Ótimo! Ficamos felizes que a IA pôde resolver seu problema! 🎉";
            }
            else
            {
                // Mantém como Aberto para o suporte assumir
                chamado.Status = "Aberto";
                TempData["Mensagem"] = "Entendido! Um especialista irá atender seu chamado em breve. 👨‍💻";
            }

            _context.Update(chamado);
            await _context.SaveChangesAsync();

            // Redireciona para a lista de chamados (ou detalhes se preferir)
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Action POST: Transfere um chamado para outro suporte.
        /// Apenas suportes e administradores podem transferir chamados.
        /// </summary>
        [HttpPost]
        [Route("api/Chamados/{chamadoId}/transferir")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<IActionResult> TransferirChamado(int chamadoId, int novoSuporteId)
        {
            var chamado = await _context.Chamados
                .Include(c => c.SuporteResponsavel)
                .FirstOrDefaultAsync(c => c.Id == chamadoId);
            
            if (chamado == null)
            {
                TempData["Erro"] = "Chamado não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Bloqueia transferência de chamados finalizados
            if (chamado.Status == "Concluído" || chamado.Status == "Solucionado por IA")
            {
                TempData["Erro"] = "Não é possível transferir um chamado já finalizado.";
                return RedirectToAction(nameof(Details), new { id = chamadoId });
            }

            var novoSuporte = await _context.Usuarios.FindAsync(novoSuporteId);
            if (novoSuporte == null || novoSuporte.Tipo != "Suporte")
            {
                TempData["Erro"] = "Suporte inválido.";
                return RedirectToAction(nameof(Details), new { id = chamadoId });
            }

            // Obtém o nome do suporte anterior para a mensagem
            var suporteAnteriorNome = chamado.SuporteResponsavel?.Nome ?? "Sistema";

            // Atualiza o responsável e volta status para Aberto (aguardando novo suporte assumir)
            chamado.SuporteResponsavelId = novoSuporteId;
            chamado.Status = "Aberto";
            _context.Update(chamado);

            // Cria mensagem de sistema informando a transferência
            var mensagemTransferencia = new MensagemChamado
            {
                ChamadoId = chamadoId,
                UsuarioId = novoSuporteId,
                Mensagem = $"🔄 Atendimento transferido de {suporteAnteriorNome} para {novoSuporte.Nome}. O chamado está aguardando ser assumido.",
                DataEnvio = DateTime.Now,
                LidaPorCliente = false,
                LidaPorSuporte = true
            };

            _context.MensagensChamados.Add(mensagemTransferencia);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = $"Chamado transferido com sucesso para {novoSuporte.Nome}!";
            return RedirectToAction(nameof(Details), new { id = chamadoId });
        }

        /// <summary>
        /// Action POST: Permite que um suporte reassuma um chamado que ele havia transferido.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<IActionResult> Reassumir(int chamadoId)
        {
            var chamado = await _context.Chamados
                .Include(c => c.SuporteResponsavel)
                .FirstOrDefaultAsync(c => c.Id == chamadoId);

            if (chamado == null)
            {
                TempData["Erro"] = "Chamado não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);
            var usuarioAtual = await _context.Usuarios.FindAsync(userId);

            if (usuarioAtual == null)
            {
                TempData["Erro"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Obtém o nome do suporte atual para a mensagem
            var suporteAtualNome = chamado.SuporteResponsavel?.Nome ?? "Sistema";

            // Atualiza o responsável para o usuário que está reassumindo
            chamado.SuporteResponsavelId = userId;
            _context.Update(chamado);

            // Cria mensagem de sistema informando a reassunção
            var mensagemReassumir = new MensagemChamado
            {
                ChamadoId = chamadoId,
                UsuarioId = userId,
                Mensagem = $"🔁 Atendimento reassumido por {usuarioAtual.Nome} (anteriormente com {suporteAtualNome}).",
                DataEnvio = DateTime.Now,
                LidaPorCliente = false,
                LidaPorSuporte = true
            };

            _context.MensagensChamados.Add(mensagemReassumir);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Você reassumiu este atendimento com sucesso!";
            return RedirectToAction(nameof(Details), new { id = chamadoId });
        }

        /// <summary>
        /// Permite que o cliente finalize seu próprio chamado.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> FinalizarChamado(int chamadoId)
        {
            var chamado = await _context.Chamados
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == chamadoId);

            if (chamado == null)
            {
                return NotFound();
            }

            // Verifica se o chamado pertence ao usuário logado
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);
            if (chamado.UsuarioId != userId)
            {
                TempData["Erro"] = "Você não tem permissão para finalizar este chamado.";
                return RedirectToAction(nameof(Details), new { id = chamadoId });
            }

            // Verifica se o chamado pode ser finalizado
            if (chamado.Status != "Aberto" && chamado.Status != "Em Andamento")
            {
                TempData["Erro"] = "Este chamado não pode ser finalizado.";
                return RedirectToAction(nameof(Details), new { id = chamadoId });
            }

            chamado.Status = "Concluído";
            _context.Update(chamado);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Chamado finalizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Método auxiliar privado que verifica se um chamado existe no banco de dados.
        /// Utilizado para validações e tratamento de erros.
        /// </summary>
        /// <param name="id">ID do chamado a verificar</param>
        /// <returns>True se o chamado existe, False caso contrário</returns>
        private bool ChamadoExists(int id)
        {
            return _context.Chamados.Any(e => e.Id == id);
        }
    }
}
