using GestaoChamadosAI_MAUI.Models;

namespace GestaoChamadosAI_MAUI.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, Usuario? Usuario)> LoginAsync(string email, string senha);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<Usuario?> GetCurrentUserAsync();
        Task<string?> GetTokenAsync();
        Task<string> GetApiUrlAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly IStorageService _storageService;
        private Usuario? _currentUser;

        public AuthService(IApiService apiService, IStorageService storageService)
        {
            _apiService = apiService;
            _storageService = storageService;
        }

        public async Task<(bool Success, string Message, Usuario? Usuario)> LoginAsync(string email, string senha)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[AuthService] Iniciando login para: {email}");
                
                var request = new LoginRequest { Email = email, Senha = senha };
                
                System.Diagnostics.Debug.WriteLine($"[AuthService] Chamando API...");
                var response = await _apiService.PostAsync<ApiResponse<LoginResponse>>("/api/Auth/login", request);
                
                System.Diagnostics.Debug.WriteLine($"[AuthService] Resposta recebida: {response != null}");

                if (response != null && response.Success && response.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[AuthService] Login bem-sucedido!");
                    
                    // Salvar token
                    await _storageService.SetAsync("auth_token", response.Data.Token);
                    await _storageService.SetAsync("user_id", response.Data.Usuario.Id.ToString());
                    await _storageService.SetAsync("user_name", response.Data.Usuario.Nome);
                    await _storageService.SetAsync("user_email", response.Data.Usuario.Email);
                    await _storageService.SetAsync("user_tipo", response.Data.Usuario.Tipo);

                    _apiService.SetAuthToken(response.Data.Token);
                    _currentUser = response.Data.Usuario;

                    return (true, "Login realizado com sucesso!", response.Data.Usuario);
                }

                System.Diagnostics.Debug.WriteLine($"[AuthService] Login falhou: {response?.Message}");
                return (false, response?.Message ?? "Email ou senha inválidos", null);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AuthService] HttpRequestException: {ex}");
                var innerMsg = ex.InnerException?.Message ?? "";
                return (false, $"ERRO DE CONEXÃO\n\nNão foi possível conectar à API.\n\nErro: {ex.Message}\n\nInner: {innerMsg}", null);
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AuthService] Timeout: {ex}");
                return (false, "TIMEOUT: A API não respondeu em 30 segundos", null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AuthService] Exception: {ex}");
                return (false, $"ERRO: {ex.GetType().Name}\n\n{ex.Message}", null);
            }
        }

        public async Task LogoutAsync()
        {
            await _storageService.RemoveAsync("auth_token");
            await _storageService.RemoveAsync("user_id");
            await _storageService.RemoveAsync("user_name");
            await _storageService.RemoveAsync("user_email");
            await _storageService.RemoveAsync("user_tipo");

            _apiService.ClearAuthToken();
            _currentUser = null;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _storageService.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }

        public async Task<Usuario?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
                return _currentUser;

            var userId = await _storageService.GetAsync("user_id");
            if (string.IsNullOrEmpty(userId))
                return null;

            _currentUser = new Usuario
            {
                Id = int.Parse(userId),
                Nome = await _storageService.GetAsync("user_name") ?? "",
                Email = await _storageService.GetAsync("user_email") ?? "",
                Tipo = await _storageService.GetAsync("user_tipo") ?? ""
            };

            return _currentUser;
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _storageService.GetAsync("auth_token");
        }

        public async Task<string> GetApiUrlAsync()
        {
#if ANDROID
            return "http://192.168.200.100:5000";
#else
            return "http://localhost:5000";
#endif
        }
    }
}
