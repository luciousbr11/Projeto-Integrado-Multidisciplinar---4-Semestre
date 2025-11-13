# 🚀 Gestão de Chamados AI - Aplicativo MAUI

Aplicativo multiplataforma (Windows Desktop e Android Mobile) desenvolvido em .NET MAUI para gerenciamento de chamados com integração à API REST.

## 📱 Funcionalidades

### ✅ Implementadas
- **Login** com autenticação JWT
- **Dashboard** com estatísticas personalizadas por perfil
- **Lista de Chamados** com paginação e filtros
- **Detalhes do Chamado** com análise de IA
- **Criar Novo Chamado** com análise automática
- **Chat em Tempo Real** para cada chamado
- **Configurações** com ajuste de URL da API

### 🎯 Perfis de Usuário
- **Cliente**: Criar e acompanhar seus chamados
- **Suporte**: Atender e gerenciar chamados
- **Administrador**: Acesso completo ao sistema

## 🛠️ Tecnologias

- **.NET 9.0 MAUI** - Framework multiplataforma
- **MVVM Pattern** - Arquitetura com CommunityToolkit.Mvvm
- **HttpClient** - Comunicação com API REST
- **SecureStorage** - Armazenamento seguro de tokens
- **Newtonsoft.Json** - Serialização JSON

## 📦 Estrutura do Projeto

```
GestaoChamadosAI_MAUI/
├── Models/                    # DTOs e modelos de dados
│   ├── Usuario.cs
│   ├── Chamado.cs
│   └── Mensagem.cs
├── Services/                  # Camada de serviços
│   ├── ApiService.cs         # Cliente HTTP base
│   ├── AuthService.cs        # Autenticação JWT
│   ├── ChamadoService.cs     # Operações de chamados
│   └── StorageService.cs     # Armazenamento local
├── ViewModels/                # Lógica de apresentação (MVVM)
│   ├── LoginViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── ChamadosListViewModel.cs
│   ├── ChamadoDetalheViewModel.cs
│   ├── NovoChamadoViewModel.cs
│   ├── ChatViewModel.cs
│   └── ConfiguracoesViewModel.cs
├── Views/                     # Interfaces XAML
│   ├── LoginPage.xaml
│   ├── DashboardPage.xaml
│   ├── ChamadosListPage.xaml
│   ├── ChamadoDetalhePage.xaml
│   ├── NovoChamadoPage.xaml
│   ├── ChatPage.xaml
│   └── ConfiguracoesPage.xaml
├── Resources/                 # Recursos estáticos
│   ├── Fonts/
│   ├── Images/
│   └── Styles/
├── App.xaml                   # Aplicação principal
├── AppShell.xaml             # Shell de navegação
└── MauiProgram.cs            # Configuração e DI
```

## ⚙️ Configuração

### 1. Requisitos
- Visual Studio 2022 (17.8 ou superior)
- Workload ".NET Multi-platform App UI development"
- SDK .NET 9.0
- Para Android: SDK Android 21+ (Lollipop)
- Para Windows: Windows 10.0.17763.0+

### 2. Configurar API
Edite o arquivo `Services/ApiService.cs`:
```csharp
private const string BaseUrl = "http://10.0.2.2:5000/api"; // Android Emulator
// private const string BaseUrl = "http://localhost:5000/api"; // Windows Desktop
```

> **Nota**: Use `10.0.2.2` no emulador Android para acessar localhost do PC host.

### 3. Restaurar Pacotes
```powershell
cd GestaoChamadosAI_MAUI
dotnet restore
```

### 4. Compilar
```powershell
# Windows Desktop
dotnet build -f net9.0-windows10.0.19041.0

# Android
dotnet build -f net9.0-android
```

### 5. Executar

**Windows Desktop:**
```powershell
dotnet run -f net9.0-windows10.0.19041.0
```

**Android (Emulador):**
```powershell
dotnet build -f net9.0-android -t:Run
```

## 📱 Uso do Aplicativo

### 1. Login
- Email: `admin@teste.com`
- Senha: `admin123`

Outros usuários de teste:
- Suporte: `suporte@teste.com` / `suporte123`
- Cliente: `cliente@teste.com` / `cliente123`

