using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoChamadosAI_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UploadController> _logger;
        private readonly IConfiguration _configuration;

        // Tamanho máximo: 10MB
        private const long MaxFileSize = 10 * 1024 * 1024;

        // Extensões permitidas
        private static readonly string[] AllowedExtensions = new[]
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", // Imagens
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv", // Documentos
            ".zip", ".rar", ".7z" // Arquivos compactados
        };

        public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger, IConfiguration configuration)
        {
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Nenhum arquivo foi enviado." });
                }

                // Validar tamanho
                if (file.Length > MaxFileSize)
                {
                    return BadRequest(new { success = false, message = "Arquivo muito grande. Tamanho máximo: 10MB." });
                }

                // Validar extensão
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                {
                    return BadRequest(new { success = false, message = "Tipo de arquivo não permitido." });
                }

                // Obter pasta compartilhada das configurações ou usar padrão
                var sharedUploadPath = _configuration["UploadSettings:SharedUploadPath"];
                string uploadsPath;
                
                if (!string.IsNullOrEmpty(sharedUploadPath) && Directory.Exists(Path.GetDirectoryName(sharedUploadPath)))
                {
                    // Usar pasta compartilhada (GestaoChamadosAI_Web/wwwroot/uploads)
                    uploadsPath = sharedUploadPath;
                    _logger.LogInformation("Usando pasta compartilhada: {Path}", uploadsPath);
                }
                else
                {
                    // Fallback: usar pasta local
                    uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                    _logger.LogWarning("Pasta compartilhada não encontrada, usando pasta local: {Path}", uploadsPath);
                }
                
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                    _logger.LogInformation("Pasta de uploads criada: {Path}", uploadsPath);
                }

                // Gerar nome único para o arquivo
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Salvar arquivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Retornar URL pública
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

                _logger.LogInformation("✅ Arquivo salvo: {FileName} → {FilePath}", file.FileName, filePath);
                _logger.LogInformation("🌐 URL pública: {FileUrl}", fileUrl);

                return Ok(new
                {
                    success = true,
                    nomeArquivo = file.FileName,
                    nomeArquivoSalvo = fileName,
                    url = fileUrl,
                    tamanho = file.Length,
                    tipo = file.ContentType
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer upload do arquivo");
                return StatusCode(500, new { success = false, message = "Erro ao fazer upload do arquivo." });
            }
        }

        [HttpDelete("{fileName}")]
        public IActionResult Delete(string fileName)
        {
            try
            {
                // Obter pasta compartilhada das configurações ou usar padrão
                var sharedUploadPath = _configuration["UploadSettings:SharedUploadPath"];
                string uploadsPath;
                
                if (!string.IsNullOrEmpty(sharedUploadPath) && Directory.Exists(sharedUploadPath))
                {
                    uploadsPath = sharedUploadPath;
                }
                else
                {
                    uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                }
                
                var filePath = Path.Combine(uploadsPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { success = false, message = "Arquivo não encontrado." });
                }

                System.IO.File.Delete(filePath);

                return Ok(new { success = true, message = "Arquivo excluído com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir arquivo");
                return StatusCode(500, new { success = false, message = "Erro ao excluir arquivo." });
            }
        }
    }
}
