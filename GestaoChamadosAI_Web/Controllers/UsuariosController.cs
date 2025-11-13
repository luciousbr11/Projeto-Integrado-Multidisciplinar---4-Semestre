using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_Web.Data;
using GestaoChamadosAI_Web.Models;

namespace GestaoChamadosAI_Web.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento completo de usuários (CRUD).
    /// Permite criar, visualizar, editar e excluir usuários do sistema.
    /// </summary>
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        // Construtor com injeçáo de dependência do contexto do banco de dados
        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Action GET: Exibe a lista de todos os usuários cadastrados no sistema.
        /// Ordena os usuários por nome para facilitar a visualização.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Busca todos os usuários ordenados por nome
            var usuarios = await _context.Usuarios
                .OrderBy(u => u.Nome)
                .ToListAsync();

            return View(usuarios);
        }

        /// <summary>
        /// Action GET: Filtra usuários por tipo.
        /// </summary>
        /// <param name="tipo">Tipo de usuário a ser filtrado (Administrador, Suporte, Cliente)</param>
        public async Task<IActionResult> FiltrarPorTipo(string tipo)
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.Tipo == tipo)
                .OrderBy(u => u.Nome)
                .ToListAsync();

            ViewBag.TipoFiltro = tipo;
            return View("Index", usuarios);
        }

        /// <summary>
        /// Action GET: Exibe os detalhes de um usuário específico.
        /// Mostra todas as informações do usuário incluindo seus chamados.
        /// </summary>
        /// <param name="id">ID do usuário a ser exibido</param>
        public async Task<IActionResult> Details(int? id)
        {
            // Valida se o ID foi fornecido
            if (id == null)
            {
                return NotFound();
            }

            // Busca o usuário incluindo seus chamados relacionados
            var usuario = await _context.Usuarios
                .Include(u => u.Chamados) // Inclui os chamados do usuário
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        /// <summary>
        /// Action GET: Exibe o formulário de criaçáo de um novo usuário.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Action POST: Processa o formulário de criaçáo de um novo usuário.
        /// Valida os dados e salva o novo usuário no banco de dados.
        /// </summary>
        /// <param name="usuario">Objeto usuário com os dados do formulário</param>
        [HttpPost]
        [ValidateAntiForgeryToken] // Proteçáo contra ataques CSRF
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,Senha,Tipo")] Usuario usuario)
        {
            // Verifica se o modelo é válido (validações de anotações)
            if (ModelState.IsValid)
            {
                // Verifica se já existe um usuário com o mesmo e-mail
                var emailExistente = await _context.Usuarios
                    .AnyAsync(u => u.Email == usuario.Email);

                if (emailExistente)
                {
                    ModelState.AddModelError("Email", "Já existe um usuário cadastrado com este e-mail.");
                    return View(usuario);
                }

                // Adiciona o novo usuário ao contexto
                _context.Add(usuario);
                
                // Salva as alterações no banco de dados
                await _context.SaveChangesAsync();

                // Exibe mensagem de sucesso (requer configuraçáo de TempData)
                TempData["Mensagem"] = "Usuário criado com sucesso!";
                
                // Redireciona para a lista de usuários
                return RedirectToAction(nameof(Index));
            }

            // Se o modelo náo for válido, retorna para o formulário com os erros
            return View(usuario);
        }

        /// <summary>
        /// Action GET: Exibe o formulário de ediçáo de um usuário existente.
        /// Carrega os dados atuais do usuário no formulário.
        /// </summary>
        /// <param name="id">ID do usuário a ser editado</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Busca o usuário pelo ID
            var usuario = await _context.Usuarios.FindAsync(id);
            
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        /// <summary>
        /// Action POST: Processa o formulário de ediçáo de um usuário.
        /// Atualiza os dados do usuário no banco de dados.
        /// </summary>
        /// <param name="id">ID do usuário sendo editado</param>
        /// <param name="usuario">Objeto com os novos dados do usuário</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Senha,Tipo")] Usuario usuario)
        {
            // Valida se o ID corresponde ao usuário sendo editado
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica se o e-mail já está sendo usado por outro usuário
                    var emailExistente = await _context.Usuarios
                        .AnyAsync(u => u.Email == usuario.Email && u.Id != usuario.Id);

                    if (emailExistente)
                    {
                        ModelState.AddModelError("Email", "Já existe outro usuário cadastrado com este e-mail.");
                        return View(usuario);
                    }

                    // Atualiza o usuário no contexto
                    _context.Update(usuario);
                    
                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    TempData["Mensagem"] = "Usuário atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Trata conflitos de concorrência (usuário modificado por outro processo)
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        /// <summary>
        /// Action GET: Exibe a página de confirmaçáo de exclusáo de um usuário.
        /// Mostra os dados do usuário antes de confirmar a exclusáo.
        /// </summary>
        /// <param name="id">ID do usuário a ser excluído</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Busca o usuário com seus chamados para exibir na confirmaçáo
            var usuario = await _context.Usuarios
                .Include(u => u.Chamados)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        /// <summary>
        /// Action POST: Processa a exclusáo confirmada de um usuário.
        /// Remove o usuário e todos os seus chamados associados do banco de dados.
        /// </summary>
        /// <param name="id">ID do usuário a ser excluído</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Busca o usuário com seus chamados
            var usuario = await _context.Usuarios
                .Include(u => u.Chamados)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario != null)
            {
                // Remove todos os chamados do usuário primeiro
                if (usuario.Chamados != null && usuario.Chamados.Any())
                {
                    _context.Chamados.RemoveRange(usuario.Chamados);
                }

                // Remove o usuário
                _context.Usuarios.Remove(usuario);
                
                // Salva as alterações
                await _context.SaveChangesAsync();

                TempData["Mensagem"] = "Usuário excluído com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Action GET: Retorna lista de suportes para a API (MAUI).
        /// </summary>
        [HttpGet]
        [Route("api/Usuarios/suportes")]
        public async Task<IActionResult> GetSuportes()
        {
            var suportes = await _context.Usuarios
                .Where(u => u.Tipo == "Suporte" || u.Tipo == "Administrador")
                .OrderBy(u => u.Nome)
                .Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Email,
                    u.Tipo
                })
                .ToListAsync();
            
            return Json(new { success = true, data = suportes });
        }

        /// <summary>
        /// Action POST: Altera senha de um usuário (para API/MAUI).
        /// </summary>
        [HttpPost]
        [Route("api/Usuarios/{id}/alterar-senha")]
        public async Task<IActionResult> AlterarSenha(int id, [FromBody] AlterarSenhaRequest request)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuário não encontrado." });
            }

            if (string.IsNullOrWhiteSpace(request?.NovaSenha) || request.NovaSenha.Length < 6)
            {
                return Json(new { success = false, message = "A senha deve ter no mínimo 6 caracteres." });
            }

            // Salva a senha diretamente (sem hash, conforme padrão do sistema)
            usuario.Senha = request.NovaSenha;
            
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Senha alterada com sucesso!" });
        }

        /// <summary>
        /// Método auxiliar privado que verifica se um usuário existe no banco de dados.
        /// Utilizado para validações e tratamento de erros.
        /// </summary>
        /// <param name="id">ID do usuário a verificar</param>
        /// <returns>True se o usuário existe, False caso contrário</returns>
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }

    /// <summary>
    /// Classe auxiliar para receber request de alteração de senha da API.
    /// </summary>
    public class AlterarSenhaRequest
    {
        public string? NovaSenha { get; set; }
    }
}
