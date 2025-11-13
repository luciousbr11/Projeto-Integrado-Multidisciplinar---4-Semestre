using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_Web.Data;
using GestaoChamadosAI_Web.Models;

namespace GestaoChamadosAI_Web.Controllers
{
    /// <summary>
    /// Controller responsável pelo sistema de chat entre suporte e cliente.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Exibe a interface de chat para um chamado específico.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int chamadoId)
        {
            var chamado = await _context.Chamados
                .Include(c => c.Usuario)
                .Include(c => c.SuporteResponsavel)
                .Include(c => c.Mensagens)
                    .ThenInclude(m => m.Usuario)
                .Include(c => c.Mensagens)
                    .ThenInclude(m => m.Anexos)
                .FirstOrDefaultAsync(c => c.Id == chamadoId);

            if (chamado == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Verifica se o usuário tem permissáo para ver este chat
            if (userRole == "Cliente" && chamado.UsuarioId != userId)
            {
                return Forbid();
            }

            // Suporte só pode ver chat se for o responsável atual
            if (userRole == "Suporte" && chamado.SuporteResponsavelId != userId)
            {
                TempData["Erro"] = "Este chamado não está atribuído a você. Ele foi transferido para outro suporte.";
                return RedirectToAction("Details", "Chamados", new { id = chamadoId });
            }

            // Marca mensagens como lidas
            if (userRole == "Cliente")
            {
                var mensagensNaoLidas = chamado.Mensagens.Where(m => !m.LidaPorCliente && m.UsuarioId != userId);
                foreach (var msg in mensagensNaoLidas)
                {
                    msg.LidaPorCliente = true;
                }
            }
            else if (userRole == "Suporte" || userRole == "Administrador")
            {
                var mensagensNaoLidas = chamado.Mensagens.Where(m => !m.LidaPorSuporte && m.UsuarioId != userId);
                foreach (var msg in mensagensNaoLidas)
                {
                    msg.LidaPorSuporte = true;
                }
            }

            await _context.SaveChangesAsync();

            // Busca lista de suportes para transferência
            var suportes = await _context.Usuarios
                .Where(u => u.Tipo == "Suporte")
                .OrderBy(u => u.Nome)
                .ToListAsync();

            ViewBag.UsuarioAtualId = userId;
            ViewBag.UsuarioRole = userRole;
            ViewBag.Suportes = suportes;

            return View(chamado);
        }

        /// <summary>
        /// Action para o suporte assumir o atendimento do chamado.
        /// </summary>
        [HttpPost]
        [Route("api/Chat/AssumirAtendimento")]
        [Authorize(AuthenticationSchemes = "Bearer,Cookies", Roles = "Suporte,Administrador")]
        public async Task<IActionResult> AssumirAtendimento([FromBody] AssumirAtendimentoRequest request)
        {
            var chamadoId = request?.ChamadoId ?? 0;
            
            var chamado = await _context.Chamados
                .Include(c => c.SuporteResponsavel)
                .FirstOrDefaultAsync(c => c.Id == chamadoId);
            
            if (chamado == null)
            {
                // Verifica se é requisição AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                    Request.Headers["Content-Type"].ToString().Contains("application/json"))
                {
                    return Json(new { success = false, message = "Chamado não encontrado." });
                }
                TempData["Erro"] = "Chamado não encontrado.";
                return RedirectToAction("Details", "Chamados", new { id = chamadoId });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                    Request.Headers["Content-Type"].ToString().Contains("application/json"))
                {
                    return Json(new { success = false, message = "Usuário não autenticado." });
                }
                TempData["Erro"] = "Usuário não autenticado.";
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);
            var usuario = await _context.Usuarios.FindAsync(userId);
            
            // Valida se o usuário existe e tem permissão
            if (usuario == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                    Request.Headers["Content-Type"].ToString().Contains("application/json"))
                {
                    return Json(new { success = false, message = "Usuário não encontrado." });
                }
                TempData["Erro"] = "Usuário não encontrado.";
                return RedirectToAction("Details", "Chamados", new { id = chamadoId });
            }

