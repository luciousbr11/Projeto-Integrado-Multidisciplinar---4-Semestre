using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_Web.Data;
using GestaoChamadosAI_Web.Models;

namespace GestaoChamadosAI_Web.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticaçáo de usuários.
    /// Gerencia login, logout e controle de acesso ao sistema.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: Exibe a tela de login.
        /// Se o usuário já estiver autenticado, redireciona para o dashboard apropriado.
        /// </summary>
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            // Se já está autenticado, redireciona
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// POST: Processa o login do usuário.
        /// Valida as credenciais e cria a sessáo de autenticaçáo.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string senha, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                ModelState.AddModelError("", "E-mail e senha sáo obrigatórios.");
                return View();
            }

            // Busca o usuário pelo e-mail e senha
            // NOTA: Em produçáo, use hash de senha (BCrypt, Identity, etc.)
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Senha == senha);

            if (usuario == null)
            {
                ModelState.AddModelError("", "E-mail ou senha inválidos.");
                return View();
            }

            // Cria as claims do usuário (informações que seráo armazenadas no cookie)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Tipo) // Cliente, Suporte, Administrador
            };

            // Cria a identidade do usuário
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Configura as propriedades do cookie de autenticaçáo
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Cookie persiste após fechar o navegador
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) // Expira em 8 horas
            };

            // Faz o login (cria o cookie de autenticaçáo)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            // Redireciona para a URL de retorno ou para o dashboard
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// POST: Faz o logout do usuário.
        /// Remove o cookie de autenticaçáo e encerra a sessáo.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Remove o cookie de autenticaçáo
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// GET: Exibe a página de acesso negado.
        /// Mostrada quando o usuário tenta acessar uma área sem permissáo.
        /// </summary>
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// GET: Exibe o perfil do usuário logado.
        /// </summary>
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Logout");
            }

            var userId = int.Parse(userIdClaim.Value);
            var usuario = await _context.Usuarios
                .Include(u => u.Chamados)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
            {
                return RedirectToAction("Logout");
            }

            return View(usuario);
        }
    }
}
