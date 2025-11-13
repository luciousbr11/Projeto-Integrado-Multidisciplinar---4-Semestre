using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_API.Data;
using GestaoChamadosAI_API.DTOs.Chamados;
using GestaoChamadosAI_API.Helpers;
using GestaoChamadosAI_API.Models;
using GestaoChamadosAI_API.Services;

namespace GestaoChamadosAI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChamadosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAService _iaService;
        private readonly GeminiService _geminiService;
        private readonly AuthService _authService;
        private readonly ILogger<ChamadosController> _logger;

        public ChamadosController(
            AppDbContext context,
            IAService iaService,
            GeminiService geminiService,
            AuthService authService,
            ILogger<ChamadosController> logger)
        {
            _context = context;
            _iaService = iaService;
            _geminiService = geminiService;
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os chamados (com filtros e paginação)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ChamadoDto>>>> GetChamados(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] int? suporteId = null,
            [FromQuery] string? prioridade = null)
        {
            try
            {
                var userId = _authService.GetUserIdFromClaims(User);
                var userRole = _authService.GetUserRoleFromClaims(User);

                var query = _context.Chamados
                    .Include(c => c.Usuario)
                    .Include(c => c.SuporteResponsavel)
                    .AsQueryable();

                // Se for cliente, filtra apenas seus chamados
                if (userRole == "Cliente")
                {
                    query = query.Where(c => c.UsuarioId == userId);
                }

                // Aplica filtros
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(c => c.Status == status);
                }

                if (suporteId.HasValue)
                {
                    query = query.Where(c => c.SuporteResponsavelId == suporteId.Value);
                }

                if (!string.IsNullOrEmpty(prioridade))
                {
                    query = query.Where(c => c.Prioridade == prioridade);
                }

                var totalCount = await query.CountAsync();

                var chamados = await query
                    .OrderByDescending(c => c.DataAbertura)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new ChamadoDto
                    {
                        Id = c.Id,
                        Titulo = c.Titulo,
                        Descricao = c.Descricao,
                        DataAbertura = c.DataAbertura,
                        Status = c.Status,
                        CategoriaIA = c.CategoriaIA,
                        SugestaoIA = c.SugestaoIA,
                        Prioridade = c.Prioridade,
                        RespostaIA = c.RespostaIA,
                        FeedbackResolvido = c.FeedbackResolvido,
                        DataFeedback = c.DataFeedback,
                        UsuarioId = c.UsuarioId,
                        UsuarioNome = c.Usuario != null ? c.Usuario.Nome : "",
                        UsuarioEmail = c.Usuario != null ? c.Usuario.Email : "",
                        SuporteResponsavelId = c.SuporteResponsavelId,
                        SuporteNome = c.SuporteResponsavel != null ? c.SuporteResponsavel.Nome : null,
                        SuporteEmail = c.SuporteResponsavel != null ? c.SuporteResponsavel.Email : null,
                        TotalMensagens = c.Mensagens.Count
                    })
                    .ToListAsync();

                var pagedResult = new PagedResult<ChamadoDto>
                {
                    Items = chamados,
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = page
                };

                return Ok(ApiResponse<PagedResult<ChamadoDto>>.SuccessResponse(pagedResult));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar chamados");
                return StatusCode(500, ApiResponse<PagedResult<ChamadoDto>>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Obtém um chamado detalhado por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ChamadoDetalhadoDto>>> GetChamado(int id)
        {
            try
            {
                var userId = _authService.GetUserIdFromClaims(User);
                var userRole = _authService.GetUserRoleFromClaims(User);

                var chamado = await _context.Chamados
                    .Include(c => c.Usuario)
                    .Include(c => c.SuporteResponsavel)
                    .Include(c => c.Mensagens)
                        .ThenInclude(m => m.Usuario)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (chamado == null)
                {
                    return NotFound(ApiResponse<ChamadoDetalhadoDto>.ErrorResponse("Chamado não encontrado"));
                }

                // Verifica permissão (cliente só vê seus próprios chamados)
                if (userRole == "Cliente" && chamado.UsuarioId != userId)
                {
                    return Forbid();
                }

                var chamadoDto = new ChamadoDetalhadoDto
                {
                    Id = chamado.Id,
                    Titulo = chamado.Titulo,
                    Descricao = chamado.Descricao,
                    DataAbertura = chamado.DataAbertura,
                    Status = chamado.Status,
                    CategoriaIA = chamado.CategoriaIA,
                    SugestaoIA = chamado.SugestaoIA,
                    Prioridade = chamado.Prioridade,
                    RespostaIA = chamado.RespostaIA,
                    FeedbackResolvido = chamado.FeedbackResolvido,
                    DataFeedback = chamado.DataFeedback,
                    UsuarioId = chamado.UsuarioId,
                    UsuarioNome = chamado.Usuario?.Nome ?? "",
                    UsuarioEmail = chamado.Usuario?.Email ?? "",
                    SuporteResponsavelId = chamado.SuporteResponsavelId,
                    SuporteNome = chamado.SuporteResponsavel?.Nome,
                    SuporteEmail = chamado.SuporteResponsavel?.Email,
                    Mensagens = chamado.Mensagens.Select(m => new MensagemDto
                    {
                        Id = m.Id,
                        UsuarioId = m.UsuarioId,
                        UsuarioNome = m.Usuario?.Nome ?? "",
                        Mensagem = m.Mensagem,
                        DataEnvio = m.DataEnvio,
                        LidaPorCliente = m.LidaPorCliente,
                        LidaPorSuporte = m.LidaPorSuporte
                    }).OrderBy(m => m.DataEnvio).ToList()
                };

                return Ok(ApiResponse<ChamadoDetalhadoDto>.SuccessResponse(chamadoDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter chamado ID: {Id}", id);
                return StatusCode(500, ApiResponse<ChamadoDetalhadoDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Cria um novo chamado com análise automática de IA
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Cliente,Administrador")]
        public async Task<ActionResult<ApiResponse<ChamadoDto>>> CreateChamado([FromBody] CreateChamadoDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<ChamadoDto>.ErrorResponse("Dados inválidos", errors));
                }

                var userId = _authService.GetUserIdFromClaims(User);
                var userRole = _authService.GetUserRoleFromClaims(User);

                // Cliente não pode ter mais de um chamado em aberto
                if (userRole == "Cliente")
                {
                    var temChamadoAberto = await _context.Chamados
                        .AnyAsync(c => c.UsuarioId == userId && 
                                      (c.Status == "Aberto" || c.Status == "Em Andamento"));

                    if (temChamadoAberto)
                    {
                        return BadRequest(ApiResponse<ChamadoDto>.ErrorResponse(
                            "Você já possui um chamado em aberto. Finalize-o antes de abrir outro."));
                    }
                }

                // Cria o chamado
                var chamado = new Chamado
                {
                    Titulo = createDto.Titulo,
                    Descricao = createDto.Descricao,
                    UsuarioId = userId,
                    DataAbertura = DateTime.Now,
                    Status = "Aberto"
                };

                // Análise com IA legada
                chamado.SugestaoIA = _iaService.AnalisarChamado(chamado.Titulo, chamado.Descricao);

                // Análise com Google Gemini AI
                try
                {
                    chamado.CategoriaIA = await _geminiService.CategorizarChamadoAsync(chamado.Titulo, chamado.Descricao);
                    chamado.Prioridade = await _geminiService.AnalisarPrioridadeAsync(chamado.Titulo, chamado.Descricao);
                    chamado.RespostaIA = await _geminiService.GerarRespostaAsync(
                        chamado.Titulo,
                        chamado.Descricao,
                        chamado.CategoriaIA ?? "Outros"
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Erro ao usar Gemini AI, usando fallback");
                    chamado.CategoriaIA = IdentificarCategoriaFallback(chamado.Titulo, chamado.Descricao);
                    chamado.Prioridade = _iaService.ClassificarPrioridade(chamado.Titulo, chamado.Descricao);
                    chamado.RespostaIA = "No momento, não foi possível gerar uma resposta automática. Um especialista irá atendê-lo em breve.";
                }

                _context.Chamados.Add(chamado);
                await _context.SaveChangesAsync();

                // Cria mensagem automática da IA com a resposta
                if (!string.IsNullOrEmpty(chamado.RespostaIA))
                {
                    // Busca um usuário admin ou suporte para ser o "remetente" da mensagem da IA
                    var usuarioSistema = await _context.Usuarios
                        .Where(u => u.Tipo == "Administrador" || u.Tipo == "Suporte")
                        .OrderBy(u => u.Id)
                        .FirstOrDefaultAsync();

                    // Se não encontrar admin/suporte, usa o próprio cliente (fallback)
                    var remetenteId = usuarioSistema?.Id ?? userId;

                    var mensagemIA = new MensagemChamado
                    {
                        ChamadoId = chamado.Id,
                        UsuarioId = remetenteId,
                        Mensagem = $"🤖 **Resposta Automática da IA:**\n\n{chamado.RespostaIA}\n\n**Isso resolveu seu problema?**",
                        DataEnvio = DateTime.Now,
                        LidaPorCliente = false,
                        LidaPorSuporte = true
                    };
                    _context.MensagensChamados.Add(mensagemIA);
                    await _context.SaveChangesAsync();
                }

                // Retorna chamado criado
                var chamadoDto = new ChamadoDto
                {
                    Id = chamado.Id,
                    Titulo = chamado.Titulo,
                    Descricao = chamado.Descricao,
                    DataAbertura = chamado.DataAbertura,
                    Status = chamado.Status,
                    CategoriaIA = chamado.CategoriaIA,
                    SugestaoIA = chamado.SugestaoIA,
                    Prioridade = chamado.Prioridade,
                    RespostaIA = chamado.RespostaIA,
                    UsuarioId = chamado.UsuarioId,
                    TotalMensagens = 0
                };

                _logger.LogInformation("Chamado criado ID: {Id} por usuário {UserId}", chamado.Id, userId);

                return CreatedAtAction(nameof(GetChamado), new { id = chamado.Id },
                    ApiResponse<ChamadoDto>.SuccessResponse(chamadoDto, "Chamado criado com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar chamado");
                return StatusCode(500, ApiResponse<ChamadoDto>.ErrorResponse($"Erro interno no servidor: {ex.Message}"));
            }
        }

        private string IdentificarCategoriaFallback(string titulo, string descricao)
        {
            string texto = (titulo + " " + descricao).ToLower();

            if (texto.Contains("senha") || texto.Contains("login") || texto.Contains("acesso"))
                return "Problemas de Acesso";
            if (texto.Contains("lento") || texto.Contains("performance"))
                return "Problemas de Performance";
            if (texto.Contains("erro") || texto.Contains("bug"))
                return "Erros Gerais";
            if (texto.Contains("impressora"))
                return "Problemas de Impressão";
            if (texto.Contains("email") || texto.Contains("e-mail"))
                return "Problemas de E-mail";
            if (texto.Contains("internet") || texto.Contains("rede") || texto.Contains("wifi"))
                return "Problemas de Rede";
            if (texto.Contains("instala") || texto.Contains("software"))
                return "Instalação de Software";

            return "Outros";
        }

        /// <summary>
        /// Atualiza um chamado existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<ActionResult<ApiResponse<ChamadoDto>>> UpdateChamado(int id, [FromBody] UpdateChamadoDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<ChamadoDto>.ErrorResponse("Dados inválidos", errors));
                }

                var chamado = await _context.Chamados.FindAsync(id);
                if (chamado == null)
                {
                    return NotFound(ApiResponse<ChamadoDto>.ErrorResponse("Chamado não encontrado"));
                }

                // Valida se pode alterar status sem ter assumido
                if (chamado.Status != updateDto.Status && chamado.SuporteResponsavelId == null)
                {
                    return BadRequest(ApiResponse<ChamadoDto>.ErrorResponse(
                        "Não é possível alterar o status do chamado sem antes assumir o atendimento"));
                }

                chamado.Titulo = updateDto.Titulo;
                chamado.Descricao = updateDto.Descricao;
                chamado.Status = updateDto.Status;

                await _context.SaveChangesAsync();

                var chamadoDto = new ChamadoDto
                {
                    Id = chamado.Id,
                    Titulo = chamado.Titulo,
                    Descricao = chamado.Descricao,
                    DataAbertura = chamado.DataAbertura,
                    Status = chamado.Status,
                    CategoriaIA = chamado.CategoriaIA,
                    Prioridade = chamado.Prioridade,
                    UsuarioId = chamado.UsuarioId
                };

                _logger.LogInformation("Chamado atualizado ID: {Id}", id);

                return Ok(ApiResponse<ChamadoDto>.SuccessResponse(chamadoDto, "Chamado atualizado com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar chamado ID: {Id}", id);
                return StatusCode(500, ApiResponse<ChamadoDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Exclui um chamado
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteChamado(int id)
        {
            try
            {
                var chamado = await _context.Chamados.FindAsync(id);
                if (chamado == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Chamado não encontrado"));
                }

                _context.Chamados.Remove(chamado);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Chamado excluído ID: {Id}", id);

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { id = id },
                    "Chamado excluído com sucesso"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir chamado ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Registra feedback do cliente sobre a resposta da IA
        /// </summary>
        [HttpPost("{id}/feedback")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<ApiResponse<ChamadoDto>>> ProcessarFeedback(int id, [FromBody] FeedbackChamadoDto feedbackDto)
        {
            try
            {
                var userId = _authService.GetUserIdFromClaims(User);
                var chamado = await _context.Chamados.FindAsync(id);

                if (chamado == null)
                {
                    return NotFound(ApiResponse<ChamadoDto>.ErrorResponse("Chamado não encontrado"));
                }

                // Verifica se o chamado pertence ao cliente
                if (chamado.UsuarioId != userId)
                {
                    return Forbid();
                }

                chamado.FeedbackResolvido = feedbackDto.Resolvido;
                chamado.DataFeedback = DateTime.Now;
                chamado.Status = feedbackDto.Resolvido ? "Solucionado por IA" : "Aberto";

                await _context.SaveChangesAsync();

                var mensagem = feedbackDto.Resolvido
                    ? "Ótimo! Ficamos felizes que a IA pôde resolver seu problema! 🎉"
                    : "Entendido! Um especialista irá atender seu chamado em breve. 👨‍💻";

                var chamadoDto = new ChamadoDto
                {
                    Id = chamado.Id,
                    Titulo = chamado.Titulo,
                    Status = chamado.Status,
                    FeedbackResolvido = chamado.FeedbackResolvido,
                    DataFeedback = chamado.DataFeedback
                };

                _logger.LogInformation("Feedback registrado para chamado ID: {Id} - Resolvido: {Resolvido}", id, feedbackDto.Resolvido);

                return Ok(ApiResponse<ChamadoDto>.SuccessResponse(chamadoDto, mensagem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar feedback do chamado ID: {Id}", id);
                return StatusCode(500, ApiResponse<ChamadoDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Gera/regenera resposta automática da IA para um chamado
        /// </summary>
        [HttpPost("{id}/gerar-resposta-ia")]
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<ActionResult<ApiResponse<object>>> GerarRespostaIA(int id)
        {
            try
            {
                var chamado = await _context.Chamados.FindAsync(id);
                if (chamado == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Chamado não encontrado"));
                }

                chamado.RespostaIA = await _geminiService.GerarRespostaAsync(
                    chamado.Titulo,
                    chamado.Descricao,
                    chamado.CategoriaIA ?? "Outros"
                );

                await _context.SaveChangesAsync();

                _logger.LogInformation("Resposta IA gerada para chamado ID: {Id}", id);

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { respostaIA = chamado.RespostaIA },
                    "Resposta gerada com sucesso pela IA"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar resposta IA para chamado ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"Erro ao gerar resposta: {ex.Message}"));
            }
        }

        /// <summary>
        /// Transfere um chamado para outro suporte
        /// </summary>
        [HttpPost("{id}/transferir")]
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<ActionResult<ApiResponse<ChamadoDto>>> TransferirChamado(int id, [FromBody] TransferirChamadoDto transferDto)
        {
            try
            {
                var chamado = await _context.Chamados
                    .Include(c => c.SuporteResponsavel)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (chamado == null)
                {
                    return NotFound(ApiResponse<ChamadoDto>.ErrorResponse("Chamado não encontrado"));
                }

                // Não permite transferir chamados finalizados
                if (chamado.Status == "Concluído" || chamado.Status == "Solucionado por IA")
                {
                    return BadRequest(ApiResponse<ChamadoDto>.ErrorResponse(
                        "Não é possível transferir um chamado já finalizado"));
                }

                var novoSuporte = await _context.Usuarios.FindAsync(transferDto.NovoSuporteId);
                if (novoSuporte == null || (novoSuporte.Tipo != "Suporte" && novoSuporte.Tipo != "Administrador"))
                {
                    return BadRequest(ApiResponse<ChamadoDto>.ErrorResponse("Suporte inválido"));
                }

                var suporteAnterior = chamado.SuporteResponsavel?.Nome ?? "Sistema";

                chamado.SuporteResponsavelId = transferDto.NovoSuporteId;
                chamado.Status = "Aberto";

                // Cria mensagem de sistema
                var mensagem = new MensagemChamado
                {
                    ChamadoId = id,
                    UsuarioId = transferDto.NovoSuporteId,
                    Mensagem = $"🔄 Atendimento transferido de {suporteAnterior} para {novoSuporte.Nome}. O chamado está aguardando ser assumido.",
                    DataEnvio = DateTime.Now,
                    LidaPorCliente = false,
                    LidaPorSuporte = true
                };

                _context.MensagensChamados.Add(mensagem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Chamado ID: {Id} transferido para suporte {SuporteId}", id, transferDto.NovoSuporteId);

                return Ok(ApiResponse<ChamadoDto>.SuccessResponse(
                    new ChamadoDto { Id = chamado.Id, Status = chamado.Status },
                    $"Chamado transferido com sucesso para {novoSuporte.Nome}"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao transferir chamado ID: {Id}", id);
                return StatusCode(500, ApiResponse<ChamadoDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Reassume um chamado que foi transferido
        /// </summary>
        [HttpPost("{id}/reassumir")]
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<ActionResult<ApiResponse<ChamadoDto>>> Reassumir(int id)
        {
            try
            {
                var userId = _authService.GetUserIdFromClaims(User);
                var chamado = await _context.Chamados
                    .Include(c => c.SuporteResponsavel)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (chamado == null)
                {
                    return NotFound(ApiResponse<ChamadoDto>.ErrorResponse("Chamado não encontrado"));
                }

                var usuario = await _context.Usuarios.FindAsync(userId);
                var suporteAtual = chamado.SuporteResponsavel?.Nome ?? "Sistema";

                chamado.SuporteResponsavelId = userId;

                // Cria mensagem de sistema
                var mensagem = new MensagemChamado
                {
                    ChamadoId = id,
                    UsuarioId = userId,
                    Mensagem = $"🔁 Atendimento reassumido por {usuario?.Nome} (anteriormente com {suporteAtual}).",
                    DataEnvio = DateTime.Now,
                    LidaPorCliente = false,
                    LidaPorSuporte = true
                };

                _context.MensagensChamados.Add(mensagem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Chamado ID: {Id} reassumido por usuário {UserId}", id, userId);

                return Ok(ApiResponse<ChamadoDto>.SuccessResponse(
                    new ChamadoDto { Id = chamado.Id },
                    "Você reassumiu este atendimento com sucesso"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reassumir chamado ID: {Id}", id);
                return StatusCode(500, ApiResponse<ChamadoDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Finaliza um chamado (apenas clientes podem finalizar seus próprios chamados)
        /// </summary>
        [HttpPost("{id}/finalizar")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<ApiResponse<ChamadoDto>>> FinalizarChamado(int id)
        {
            try
            {
                var userId = _authService.GetUserIdFromClaims(User);
                var chamado = await _context.Chamados.FindAsync(id);

                if (chamado == null)
                {
                    return NotFound(ApiResponse<ChamadoDto>.ErrorResponse("Chamado não encontrado"));
                }

                if (chamado.UsuarioId != userId)
                {
                    return Forbid();
                }

                if (chamado.Status != "Aberto" && chamado.Status != "Em Andamento")
                {
                    return BadRequest(ApiResponse<ChamadoDto>.ErrorResponse("Este chamado não pode ser finalizado"));
                }

                chamado.Status = "Concluído";
                await _context.SaveChangesAsync();

                _logger.LogInformation("Chamado ID: {Id} finalizado pelo cliente", id);

                return Ok(ApiResponse<ChamadoDto>.SuccessResponse(
                    new ChamadoDto { Id = chamado.Id, Status = chamado.Status },
                    "Chamado finalizado com sucesso"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar chamado ID: {Id}", id);
                return StatusCode(500, ApiResponse<ChamadoDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Lista chamados do usuário logado
        /// </summary>
        [HttpGet("meus")]
        public async Task<ActionResult<ApiResponse<List<ChamadoDto>>>> GetMeusChamados()
        {
            try
            {
                var userId = _authService.GetUserIdFromClaims(User);

                var chamados = await _context.Chamados
                    .Include(c => c.Usuario)
                    .Include(c => c.SuporteResponsavel)
                    .Where(c => c.UsuarioId == userId)
                    .OrderByDescending(c => c.DataAbertura)
                    .Select(c => new ChamadoDto
                    {
                        Id = c.Id,
                        Titulo = c.Titulo,
                        Descricao = c.Descricao,
                        DataAbertura = c.DataAbertura,
                        Status = c.Status,
                        CategoriaIA = c.CategoriaIA,
                        Prioridade = c.Prioridade,
                        RespostaIA = c.RespostaIA,
                        FeedbackResolvido = c.FeedbackResolvido,
                        SuporteNome = c.SuporteResponsavel != null ? c.SuporteResponsavel.Nome : null,
                        TotalMensagens = c.Mensagens.Count
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<ChamadoDto>>.SuccessResponse(chamados));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar meus chamados");
                return StatusCode(500, ApiResponse<List<ChamadoDto>>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Obtém sugestão da IA em tempo real (para mostrar enquanto digita)
        /// </summary>
        [HttpPost("sugestao-ia")]
        public ActionResult<ApiResponse<object>> ObterSugestaoIA([FromBody] CreateChamadoDto dto)
        {
            try
            {
                var sugestao = _iaService.AnalisarChamado(dto.Titulo, dto.Descricao);
                var prioridade = _iaService.ClassificarPrioridade(dto.Titulo, dto.Descricao);

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { sugestao, prioridade }
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter sugestão IA");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
            }
        }
    }
}
