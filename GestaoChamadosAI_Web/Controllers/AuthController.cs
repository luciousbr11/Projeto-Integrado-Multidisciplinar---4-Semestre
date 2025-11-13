using GestaoChamadosAI_Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestaoChamadosAI_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        public class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
            public UsuarioDto Usuario { get; set; } = null!;
        }

        public class UsuarioDto
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Tipo { get; set; } = string.Empty;
        }

        /// <summary>
        /// Endpoint de login para aplicações móveis (retorna JWT token)
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
                {
                    return Json(new { success = false, message = "Email e senha são obrigatórios." });
                }

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (usuario == null || usuario.Senha != request.Senha)
                {
                    return Json(new { success = false, message = "Email ou senha inválidos." });
                }

                // Gera o token JWT
                var token = GenerateJwtToken(usuario);

                var usuarioDto = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Tipo = usuario.Tipo
                };

                var response = new LoginResponse
                {
                    Token = token,
                    Usuario = usuarioDto
                };

                return Json(new { success = true, message = "Login realizado com sucesso!", data = response });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao fazer login: {ex.Message}" });
            }
        }

        /// <summary>
        /// Gera um token JWT para o usuário
        /// </summary>
        private string GenerateJwtToken(Models.Usuario usuario)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var jwtExpirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "8");

            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key não configurada");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Tipo),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(jwtExpirationHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Valida se o token JWT ainda é válido
        /// </summary>
        [HttpGet("validate")]
        public IActionResult ValidateToken()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                return Json(new
                {
                    success = true,
                    message = "Token válido",
                    data = new
                    {
                        id = userId,
                        nome = userName,
                        email = userEmail,
                        tipo = userRole
                    }
                });
            }

            return Json(new { success = false, message = "Token inválido ou expirado" });
        }
    }
}
