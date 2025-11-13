using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_API.Data;
using GestaoChamadosAI_API.DTOs.Chat;
using GestaoChamadosAI_API.Helpers;
using GestaoChamadosAI_API.Models;
using System.Security.Claims;

namespace GestaoChamadosAI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ChatController> _logger;

        public ChatController(AppDbContext context, ILogger<ChatController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todas as mensagens de um chamado específico
        /// </summary>
        /// <param name="chamadoId">ID do chamado</param>
        /// <returns>Lista de mensagens do chamado</returns>
        [HttpGet("{chamadoId}")]
        public async Task<ActionResult<ApiResponse<List<MensagemResponseDto>>>> GetMensagens(int chamadoId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Verificar se o chamado existe
                var chamado = await _context.Chamados
                    .Include(c => c.Usuario)
                    .Include(c => c.SuporteResponsavel)
                    .FirstOrDefaultAsync(c => c.Id == chamadoId);

                if (chamado == null)
                {
                    return NotFound(ApiResponse<List<MensagemResponseDto>>.ErrorResult("Chamado não encontrado"));
                }

                // Verificar permissão de acesso
                if (userRole == "Cliente" && chamado.UsuarioId != userId)
                {
                    return Forbid();
                }

                // Buscar mensagens
                var mensagens = await _context.MensagensChamados
                    .Include(m => m.Usuario)
                    .Include(m => m.Anexos)  // ← ADICIONAR ANEXOS
                    .Where(m => m.ChamadoId == chamadoId)
                    .OrderBy(m => m.DataEnvio)
                    .Select(m => new MensagemResponseDto
                    {
                        Id = m.Id,
                        ChamadoId = m.ChamadoId,
                        UsuarioId = m.UsuarioId,
                        NomeUsuario = m.Usuario.Nome,
                        TipoUsuario = m.Usuario.Tipo,
                        Mensagem = m.Mensagem,
                        DataEnvio = m.DataEnvio,
                        Anexos = m.Anexos.Select(a => new AnexoResponseDto
                        {
                            Id = a.Id,
                            NomeArquivo = a.NomeArquivo,
                            Url = $"{Request.Scheme}://{Request.Host}{a.CaminhoArquivo}",
                            Tipo = a.TipoArquivo
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<MensagemResponseDto>>.SuccessResult(mensagens));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar mensagens do chamado {ChamadoId}", chamadoId);
                return StatusCode(500, ApiResponse<List<MensagemResponseDto>>.ErrorResult("Erro ao buscar mensagens"));
            }
        }

        /// <summary>
        /// Envia uma nova mensagem em um chamado
        /// </summary>
        /// <param name="chamadoId">ID do chamado</param>
        /// <param name="request">Dados da mensagem</param>
        /// <returns>Mensagem criada</returns>
        [HttpPost("{chamadoId}/mensagens")]
        [Consumes("application/json", "application/x-www-form-urlencoded", "multipart/form-data")]
        public async Task<ActionResult<ApiResponse<MensagemResponseDto>>> EnviarMensagem(
            int chamadoId,
            [FromForm] EnviarMensagemDto request)
        {
            try
            {
                Console.WriteLine($"[API-CHAT] 📥 EnviarMensagem RECEBIDO");
                Console.WriteLine($"[API-CHAT] 🔢 ChamadoId: {chamadoId}");
                Console.WriteLine($"[API-CHAT] 📝 Request.Mensagem: '{request?.Mensagem}'");
                Console.WriteLine($"[API-CHAT] 📎 Request.AnexosUrls: {request?.AnexosUrls?.Count ?? 0}");
                
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                Console.WriteLine($"[API-CHAT] 🔐 User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
                Console.WriteLine($"[API-CHAT] 👤 UserId: {userId}");
                Console.WriteLine($"[API-CHAT] 🎭 UserRole: {userRole}");

                // Verificar se o chamado existe
                var chamado = await _context.Chamados
                    .Include(c => c.Usuario)
                    .Include(c => c.SuporteResponsavel)
                    .FirstOrDefaultAsync(c => c.Id == chamadoId);

                if (chamado == null)
                {
                    Console.WriteLine($"[API-CHAT] ❌ FALHA: Chamado {chamadoId} não encontrado");
                    return NotFound(ApiResponse<MensagemResponseDto>.ErrorResult("Chamado não encontrado"));
                }

                Console.WriteLine($"[API-CHAT] ✅ Chamado encontrado: #{chamado.Id} - Status: {chamado.Status}");
                Console.WriteLine($"[API-CHAT] 👥 Chamado.UsuarioId: {chamado.UsuarioId}, Chamado.SuporteResponsavelId: {chamado.SuporteResponsavelId}");

                // Verificar permissão de acesso
                if (userRole == "Cliente" && chamado.UsuarioId != userId)
                {
                    Console.WriteLine($"[API-CHAT] ❌ FALHA: Cliente {userId} não é dono do chamado (dono: {chamado.UsuarioId})");
                    return Forbid();
                }

                Console.WriteLine($"[API-CHAT] ✅ Permissão validada");

                // Verificar se o chamado está fechado
                if (chamado.Status == "Concluído")
                {
                    Console.WriteLine($"[API-CHAT] ❌ FALHA: Chamado já está concluído");
                    return BadRequest(ApiResponse<MensagemResponseDto>.ErrorResult("Não é possível enviar mensagens em chamados concluídos"));
                }

                // Criar a mensagem
                Console.WriteLine($"[API-CHAT] ✏️ Criando nova mensagem...");
                var mensagem = new MensagemChamado
                {
                    ChamadoId = chamadoId,
                    UsuarioId = userId,
                    Mensagem = request.Mensagem,
                    DataEnvio = DateTime.Now,
                    Anexos = new List<AnexoMensagem>()
                };

                // Processar anexos se houver
                if (request.AnexosUrls != null && request.AnexosUrls.Any())
                {
                    Console.WriteLine($"[API-CHAT] 📎 Processando {request.AnexosUrls.Count} anexos...");
                    foreach (var url in request.AnexosUrls)
                    {
                        // Extrair informações da URL
                        var uri = new Uri(url);
                        var nomeArquivoComExtensao = Path.GetFileName(uri.LocalPath);
                        var extensao = Path.GetExtension(nomeArquivoComExtensao);
                        
                        var anexo = new AnexoMensagem
                        {
                            NomeArquivo = nomeArquivoComExtensao,
                            CaminhoArquivo = uri.LocalPath, // Ex: /uploads/guid.pdf
                            TipoArquivo = GetContentType(extensao),
                            TamanhoBytes = 0, // Pode ser calculado se necessário
                            DataUpload = DateTime.Now
                        };
                        
                        mensagem.Anexos.Add(anexo);
                        Console.WriteLine($"[API-CHAT] 📎 Anexo adicionado: {nomeArquivoComExtensao}");
                    }
                }

                _context.MensagensChamados.Add(mensagem);
                Console.WriteLine($"[API-CHAT] 💾 Salvando mensagem no banco...");

                // Atualizar status do chamado se necessário
                if (userRole == "Cliente" && chamado.Status == "Aguardando Cliente")
                {
                    chamado.Status = "Em Atendimento";
                    Console.WriteLine($"[API-CHAT] 🔄 Status atualizado: Aguardando Cliente → Em Atendimento");
                }
                else if (userRole == "Suporte" && chamado.Status == "Aberto")
                {
                    chamado.Status = "Em Atendimento";
                    Console.WriteLine($"[API-CHAT] 🔄 Status atualizado: Aberto → Em Atendimento");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine($"[API-CHAT] ✅ Mensagem salva com ID: {mensagem.Id}");

                // Buscar mensagem completa com dados do usuário e anexos
                var mensagemCompleta = await _context.MensagensChamados
                    .Include(m => m.Usuario)
                    .Include(m => m.Anexos)
                    .Where(m => m.Id == mensagem.Id)
                    .Select(m => new MensagemResponseDto
                    {
                        Id = m.Id,
                        ChamadoId = m.ChamadoId,
                        UsuarioId = m.UsuarioId,
                        NomeUsuario = m.Usuario.Nome,
                        TipoUsuario = m.Usuario.Tipo,
                        Mensagem = m.Mensagem,
                        DataEnvio = m.DataEnvio,
                        Anexos = m.Anexos.Select(a => new AnexoResponseDto
                        {
                            Id = a.Id,
                            NomeArquivo = a.NomeArquivo,
                            Url = $"{Request.Scheme}://{Request.Host}{a.CaminhoArquivo}",
                            Tipo = a.TipoArquivo
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                Console.WriteLine($"[API-CHAT] ✅ SUCCESS - Retornando resposta");
                return Ok(ApiResponse<MensagemResponseDto>.SuccessResult(
                    mensagemCompleta!,
                    "Mensagem enviada com sucesso"
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API-CHAT] ❌ EXCEPTION: {ex.GetType().Name}");
                Console.WriteLine($"[API-CHAT] ❌ Message: {ex.Message}");
                Console.WriteLine($"[API-CHAT] ❌ StackTrace: {ex.StackTrace}");
                _logger.LogError(ex, "Erro ao enviar mensagem no chamado {ChamadoId}", chamadoId);
                return StatusCode(500, ApiResponse<MensagemResponseDto>.ErrorResult($"Erro ao enviar mensagem: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtém novas mensagens de um chamado a partir de uma data
        /// </summary>
        /// <param name="chamadoId">ID do chamado</param>
        /// <param name="ultimaMensagemId">ID da última mensagem recebida</param>
        /// <returns>Lista de novas mensagens</returns>
        [HttpGet("{chamadoId}/mensagens/novas")]
        public async Task<ActionResult<ApiResponse<List<MensagemResponseDto>>>> GetNovasMensagens(
            int chamadoId,
            [FromQuery] int ultimaMensagemId = 0)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Verificar se o chamado existe e se o usuário tem acesso
                var chamado = await _context.Chamados
                    .FirstOrDefaultAsync(c => c.Id == chamadoId);

                if (chamado == null)
                {
                    return NotFound(ApiResponse<List<MensagemResponseDto>>.ErrorResult("Chamado não encontrado"));
                }

                // Verificar permissão
                if (userRole == "Cliente" && chamado.UsuarioId != userId)
                {
                    return Forbid();
                }

                // Buscar novas mensagens
                var novasMensagens = await _context.MensagensChamados
                    .Include(m => m.Usuario)
                    .Where(m => m.ChamadoId == chamadoId && m.Id > ultimaMensagemId)
                    .OrderBy(m => m.DataEnvio)
                    .Select(m => new MensagemResponseDto
                    {
                        Id = m.Id,
                        ChamadoId = m.ChamadoId,
                        UsuarioId = m.UsuarioId,
                        NomeUsuario = m.Usuario.Nome,
                        TipoUsuario = m.Usuario.Tipo,
                        Mensagem = m.Mensagem,
                        DataEnvio = m.DataEnvio
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<MensagemResponseDto>>.SuccessResult(novasMensagens));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar novas mensagens do chamado {ChamadoId}", chamadoId);
                return StatusCode(500, ApiResponse<List<MensagemResponseDto>>.ErrorResult("Erro ao buscar novas mensagens"));
            }
        }

        /// <summary>
        /// Suporte assume o atendimento de um chamado
        /// </summary>
        /// <param name="chamadoId">ID do chamado</param>
        /// <returns>Confirmação da operação</returns>
        [HttpPost("{chamadoId}/assumir")]
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<ActionResult<ApiResponse<object>>> AssumirChamado(int chamadoId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var chamado = await _context.Chamados.FindAsync(chamadoId);

                if (chamado == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Chamado não encontrado"));
                }

                if (chamado.Status == "Concluído")
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Não é possível assumir chamados concluídos"));
                }

                // Atualizar chamado
                chamado.SuporteResponsavelId = userId;
                chamado.Status = "Em Atendimento";

                // Criar mensagem automática
                var mensagem = new MensagemChamado
                {
                    ChamadoId = chamadoId,
                    UsuarioId = userId,
                    Mensagem = "Olá! Assumi o atendimento do seu chamado e estou analisando a situação.",
                    DataEnvio = DateTime.Now
                };

                _context.MensagensChamados.Add(mensagem);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResult(
                    new { chamadoId, suporteId = userId },
                    "Chamado assumido com sucesso"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao assumir chamado {ChamadoId}", chamadoId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("Erro ao assumir chamado"));
            }
        }

        /// <summary>
        /// Finaliza o atendimento de um chamado pelo chat
        /// </summary>
        /// <param name="chamadoId">ID do chamado</param>
        /// <returns>Confirmação da operação</returns>
        [HttpPost("{chamadoId}/finalizar")]
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<ActionResult<ApiResponse<object>>> FinalizarChamado(int chamadoId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var chamado = await _context.Chamados.FindAsync(chamadoId);

                if (chamado == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Chamado não encontrado"));
                }

                if (chamado.Status == "Concluído")
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Chamado já está concluído"));
                }

                // Atualizar chamado
                chamado.Status = "Concluído";

                // Criar mensagem de encerramento
                var mensagem = new MensagemChamado
                {
                    ChamadoId = chamadoId,
                    UsuarioId = userId,
                    Mensagem = "Chamado finalizado. Obrigado pelo contato!",
                    DataEnvio = DateTime.Now
                };

                _context.MensagensChamados.Add(mensagem);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResult(
                    new { chamadoId, status = "Concluído" },
                    "Chamado finalizado com sucesso"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar chamado {ChamadoId}", chamadoId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("Erro ao finalizar chamado"));
            }
        }

        /// <summary>
        /// Obtém o Content-Type baseado na extensão do arquivo
        /// </summary>
        private string GetContentType(string extensao)
        {
            return extensao.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                ".7z" => "application/x-7z-compressed",
                _ => "application/octet-stream"
            };
        }
    }
}
