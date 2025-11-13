using GestaoChamadosAI_MAUI.Models;
using System.Text.Json;

namespace GestaoChamadosAI_MAUI.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IApiService _apiService;

        public UsuarioService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<Usuario>?> GetUsuariosAsync(int page = 1, int pageSize = 50, string? tipo = null)
        {
            try
            {
                var url = $"/api/Usuarios?page={page}&pageSize={pageSize}";
                if (!string.IsNullOrEmpty(tipo))
                    url += $"&tipo={tipo}";

                var response = await _apiService.GetAsync<ApiResponse<PagedResult<Usuario>>>(url);
                return response?.Data?.Items;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UsuarioDetalhado?> GetUsuarioAsync(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<UsuarioDetalhado>>($"/api/Usuarios/{id}");
                return response?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<(bool Success, string Message)> CreateUsuarioAsync(CreateUsuarioRequest request)
        {
            try
            {
                var response = await _apiService.PostAsync<ApiResponse<Usuario>>("/api/Usuarios", request);
                
                if (response != null && response.Success)
                    return (true, response.Message ?? "Usuário criado com sucesso!");
                
                return (false, response?.Message ?? "Erro ao criar usuário");
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateUsuarioAsync(int id, UpdateUsuarioRequest request)
        {
            try
            {
                var response = await _apiService.PutAsync<ApiResponse<Usuario>>($"/api/Usuarios/{id}", request);
                
                if (response != null && response.Success)
                    return (true, response.Message ?? "Usuário atualizado com sucesso!");
                
                return (false, response?.Message ?? "Erro ao atualizar usuário");
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteUsuarioAsync(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ApiResponse<object>>($"/api/Usuarios/{id}");
                
                if (response != null && response.Success)
                    return (true, response.Message ?? "Usuário excluído com sucesso!");
                
                return (false, response?.Message ?? "Erro ao excluir usuário");
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> AlterarSenhaAsync(int id, string novaSenha)
        {
            try
            {
                var request = new AlterarSenhaRequest { NovaSenha = novaSenha };
                var response = await _apiService.PostAsync<ApiResponse<object>>($"/api/Usuarios/{id}/alterar-senha", request);
                
                if (response != null && response.Success)
                    return (true, response.Message ?? "Senha alterada com sucesso!");
                
                return (false, response?.Message ?? "Erro ao alterar senha");
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }
        }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