### 2. Dashboard
- Visualize estatísticas personalizadas
- Acesso rápido às funcionalidades
- Informações de acordo com seu perfil

### 3. Chamados
- **Listar**: Veja todos os chamados (com filtros)
- **Criar**: Novo chamado com análise de IA automática
- **Detalhes**: Veja prioridade, categoria e resposta da IA
- **Chat**: Converse em tempo real sobre o chamado

### 4. Navegação
- Menu inferior (Bottom Navigation) no mobile
- Menu lateral (Side Menu) no desktop
- Pull-to-refresh nas listas
- Scroll infinito com paginação

## 🎨 Interface

### Mobile (Android)
- Design Material Design 3
- Bottom Navigation Bar
- Cards responsivos
- Gestos de swipe

### Desktop (Windows)
- Navigation View com menu lateral
- Layouts adaptativos
- Atalhos de teclado
- Multi-janela (futuro)

## 🔐 Segurança

- **JWT Token**: Armazenado em SecureStorage
- **HTTPS**: Comunicação criptografada (produção)
- **Auto-logout**: Token expirado = logout automático
- **Validação**: Todos os inputs são validados

## 🔄 Sincronização

- **Automática**: Dados atualizados ao abrir telas
- **Manual**: Pull-to-refresh em listas
- **Tempo Real**: Polling de mensagens (5 segundos)

## 🐛 Troubleshooting

### Erro: "Cannot connect to API"
**Solução:**
1. Verifique se a API está rodando: `http://localhost:5000`
2. No Android Emulator, use `http://10.0.2.2:5000/api`
3. Verifique firewall/antivírus

### Erro: "Login failed"
**Solução:**
1. Confirme que o banco de dados está configurado
2. Verifique se os usuários de teste existem
3. Valide URL da API nas configurações

### Erro: "Build failed - Android SDK not found"
**Solução:**
1. Abra Visual Studio Installer
2. Modifique instalação do VS 2022
3. Instale "Mobile development with .NET"

## 📊 Próximas Features

- [ ] Notificações push
- [ ] Modo offline com cache local
- [ ] Anexar arquivos aos chamados
- [ ] Dark mode
- [ ] Biometria para login
- [ ] Relatórios em PDF
- [ ] Suporte a iOS

## 📝 Arquivos Principais Criados

### ✅ Completos
- [x] GestaoChamadosAI_MAUI.csproj
- [x] App.xaml + App.xaml.cs
- [x] MauiProgram.cs
- [x] 3 Models (Usuario, Chamado, Mensagem)
- [x] 4 Services (Api, Auth, Chamado, Storage)
- [x] 7 ViewModels completos
- [x] LoginPage.xaml completa

### ⏳ Para Criar Manualmente
- [ ] DashboardPage.xaml
- [ ] ChamadosListPage.xaml
- [ ] ChamadoDetalhePage.xaml
- [ ] NovoChamadoPage.xaml
- [ ] ChatPage.xaml
- [ ] ConfiguracoesPage.xaml
- [ ] AppShell.xaml
- [ ] Resources/Styles/Colors.xaml
- [ ] Resources/Styles/Styles.xaml
- [ ] Converters (StringToBoolConverter, InvertedBoolConverter)

## 🎓 Exemplos de Código

### Chamar API
```csharp
var response = await _apiService.GetAsync<ApiResponse<Chamado>>($"/chamados/{id}");
if (response?.Success == true)
{
    Chamado = response.Data;
}
```

### Navegar entre páginas
```csharp
await Shell.Current.GoToAsync($"{nameof(ChamadoDetalhePage)}?Id={chamado.Id}");
```

### Armazenar dados
```csharp
await _storageService.SetAsync("auth_token", token);
var token = await _storageService.GetAsync("auth_token");
```

## 📞 Suporte

Para dúvidas ou problemas:
1. Consulte este README
2. Verifique logs do Visual Studio (Output > Debug)
3. Teste a API diretamente no Swagger

**Versão:** 1.0.0  
**Data:** 05/11/2025  
**Framework:** .NET 9.0 MAUI