            // Verifica se é Suporte ou Administrador
            if (usuario.Tipo != "Suporte" && usuario.Tipo != "Administrador")
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                    Request.Headers["Content-Type"].ToString().Contains("application/json"))
                {
                    return Json(new { success = false, message = "Você não tem permissão para assumir atendimentos." });
                }
                TempData["Erro"] = "Você não tem permissão para assumir atendimentos.";
                return RedirectToAction("Details", "Chamados", new { id = chamadoId });
            }

            // Bloqueia assumir atendimento de chamados finalizados
            if (chamado.Status == "Concluído" || chamado.Status == "Solucionado por IA")
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                    Request.Headers["Content-Type"].ToString().Contains("application/json"))
                {
                    return Json(new { success = false, message = "Não é possível assumir um chamado já finalizado." });
                }
                TempData["Erro"] = "Não é possível assumir um chamado já finalizado.";
                return RedirectToAction("Details", "Chamados", new { id = chamadoId });
            }
            
            var suporteAnterior = chamado.SuporteResponsavel;

            // Atribui o novo responsável
            chamado.SuporteResponsavelId = userId;
            chamado.Status = "Em Atendimento";

            await _context.SaveChangesAsync();

            // Cria mensagem apropriada conforme situação
            string mensagemTexto;
            if (suporteAnterior != null)
            {
                // Admin assumiu de um suporte
                mensagemTexto = $"⚡ Atendimento assumido por {usuario?.Nome} (Administrador). {suporteAnterior.Nome} não tem mais acesso a este chamado.";
            }
            else
            {
                // Primeiro a assumir
                mensagemTexto = "📢 Atendimento iniciado. Como posso ajudá-lo?";
            }

            var mensagemSistema = new MensagemChamado
            {
                ChamadoId = chamadoId,
                UsuarioId = userId,
                Mensagem = mensagemTexto,
                DataEnvio = DateTime.Now,
                LidaPorCliente = false,
                LidaPorSuporte = true
            };

            _context.MensagensChamados.Add(mensagemSistema);
            await _context.SaveChangesAsync();

            // Verifica se é requisição AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                Request.Headers["Content-Type"].ToString().Contains("application/json"))
            {
                return Json(new { success = true, message = "Atendimento assumido com sucesso!" });
            }

            TempData["Mensagem"] = "Atendimento assumido com sucesso!";
            return RedirectToAction("Index", new { chamadoId = chamadoId });
        }

        /// <summary>
        /// Envia uma nova mensagem no chat.
        /// </summary>
        [HttpPost]
        [Route("api/Chat/{chamadoId}/mensagens")]
        public async Task<IActionResult> EnviarMensagem(int chamadoId, [FromForm] string mensagem, [FromForm] List<string>? anexosUrls = null)
        {
            Console.WriteLine($"[API-CHAT] ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"[API-CHAT] 📥 EnviarMensagem RECEBIDO");
            Console.WriteLine($"[API-CHAT] 🔢 ChamadoId: {chamadoId}");
            Console.WriteLine($"[API-CHAT] 📝 Mensagem: '{mensagem}'");
            Console.WriteLine($"[API-CHAT] 📎 AnexosUrls: {anexosUrls?.Count ?? 0}");
            Console.WriteLine($"[API-CHAT] 🔐 User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"[API-CHAT] 👤 User.Identity.Name: {User.Identity?.Name}");
            
            if (string.IsNullOrWhiteSpace(mensagem) && (anexosUrls == null || !anexosUrls.Any()))
            {
                Console.WriteLine($"[API-CHAT] ❌ FALHA: Mensagem/anexo não fornecido");
                return Json(new { success = false, message = "Mensagem ou anexo deve ser fornecido." });
            }

            var chamado = await _context.Chamados.FindAsync(chamadoId);
            
            if (chamado == null)
            {
                Console.WriteLine($"[API-CHAT] ❌ FALHA: Chamado não encontrado");
                return Json(new { success = false, message = "Chamado não encontrado." });
            }
            Console.WriteLine($"[API-CHAT] ✅ Chamado encontrado: #{chamado.Id}");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                Console.WriteLine($"[API-CHAT] ❌ FALHA: NameIdentifier claim não encontrado");
                Console.WriteLine($"[API-CHAT] 📋 Claims disponíveis:");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"[API-CHAT]    - {claim.Type}: {claim.Value}");
                }
                return Json(new { success = false, message = "Usuário não autenticado." });
            }

            var userId = int.Parse(userIdClaim.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"[API-CHAT] 👤 UserId: {userId}");
            Console.WriteLine($"[API-CHAT] 🎭 UserRole: {userRole}");

            // Verifica se o usuário tem permissão
            if (userRole == "Cliente" && chamado.UsuarioId != userId)
            {
                Console.WriteLine($"[API-CHAT] ❌ FALHA: Cliente sem permissão (chamado.UsuarioId={chamado.UsuarioId})");
                return Json(new { success = false, message = "Você não tem permissão para enviar mensagens neste chamado." });
            }

            // Suporte só pode enviar mensagem se for o responsável atual
            if (userRole == "Suporte" && chamado.SuporteResponsavelId != userId)
            {
                Console.WriteLine($"[API-CHAT] ❌ FALHA: Suporte não é responsável (chamado.SuporteResponsavelId={chamado.SuporteResponsavelId})");
                return Json(new { success = false, message = "Este chamado foi transferido para outro suporte. Você não pode mais enviar mensagens." });
            }

            var novaMensagem = new MensagemChamado
            {
                ChamadoId = chamadoId,
                UsuarioId = userId,
                Mensagem = mensagem ?? "",
                DataEnvio = DateTime.Now,
                LidaPorCliente = userRole != "Cliente",
                LidaPorSuporte = userRole == "Suporte" || userRole == "Administrador"
            };

            Console.WriteLine($"[API-CHAT] ✏️ Criando nova mensagem...");
            _context.MensagensChamados.Add(novaMensagem);
            await _context.SaveChangesAsync();
            Console.WriteLine($"[API-CHAT] ✅ Mensagem salva com ID: {novaMensagem.Id}");

            // Salvar anexos se houver
            var anexos = new List<object>();
            if (anexosUrls != null && anexosUrls.Any())
            {
                Console.WriteLine($"[API-CHAT] 📎 Salvando {anexosUrls.Count} anexos...");
                foreach (var url in anexosUrls)
                {
                    if (string.IsNullOrWhiteSpace(url)) continue;

                    var fileName = Path.GetFileName(new Uri(url).LocalPath);
                    var filePath = $"/uploads/{fileName}";

                    var anexo = new AnexoMensagem
                    {
                        MensagemChamadoId = novaMensagem.Id,
                        NomeArquivo = fileName,
                        CaminhoArquivo = filePath,
                        TipoArquivo = Path.GetExtension(fileName).ToLowerInvariant(),
                        TamanhoBytes = 0, // Será calculado pelo cliente se necessário
                        DataUpload = DateTime.Now
                    };

                    _context.AnexosMensagens.Add(anexo);
                    
                    anexos.Add(new
                    {
                        id = anexo.Id,
                        nomeArquivo = anexo.NomeArquivo,
                        url = url,
                        tipo = anexo.TipoArquivo
                    });
                }

                await _context.SaveChangesAsync();
                Console.WriteLine($"[API-CHAT] ✅ Anexos salvos");
            }

            var usuario = await _context.Usuarios.FindAsync(userId);
            Console.WriteLine($"[API-CHAT] ✅ SUCCESS - Retornando resposta");
            Console.WriteLine($"[API-CHAT] ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            return Json(new
            {
                success = true,
                mensagem = new
                {
                    id = novaMensagem.Id,
                    usuarioNome = usuario?.Nome,
                    mensagem = novaMensagem.Mensagem,
                    dataEnvio = novaMensagem.DataEnvio.ToString("dd/MM/yyyy HH:mm"),
                    isUsuarioAtual = true,
                    anexos = anexos
                }
            });
        }

        /// <summary>
        /// Busca novas mensagens (para atualizaçáo automática via AJAX).
        /// </summary>
        [HttpGet]
        [Route("api/Chat/{chamadoId}")]
        public async Task<IActionResult> BuscarNovasMensagens(int chamadoId, int ultimaMensagemId = 0)
        {
            var mensagens = await _context.MensagensChamados
                .Include(m => m.Usuario)
                .Include(m => m.Anexos)
                .Where(m => m.ChamadoId == chamadoId)
                .OrderBy(m => m.DataEnvio)
                .ToListAsync();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            var mensagensDto = mensagens.Select(m => new
            {
                id = m.Id,
                chamadoId = m.ChamadoId,
                usuarioId = m.UsuarioId,
                nomeUsuario = m.Usuario?.Nome ?? "Sistema",
                tipoUsuario = m.Usuario?.Tipo ?? "Sistema",
                mensagem = m.Mensagem,
                dataEnvio = m.DataEnvio,
                isMinhaMensagem = m.UsuarioId == userId,
                anexos = m.Anexos.Select(a => new
                {
                    id = a.Id,
                    nomeArquivo = a.NomeArquivo,
                    url = $"{Request.Scheme}://{Request.Host}{a.CaminhoArquivo}",
                    tipo = a.TipoArquivo
                }).ToList()
            });

            return Json(new { success = true, data = mensagensDto });
        }

        /// <summary>
        /// Finaliza o atendimento do chamado.
        /// </summary>
        [HttpPost]
        [Route("api/Chat/FinalizarAtendimento")]
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<IActionResult> FinalizarAtendimento(int chamadoId)
        {
            var chamado = await _context.Chamados.FindAsync(chamadoId);
            
            if (chamado == null)
            {
                return Json(new { success = false, message = "Chamado não encontrado." });
            }

            chamado.Status = "Concluído";
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Atendimento finalizado com sucesso!",
                redirectUrl = Url.Action("Index", "Chamados")
            });
        }
    }

    // Classe auxiliar para receber request JSON
    public class AssumirAtendimentoRequest
    {
        public int ChamadoId { get; set; }
    }
}
