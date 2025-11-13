using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;

namespace GestaoChamadosAI_MAUI.Services
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<T?> PostAsync<T>(string endpoint, object data);
        Task<T?> PostFormAsync<T>(string endpoint, Dictionary<string, string> formData);
        Task<T?> PutAsync<T>(string endpoint, object data);
        Task<T?> DeleteAsync<T>(string endpoint);
        void SetAuthToken(string token);
        void ClearAuthToken();
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IStorageService _storageService;
        
        // Para Android dispositivo real, use o IP do seu PC na rede local
        // Para Android Emulator, use 10.0.2.2
        // Para Windows Desktop, use localhost
        // API deve estar rodando na porta 5000
        // NOTA: NÃO incluir /api aqui pois os endpoints já têm /api no caminho
#if ANDROID
        private const string BaseUrl = "http://192.168.200.100:5000"; // IP local (funciona na mesma rede)
        // Para usar ngrok (funciona de qualquer lugar), descomente a linha abaixo:
        // private const string BaseUrl = "https://nondedicative-paresthetic-claudie.ngrok-free.dev";
#else
        private const string BaseUrl = "http://localhost:5000";
#endif

        public ApiService(IStorageService storageService)
        {
            _storageService = storageService;
            
            // Configurar HttpClient Handler para aceitar HTTP e ignorar SSL em desenvolvimento
            var handler = new HttpClientHandler();
#if DEBUG
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
            
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                await LoadTokenAsync();
                
                System.Diagnostics.Debug.WriteLine($"[GET] URL: {_httpClient.BaseAddress}{endpoint}");
                
                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"[GET] Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<T>(content);
                    return result;
                }
                
                System.Diagnostics.Debug.WriteLine($"[GET] Failed: {response.StatusCode}");
                return default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GET] Exception: {ex.Message}");
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                await LoadTokenAsync();
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"[POST] URL: {_httpClient.BaseAddress}{endpoint}");
                System.Diagnostics.Debug.WriteLine($"[POST] Data: {json}");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"[POST] Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[POST] Response: {responseContent}");

                // Sempre tenta desserializar a resposta, mesmo em caso de erro
                // A API retorna ApiResponse mesmo em erros
                if (!string.IsNullOrEmpty(responseContent))
                {
                    var result = JsonConvert.DeserializeObject<T>(responseContent);
                    return result;
                }

                System.Diagnostics.Debug.WriteLine("[POST] Response content is empty!");
                return default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[POST] Exception: {ex.Message}");
                throw; // Re-throw para capturar no AuthService
            }
        }

        public async Task<T?> PostFormAsync<T>(string endpoint, Dictionary<string, string> formData)
        {
            try
            {
                await LoadTokenAsync();
                var content = new FormUrlEncodedContent(formData);

                System.Diagnostics.Debug.WriteLine($"[POST FORM] URL: {_httpClient.BaseAddress}{endpoint}");
                System.Diagnostics.Debug.WriteLine($"[POST FORM] Form Data: {string.Join(", ", formData.Select(kv => $"{kv.Key}={kv.Value}"))}");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"[POST FORM] Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[POST FORM] Response: {responseContent}");

                if (!string.IsNullOrEmpty(responseContent))
                {
                    var result = JsonConvert.DeserializeObject<T>(responseContent);
                    return result;
                }

                return default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[POST FORM] Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                await LoadTokenAsync();
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }

                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PUT Error: {ex.Message}");
                return default;
            }
        }

        public async Task<T?> DeleteAsync<T>(string endpoint)
        {
            try
            {
                await LoadTokenAsync();
                var response = await _httpClient.DeleteAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(content);
                }

                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DELETE Error: {ex.Message}");
                return default;
            }
        }

        private async Task LoadTokenAsync()
        {
            var token = await _storageService.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                SetAuthToken(token);
            }
        }
    }
}
