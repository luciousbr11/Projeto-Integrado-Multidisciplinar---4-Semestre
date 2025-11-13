using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_API.Data;
using GestaoChamadosAI_API.DTOs.Usuarios;
using GestaoChamadosAI_API.Helpers;
using GestaoChamadosAI_API.Models;
using GestaoChamadosAI_API.Services;

namespace GestaoChamadosAI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHashService _passwordService;
        private readonly AuthService _authService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            AppDbContext context,
            PasswordHashService passwordService,
            AuthService authService,
            ILogger<UsuariosController> logger)
        {
            _context = context;
            _passwordService = passwordService;
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os usuários (com paginação)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrador,Suporte")]
        public async Task<ActionResult<ApiResponse<PagedResult<UsuarioDto>>>> GetUsuarios(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? tipo = null)
        {
            try
            {
                var query = _context.Usuarios.AsQueryable();

                // Filtro por tipo
                if (!string.IsNullOrEmpty(tipo))
                {
                    query = query.Where(u => u.Tipo == tipo);
                }

                var totalCount = await query.CountAsync();

                var usuarios = await query
                    .OrderBy(u => u.Nome)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UsuarioDto
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        Email = u.Email,
                        Tipo = u.Tipo,
                        DataCadastro = u.DataCadastro,
                        TotalChamados = u.Chamados != null ? u.Chamados.Count : 0
                    })
                    .ToListAsync();

                var pagedResult = new PagedResult<UsuarioDto>
                {
                    Items = usuarios,
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = page
                };

                return Ok(ApiResponse<PagedResult<UsuarioDto>>.SuccessResponse(pagedResult));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários");
                return StatusCode(500, ApiResponse<PagedResult<UsuarioDto>>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Obtém um usuário por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Suporte")]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Where(u => u.Id == id)
                    .Select(u => new UsuarioDto
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        Email = u.Email,
                        Tipo = u.Tipo,
                        DataCadastro = u.DataCadastro,
                        TotalChamados = u.Chamados != null ? u.Chamados.Count : 0
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound(ApiResponse<UsuarioDto>.ErrorResponse("Usuário não encontrado"));
                }

                return Ok(ApiResponse<UsuarioDto>.SuccessResponse(usuario));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário ID: {Id}", id);
                return StatusCode(500, ApiResponse<UsuarioDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> CreateUsuario([FromBody] CreateUsuarioDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<UsuarioDto>.ErrorResponse("Dados inválidos", errors));
                }

                // Verifica se email já existe
                var emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == createDto.Email);
                if (emailExiste)
                {
                    return BadRequest(ApiResponse<UsuarioDto>.ErrorResponse("E-mail já cadastrado"));
                }

                // Cria novo usuário com senha hasheada
                var usuario = new Usuario
                {
                    Nome = createDto.Nome,
                    Email = createDto.Email,
                    Senha = _passwordService.HashPassword(createDto.Senha),
                    Tipo = createDto.Tipo,
                    DataCadastro = DateTime.Now
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var usuarioDto = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Tipo = usuario.Tipo,
                    DataCadastro = usuario.DataCadastro,
                    TotalChamados = 0
                };

                _logger.LogInformation("Usuário criado: {Email}", usuario.Email);

                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id },
                    ApiResponse<UsuarioDto>.SuccessResponse(usuarioDto, "Usuário criado com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário");
                return StatusCode(500, ApiResponse<UsuarioDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> UpdateUsuario(int id, [FromBody] UpdateUsuarioDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<UsuarioDto>.ErrorResponse("Dados inválidos", errors));
                }

                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound(ApiResponse<UsuarioDto>.ErrorResponse("Usuário não encontrado"));
                }

                // Verifica se email já existe em outro usuário
                var emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == updateDto.Email && u.Id != id);
                if (emailExiste)
                {
                    return BadRequest(ApiResponse<UsuarioDto>.ErrorResponse("E-mail já cadastrado para outro usuário"));
                }

                // Atualiza dados
                usuario.Nome = updateDto.Nome;
                usuario.Email = updateDto.Email;
                usuario.Tipo = updateDto.Tipo;

                // Atualiza senha apenas se fornecida
                if (!string.IsNullOrEmpty(updateDto.Senha))
                {
                    usuario.Senha = _passwordService.HashPassword(updateDto.Senha);
                }

                await _context.SaveChangesAsync();

                var usuarioDto = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Tipo = usuario.Tipo,
                    DataCadastro = usuario.DataCadastro,
                    TotalChamados = usuario.Chamados?.Count ?? 0
                };

                _logger.LogInformation("Usuário atualizado: {Email}", usuario.Email);

                return Ok(ApiResponse<UsuarioDto>.SuccessResponse(usuarioDto, "Usuário atualizado com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário ID: {Id}", id);
                return StatusCode(500, ApiResponse<UsuarioDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Exclui um usuário
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Chamados)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (usuario == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Usuário não encontrado"));
                }

                // Remove chamados do usuário
                if (usuario.Chamados != null && usuario.Chamados.Any())
                {
                    _context.Chamados.RemoveRange(usuario.Chamados);
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário excluído: {Email}", usuario.Email);

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { id = id },
                    "Usuário excluído com sucesso"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Lista usuários por tipo
        /// </summary>
        [HttpGet("tipo/{tipo}")]
        [Authorize(Roles = "Administrador,Suporte")]
        public async Task<ActionResult<ApiResponse<List<UsuarioDto>>>> GetUsuariosPorTipo(string tipo)
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .Where(u => u.Tipo == tipo)
                    .OrderBy(u => u.Nome)
                    .Select(u => new UsuarioDto
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        Email = u.Email,
                        Tipo = u.Tipo,
                        DataCadastro = u.DataCadastro,
                        TotalChamados = u.Chamados != null ? u.Chamados.Count : 0
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<UsuarioDto>>.SuccessResponse(usuarios));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários do tipo: {Tipo}", tipo);
                return StatusCode(500, ApiResponse<List<UsuarioDto>>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Lista todos os suportes (Suporte + Administrador)
        /// </summary>
        [HttpGet("suportes")]
        [Authorize(Roles = "Administrador,Suporte")]
        public async Task<ActionResult<ApiResponse<List<UsuarioDto>>>> GetSuPortes()
        {
            try
            {
                var suportes = await _context.Usuarios
                    .Where(u => u.Tipo == "Suporte" || u.Tipo == "Administrador")
                    .OrderBy(u => u.Nome)
                    .Select(u => new UsuarioDto
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        Email = u.Email,
                        Tipo = u.Tipo,
                        DataCadastro = u.DataCadastro,
                        TotalChamados = u.Chamados != null ? u.Chamados.Count : 0
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<UsuarioDto>>.SuccessResponse(suportes));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar suportes");
                return StatusCode(500, ApiResponse<List<UsuarioDto>>.ErrorResponse("Erro interno no servidor"));
            }
        }
    }
}
