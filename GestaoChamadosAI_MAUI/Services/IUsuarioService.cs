using GestaoChamadosAI_MAUI.Models;

namespace GestaoChamadosAI_MAUI.Services
{
    public interface IUsuarioService
    {
        Task<List<Usuario>?> GetUsuariosAsync(int page = 1, int pageSize = 50, string? tipo = null);
        Task<UsuarioDetalhado?> GetUsuarioAsync(int id);
        Task<(bool Success, string Message)> CreateUsuarioAsync(CreateUsuarioRequest request);
        Task<(bool Success, string Message)> UpdateUsuarioAsync(int id, UpdateUsuarioRequest request);
        Task<(bool Success, string Message)> DeleteUsuarioAsync(int id);
        Task<(bool Success, string Message)> AlterarSenhaAsync(int id, string novaSenha);
    }
}
