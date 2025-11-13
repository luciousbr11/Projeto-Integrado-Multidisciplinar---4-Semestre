using System.Net.Http.Json;
using Newtonsoft.Json;

Console.WriteLine("=== TESTE DA API - Gestão de Chamados ===\n");

var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000/api/") };

try
{
    // 1. Login
    Console.WriteLine("1. Fazendo login...");
    var loginResponse = await client.PostAsJsonAsync("auth/login", new
    {
        email = "admin@teste.com",
        senha = "admin123"
    });

    if (loginResponse.IsSuccessStatusCode)
    {
        var loginResult = await loginResponse.Content.ReadAsStringAsync();
        dynamic? result = JsonConvert.DeserializeObject(loginResult);
        string? token = result?.data?.token;
        
        Console.WriteLine($"✓ Login realizado com sucesso!");
        Console.WriteLine($"  Token: {token?[..50]}...\n");

        // Adicionar token no header
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Buscar Dashboard
        Console.WriteLine("2. Buscando estatísticas do dashboard...");
        var dashboardResponse = await client.GetAsync("dashboard");
        
        if (dashboardResponse.IsSuccessStatusCode)
        {
            var dashboardData = await dashboardResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"✓ Dashboard carregado:");
            Console.WriteLine($"{dashboardData}\n");
        }

        // 3. Listar Chamados
        Console.WriteLine("3. Listando chamados...");
        var chamadosResponse = await client.GetAsync("chamados?pagina=1&tamanhoPagina=5");
        
        if (chamadosResponse.IsSuccessStatusCode)
        {
            var chamadosData = await chamadosResponse.Content.ReadAsStringAsync();
            dynamic? chamados = JsonConvert.DeserializeObject(chamadosData);
            Console.WriteLine($"✓ Chamados encontrados: {chamados?.data?.totalItens}");
            Console.WriteLine($"{chamadosData}\n");
        }

        Console.WriteLine("✓ Todos os testes passaram!");
    }
    else
    {
        Console.WriteLine($"✗ Erro no login: {loginResponse.StatusCode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Erro: {ex.Message}");
}

Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();
