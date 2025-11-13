using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_API.Data;
using GestaoChamadosAI_API.DTOs.Auth;
using GestaoChamadosAI_API.Helpers;
using GestaoChamadosAI_API.Services;

namespace GestaoChamadosAI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly PasswordHashService _passwordService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AppDbContext context,
            AuthService authService,
            PasswordHashService passwordService,
            ILogger<AuthController> logger)
        {
            _context = context;
            _authService = authService;
            _passwordService = passwordService;
            _logger = logger;
        }

        /// <summary>
        /// Realiza login e retorna token JWT
        /// </summary>
        /// <param name="loginDto">Credenciais de login</param>
        /// <returns>Token JWT e dados do usuário</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<LoginResponseDto>.ErrorResponse("Dados inválidos", errors));
                }

                // Busca usuário por email
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (usuario == null)
                {
                    _logger.LogWarning("Tentativa de login com email inexistente: {Email}", loginDto.Email);
                    return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse("E-mail ou senha inválidos"));
                }

                // Verifica senha (aceita tanto BCrypt quanto texto simples para compatibilidade)
                bool senhaValida = false;
                
                // Tenta verificar com BCrypt primeiro
                if (usuario.Senha.StartsWith("$2a$") || usuario.Senha.StartsWith("$2b$") || usuario.Senha.StartsWith("$2y$"))
                {
                    senhaValida = _passwordService.VerifyPassword(loginDto.Senha, usuario.Senha);
                }
                else
                {
                    // Se não for BCrypt, compara texto simples (sistema legado)
                    senhaValida = usuario.Senha == loginDto.Senha;
                }

                if (!senhaValida)
                {
                    _logger.LogWarning("Tentativa de login com senha incorreta para: {Email}", loginDto.Email);
                    return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse("E-mail ou senha inválidos"));
                }

                // Gera tokens
                var token = _authService.GenerateJwtToken(usuario);
                var refreshToken = _authService.GenerateRefreshToken();

                var response = new LoginResponseDto
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    Usuario = new UsuarioAuthDto
                    {
                        Id = usuario.Id,
                        Nome = usuario.Nome,
                        Email = usuario.Email,
                        Tipo = usuario.Tipo,
                        DataCadastro = usuario.DataCadastro
                    }
                };

                _logger.LogInformation("Login bem-sucedido para: {Email}", loginDto.Email);

                return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login realizado com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar login");
                return StatusCode(500, ApiResponse<LoginResponseDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Obtém dados do usuário autenticado
        /// </summary>
        /// <returns>Dados do usuário logado</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UsuarioAuthDto>>> GetProfile()
        {
            try
            {
                var userId = _authService.GetUserIdFromClaims(User);
                
                if (userId == 0)
                {
                    return Unauthorized(ApiResponse<UsuarioAuthDto>.ErrorResponse("Token inválido"));
                }

                var usuario = await _context.Usuarios.FindAsync(userId);

                if (usuario == null)
                {
                    return NotFound(ApiResponse<UsuarioAuthDto>.ErrorResponse("Usuário não encontrado"));
                }

                var usuarioDto = new UsuarioAuthDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Tipo = usuario.Tipo,
                    DataCadastro = usuario.DataCadastro
                };

                return Ok(ApiResponse<UsuarioAuthDto>.SuccessResponse(usuarioDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter perfil do usuário");
                return StatusCode(500, ApiResponse<UsuarioAuthDto>.ErrorResponse("Erro interno no servidor"));
            }
        }

        /// <summary>
        /// Realiza logout (cliente deve descartar o token)
        /// </summary>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("logout")]
        [Authorize]
        public ActionResult<ApiResponse<object>> Logout()
        {
            var userId = _authService.GetUserIdFromClaims(User);
            _logger.LogInformation("Logout realizado para usuário ID: {UserId}", userId);
            
            return Ok(ApiResponse<object>.SuccessResponse(
                new { message = "Logout realizado com sucesso" },
                "Logout realizado com sucesso. Descarte o token no cliente."
            ));
        }

        /// <summary>
        /// Valida se o token ainda é válido
        /// </summary>
        /// <returns>Status de validação</returns>
        [HttpGet("validate")]
        [Authorize]
        public ActionResult<ApiResponse<object>> ValidateToken()
        {
            var userId = _authService.GetUserIdFromClaims(User);
            var userRole = _authService.GetUserRoleFromClaims(User);

            return Ok(ApiResponse<object>.SuccessResponse(
                new 
                { 
                    valid = true, 
                    userId = userId,
                    role = userRole
                },
                "Token válido"
            ));
        }
    }
}
