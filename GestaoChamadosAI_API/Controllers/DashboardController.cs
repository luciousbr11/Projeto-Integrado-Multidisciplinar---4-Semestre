using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_API.Data;
using GestaoChamadosAI_API.Helpers;
using System.Security.Claims;

namespace GestaoChamadosAI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(AppDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtém estatísticas gerais do dashboard baseado no perfil do usuário
        /// </summary>
        /// <returns>Estatísticas personalizadas por perfil</returns>
        [HttpGet("estatisticas")]
        public async Task<ActionResult<ApiResponse<object>>> GetEstatisticas()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                object estatisticas;

                switch (userRole)
                {
                    case "Administrador":
                        estatisticas = await GetEstatisticasAdministrador();
                        break;

                    case "Suporte":
                        estatisticas = await GetEstatisticasSuporte(userId);
                        break;

                    case "Cliente":
                        estatisticas = await GetEstatisticasCliente(userId);
                        break;

                    default:
                        return BadRequest(ApiResponse<object>.ErrorResult("Tipo de usuário inválido"));
                }

                return Ok(ApiResponse<object>.SuccessResult(estatisticas));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas do dashboard");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Erro ao buscar estatísticas"));
            }
        }

        /// <summary>
        /// Obtém lista de chamados do usuário logado (todos os perfis)
        /// </summary>
        /// <returns>Lista de chamados conforme perfil</returns>
        [HttpGet("meus-chamados")]
        public async Task<ActionResult<ApiResponse<object>>> GetMeusChamados()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                object chamados;

                if (userRole == "Cliente")
                {
                    chamados = await _context.Chamados
                        .Include(c => c.SuporteResponsavel)
                        .Where(c => c.UsuarioId == userId)
                        .OrderByDescending(c => c.DataAbertura)
                        .Take(10)
                        .Select(c => new
                        {
                            c.Id,
                            c.Titulo,
                            c.Status,
                            c.Prioridade,
                            c.DataAbertura,
                            SuporteResponsavel = c.SuporteResponsavel != null ? c.SuporteResponsavel.Nome : "Não atribuído"
                        })
                        .ToListAsync();
                }
                else if (userRole == "Suporte")
                {
                    chamados = await _context.Chamados
                        .Include(c => c.Usuario)
                        .Where(c => c.SuporteResponsavelId == userId)
                        .OrderByDescending(c => c.DataAbertura)
                        .Take(10)
                        .Select(c => new
                        {
                            c.Id,
                            c.Titulo,
                            c.Status,
                            c.Prioridade,
                            c.DataAbertura,
                            Cliente = c.Usuario!.Nome
                        })
                        .ToListAsync();
                }
                else if (userRole == "Administrador")
                {
                    chamados = await _context.Chamados
                        .Include(c => c.Usuario)
                        .Include(c => c.SuporteResponsavel)
                        .OrderByDescending(c => c.DataAbertura)
                        .Take(10)
                        .Select(c => new
                        {
                            c.Id,
                            c.Titulo,
                            c.Status,
                            c.Prioridade,
                            c.DataAbertura,
                            Cliente = c.Usuario!.Nome,
                            SuporteResponsavel = c.SuporteResponsavel != null ? c.SuporteResponsavel.Nome : "Não atribuído"
                        })
                        .ToListAsync();
                }
                else
                {
                    chamados = new List<object>();
                }

                return Ok(ApiResponse<object>.SuccessResult(chamados));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar chamados do usuário");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Erro ao buscar chamados"));
            }
        }

        /// <summary>
        /// Obtém chamados que precisam de atenção do suporte
        /// </summary>
        /// <returns>Lista de chamados aguardando atendimento</returns>
        [HttpGet("chamados-aguardando")]
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<ActionResult<ApiResponse<object>>> GetChamadosAguardando()
        {
            try
            {
                var chamadosAbertos = await _context.Chamados
                    .Include(c => c.Usuario)
                    .Where(c => c.Status == "Aberto" && c.SuporteResponsavelId == null)
                    .OrderByDescending(c => c.Prioridade == "Alta")
                    .ThenByDescending(c => c.Prioridade == "Média")
                    .ThenBy(c => c.DataAbertura)
                    .Take(20)
                    .Select(c => new
                    {
                        c.Id,
                        c.Titulo,
                        c.Descricao,
                        c.Status,
                        c.Prioridade,
                        c.CategoriaIA,
                        c.DataAbertura,
                        Cliente = c.Usuario.Nome,
                        ClienteEmail = c.Usuario.Email,
                        TempoAguardando = DateTime.Now.Subtract(c.DataAbertura).TotalHours
                    })
                    .ToListAsync();

                return Ok(ApiResponse<object>.SuccessResult(new
                {
                    total = chamadosAbertos.Count,
                    chamados = chamadosAbertos
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar chamados aguardando");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Erro ao buscar chamados"));
            }
        }

        /// <summary>
        /// Obtém performance dos suportes (apenas Admin)
        /// </summary>
        /// <returns>Estatísticas de cada suporte</returns>
        [HttpGet("performance-suportes")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<object>>> GetPerformanceSuportes()
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
                        ChamadosAtivos = _context.Chamados.Count(c => 
                            c.SuporteResponsavelId == u.Id && 
                            c.Status != "Concluído"),
                        ChamadosFinalizados = _context.Chamados.Count(c => 
                            c.SuporteResponsavelId == u.Id && 
                            c.Status == "Concluído"),
                        TotalChamados = _context.Chamados.Count(c => 
                            c.SuporteResponsavelId == u.Id)
                    })
                    .ToListAsync();

                return Ok(ApiResponse<object>.SuccessResult(suportes));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar performance dos suportes");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Erro ao buscar performance"));
            }
        }

        #region Métodos Privados

        private async Task<object> GetEstatisticasAdministrador()
        {
            var totalUsuarios = await _context.Usuarios.CountAsync();
            var totalChamados = await _context.Chamados.CountAsync();
            var chamadosAbertos = await _context.Chamados.CountAsync(c => c.Status == "Aberto");
            var chamadosEmAtendimento = await _context.Chamados.CountAsync(c => c.Status == "Em Atendimento");
            var chamadosAguardandoCliente = await _context.Chamados.CountAsync(c => c.Status == "Aguardando Cliente");
            
            // Chamados concluídos incluem: Concluído e Solucionado por IA
            var chamadosFechados = await _context.Chamados.CountAsync(c => 
                c.Status == "Concluído" || c.Status == "Solucionado por IA");

            var chamadosPorPrioridade = await _context.Chamados
                .Where(c => c.Status != "Concluído" && c.Status != "Solucionado por IA")
                .GroupBy(c => c.Prioridade)
                .Select(g => new { prioridade = g.Key, total = g.Count() })
                .ToListAsync();

            var chamadosPorCategoria = await _context.Chamados
                .GroupBy(c => c.CategoriaIA)
                .Select(g => new { categoria = g.Key, total = g.Count() })
                .OrderByDescending(x => x.total)
                .Take(5)
                .ToListAsync();

            return new
            {
                usuarios = new
                {
                    total = totalUsuarios,
                    clientes = await _context.Usuarios.CountAsync(u => u.Tipo == "Cliente"),
                    suportes = await _context.Usuarios.CountAsync(u => u.Tipo == "Suporte"),
                    administradores = await _context.Usuarios.CountAsync(u => u.Tipo == "Administrador")
                },
                chamados = new
                {
                    total = totalChamados,
                    abertos = chamadosAbertos,
                    emAtendimento = chamadosEmAtendimento,
                    aguardandoCliente = chamadosAguardandoCliente,
                    fechados = chamadosFechados,
                    porPrioridade = chamadosPorPrioridade,
                    porCategoria = chamadosPorCategoria
                }
            };
        }

        private async Task<object> GetEstatisticasSuporte(int userId)
        {
            // Chamados fechados incluem: Fechado, Solucionado por IA e Concluido
            var meusChamadosAbertos = await _context.Chamados
                .CountAsync(c => c.SuporteResponsavelId == userId && 
                    c.Status != "Concluído" && c.Status != "Solucionado por IA");

            var meusChamadosFechados = await _context.Chamados
                .CountAsync(c => c.SuporteResponsavelId == userId && 
                    (c.Status == "Concluído" || c.Status == "Solucionado por IA"));

            var chamadosDisponiveis = await _context.Chamados
                .CountAsync(c => c.Status == "Aberto" && c.SuporteResponsavelId == null);

            var chamadosPorStatus = await _context.Chamados
                .Where(c => c.SuporteResponsavelId == userId)
                .GroupBy(c => c.Status)
                .Select(g => new { status = g.Key, total = g.Count() })
                .ToListAsync();

            var chamadosPorPrioridade = await _context.Chamados
                .Where(c => c.SuporteResponsavelId == userId && 
                    c.Status != "Fechado" && c.Status != "Solucionado por IA" && c.Status != "Concluido")
                .GroupBy(c => c.Prioridade)
                .Select(g => new { prioridade = g.Key, total = g.Count() })
                .ToListAsync();

            return new
            {
                meusChamados = new
                {
                    ativos = meusChamadosAbertos,
                    finalizados = meusChamadosFechados,
                    total = meusChamadosAbertos + meusChamadosFechados,
                    porStatus = chamadosPorStatus,
                    porPrioridade = chamadosPorPrioridade
                },
                disponivel = new
                {
                    chamadosAguardando = chamadosDisponiveis
                }
            };
        }

        private async Task<object> GetEstatisticasCliente(int userId)
        {
            var totalChamados = await _context.Chamados.CountAsync(c => c.UsuarioId == userId);
            var chamadosAbertos = await _context.Chamados.CountAsync(c => c.UsuarioId == userId && c.Status == "Aberto");
            var chamadosEmAtendimento = await _context.Chamados.CountAsync(c => c.UsuarioId == userId && c.Status == "Em Atendimento");
            var chamadosAguardandoCliente = await _context.Chamados.CountAsync(c => c.UsuarioId == userId && c.Status == "Aguardando Cliente");
            
            // Chamados concluídos incluem: Concluído e Solucionado por IA
            var chamadosFechados = await _context.Chamados.CountAsync(c => 
                c.UsuarioId == userId && 
                (c.Status == "Concluído" || c.Status == "Solucionado por IA"));

            var chamadosPorCategoria = await _context.Chamados
                .Where(c => c.UsuarioId == userId)
                .GroupBy(c => c.CategoriaIA)
                .Select(g => new { categoria = g.Key, total = g.Count() })
                .OrderByDescending(x => x.total)
                .ToListAsync();

            var ultimosChamados = await _context.Chamados
                .Where(c => c.UsuarioId == userId)
                .OrderByDescending(c => c.DataAbertura)
                .Take(5)
                .Select(c => new
                {
                    c.Id,
                    c.Titulo,
                    c.Status,
                    c.DataAbertura
                })
                .ToListAsync();

            return new
            {
                meusChamados = new
                {
                    total = totalChamados,
                    abertos = chamadosAbertos,
                    emAtendimento = chamadosEmAtendimento,
                    aguardandoMinhaResposta = chamadosAguardandoCliente,
                    fechados = chamadosFechados,
                    porCategoria = chamadosPorCategoria
                },
                ultimosChamados = ultimosChamados
            };
        }

        #endregion
    }
}
