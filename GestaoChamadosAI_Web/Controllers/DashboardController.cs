using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_Web.Data;

namespace GestaoChamadosAI_Web.Controllers
{
    /// <summary>
    /// Controller do Dashboard principal.
    /// Exibe diferentes dashboards baseado no tipo de usuário logado.
    /// </summary>
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Action principal que redireciona para o dashboard apropriado
        /// baseado no tipo de usuário (Cliente, Suporte ou Administrador).
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);

            // Redireciona baseado no tipo de usuário
            return userRole switch
            {
                "Administrador" => RedirectToAction("Administrador"),
                "Suporte" => RedirectToAction("Suporte"),
                "Cliente" => RedirectToAction("Cliente"),
                _ => RedirectToAction("Login", "Account")
            };
        }

        /// <summary>
        /// Dashboard do Administrador - Visáo completa do sistema.
        /// </summary>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Administrador()
        {
            var totalChamados = await _context.Chamados.CountAsync();
            var chamadosAbertos = await _context.Chamados.CountAsync(c => c.Status == "Aberto");
            var chamadosEmAndamento = await _context.Chamados.CountAsync(c => c.Status == "Em Andamento");
            var chamadosResolvidos = await _context.Chamados.CountAsync(c => c.Status == "Resolvido" || c.Status == "Solucionado por IA");
            var chamadosSolucionadosIA = await _context.Chamados.CountAsync(c => c.Status == "Solucionado por IA");

            var chamadosRecentes = await _context.Chamados
                .Include(c => c.Usuario)
                .OrderByDescending(c => c.DataAbertura)
                .Take(15)
                .ToListAsync();

            ViewBag.TotalChamados = totalChamados;
            ViewBag.ChamadosAbertos = chamadosAbertos;
            ViewBag.ChamadosEmAndamento = chamadosEmAndamento;
            ViewBag.ChamadosResolvidos = chamadosResolvidos;
            ViewBag.ChamadosSolucionadosIA = chamadosSolucionadosIA;
            ViewBag.ChamadosRecentes = chamadosRecentes;

            return View();
        }

        /// <summary>
        /// Dashboard do Suporte - Gerenciamento de chamados.
        /// </summary>
        [Authorize(Roles = "Suporte,Administrador")]
        public async Task<IActionResult> Suporte()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);
            var hoje = DateTime.Today;
            
            var chamadosPendentes = await _context.Chamados.CountAsync(c => c.Status == "Aberto");
            var chamadosEmAndamento = await _context.Chamados.CountAsync(c => c.Status == "Em Andamento");
            var chamadosResolvidosHoje = await _context.Chamados
                .CountAsync(c => (c.Status == "Resolvido" || c.Status == "Solucionado por IA") && c.DataAbertura.Date == hoje);

            // Chamados atribuídos ao suporte logado
            var meusChamados = await _context.Chamados
                .Include(c => c.Usuario)
                .Include(c => c.SuporteResponsavel)
                .Where(c => c.SuporteResponsavelId == userId && (c.Status == "Aberto" || c.Status == "Em Atendimento"))
                .OrderByDescending(c => c.DataAbertura)
                .ToListAsync();

            // Chamados que foram transferidos pelo suporte logado (já não são mais dele)
            var chamadosTransferidos = await _context.Chamados
                .Include(c => c.Usuario)
                .Include(c => c.SuporteResponsavel)
                .Include(c => c.Mensagens)
                .Where(c => c.Status == "Aberto" || c.Status == "Em Atendimento")
                .ToListAsync();

            // Filtra chamados onde o usuário enviou mensagem mas não é mais o responsável
            var chamadosQueTranferi = chamadosTransferidos
                .Where(c => c.SuporteResponsavelId != userId && 
                           c.Mensagens.Any(m => m.UsuarioId == userId))
                .OrderByDescending(c => c.DataAbertura)
                .Take(10)
                .ToList();

            // Chamados prioritários aguardando atendimento (apenas status "Aberto")
            var chamadosAlta = await _context.Chamados
                .Include(c => c.Usuario)
                .Where(c => c.Status == "Aberto")
                .OrderByDescending(c => c.DataAbertura)
                .Take(10)
                .ToListAsync();

            var chamadosAbertos = await _context.Chamados
                .Include(c => c.Usuario)
                .Where(c => c.Status == "Aberto")
                .OrderByDescending(c => c.DataAbertura)
                .Take(15)
                .ToListAsync();

            ViewBag.ChamadosPendentes = chamadosPendentes;
            ViewBag.ChamadosEmAndamento = chamadosEmAndamento;
            ViewBag.ChamadosResolvidosHoje = chamadosResolvidosHoje;
            ViewBag.ChamadosAlta = chamadosAlta;
            ViewBag.ChamadosAbertos = chamadosAbertos;
            ViewBag.MeusChamados = meusChamados;
            ViewBag.ChamadosTransferidos = chamadosQueTranferi;

            return View();
        }

        /// <summary>
        /// Dashboard do Cliente - Visualizaçáo dos próprios chamados.
        /// </summary>
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Cliente()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim.Value);

            var meusChamados = await _context.Chamados
                .Include(c => c.Usuario)
                .Where(c => c.UsuarioId == userId)
                .OrderByDescending(c => c.DataAbertura)
                .ToListAsync();

            var totalChamados = meusChamados.Count;
            var chamadosAbertos = meusChamados.Count(c => c.Status == "Aberto");
            var chamadosEmAndamento = meusChamados.Count(c => c.Status == "Em Atendimento");
            var chamadosResolvidos = meusChamados.Count(c => c.Status == "Concluído" || c.Status == "Solucionado por IA");

            var chamadosAtivos = meusChamados
                .Where(c => c.Status == "Aberto" || c.Status == "Em Atendimento")
                .Take(10)
                .ToList();

            var chamadosResolvidosRecentes = meusChamados
                .Where(c => c.Status == "Resolvido" || c.Status == "Solucionado por IA")
                .OrderByDescending(c => c.DataAbertura)
                .Take(10)
                .ToList();

            ViewBag.TotalChamados = totalChamados;
            ViewBag.ChamadosAbertos = chamadosAbertos;
            ViewBag.ChamadosEmAndamento = chamadosEmAndamento;
            ViewBag.ChamadosResolvidos = chamadosResolvidos;
            ViewBag.ChamadosAtivos = chamadosAtivos;
            ViewBag.ChamadosResolvidosRecentes = chamadosResolvidosRecentes;

            return View();
        }
    }
}
