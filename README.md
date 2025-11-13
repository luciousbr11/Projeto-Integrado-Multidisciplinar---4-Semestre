# 🎯 Sistema de Gestão de Chamados com IA

> Sistema completo de gerenciamento de chamados de suporte técnico com integração de Inteligência Artificial (Google Gemini), desenvolvido para o TCC do 4º Semestre.

## 📋 Sobre o Projeto

Sistema multiplataforma para gestão eficiente de chamados de suporte técnico, utilizando IA para categorização automática e sugestões de respostas. O projeto oferece três interfaces distintas (Web, Desktop e Mobile) consumindo uma única API REST centralizada.

### ✨ Principais Funcionalidades

- 🤖 **Categorização automática** de chamados usando Google Gemini AI
- 💬 **Chat em tempo real** para comunicação entre cliente e suporte
- 📊 **Dashboard personalizado** por perfil de usuário
- 📁 **Anexos e arquivos** em mensagens
- 📈 **Relatórios e estatísticas** em PDF
- 🔐 **Autenticação JWT** com tokens de acesso e renovação
- 👥 **Três perfis de usuário**: Cliente, Suporte e Administrador

## 🏗️ Arquitetura do Sistema

```
┌─────────────────────────────────────────────────────────┐
│                    Clientes                             │
├──────────────┬──────────────────┬──────────────────────┤
│  Web (MVC)   │  Desktop (MAUI)  │  Mobile (MAUI)       │
│  ASP.NET     │  Windows         │  Android             │
└──────┬───────┴────────┬─────────┴──────────┬───────────┘
       │                │                    │
       └────────────────┼────────────────────┘
                        │
                ┌───────▼────────┐
                │   API REST     │
                │   .NET 9.0     │
                │   JWT Auth     │
                └───────┬────────┘
                        │
                ┌───────▼────────┐
                │  SQL Server    │
                │   Database     │
                └────────────────┘
```

## 🛠️ Tecnologias Utilizadas

### Backend (API)
- **.NET 9.0** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core** - ORM
- **SQL Server** - Banco de dados
- **JWT Authentication** - Segurança
- **Google Gemini AI** - Inteligência Artificial
- **BCrypt** - Criptografia de senhas
- **Serilog** - Logging estruturado
- **iText7** - Geração de PDFs
- **Swagger** - Documentação automática

### Frontend Web
- **ASP.NET Core MVC** - Framework web
- **Razor Pages** - Views
- **Bootstrap 5** - Interface responsiva
- **JavaScript/jQuery** - Interatividade
- **Chart.js** - Gráficos e estatísticas

### Aplicativo MAUI (Desktop e Mobile)
- **.NET MAUI** - Framework multiplataforma
- **MVVM Pattern** - Arquitetura
- **CommunityToolkit.Mvvm** - Helpers MVVM
- **SecureStorage** - Armazenamento seguro

## 📁 Estrutura do Repositório

```
GestaoChamadosAI/
├── GestaoChamadosAI_API/          # 🔧 API REST (.NET 9.0)
│   ├── Controllers/                # Endpoints da API
│   ├── Services/                   # Lógica de negócio
│   ├── Models/                     # Entidades do banco
│   ├── DTOs/                       # Data Transfer Objects
│   ├── Data/                       # DbContext e migrations
│   └── Middleware/                 # Middleware customizado
│
├── GestaoChamadosAI_Web/          # 🌐 Aplicação Web (MVC)
│   ├── Controllers/                # Controllers MVC
│   ├── Views/                      # Views Razor
│   ├── Models/                     # ViewModels
│   ├── Services/                   # Serviços de integração
│   └── wwwroot/                    # Assets estáticos
│
├── GestaoChamadosAI_MAUI/         # 📱 App Desktop/Mobile
│   ├── Views/                      # Interfaces XAML
│   ├── ViewModels/                 # ViewModels MVVM
│   ├── Services/                   # Serviços de API
│   ├── Models/                     # Modelos de dados
│   └── Platforms/                  # Código específico por plataforma
│
└── TesteAPI_Console/              # 🧪 Testes da API
```

## 🚀 Como Executar

### Pré-requisitos

- **.NET SDK 9.0** ou superior
- **SQL Server** (LocalDB, Express ou SQL Server)
- **Visual Studio 2022** ou **VS Code**
- **Google Gemini API Key** (gratuita)

### 1️⃣ Configurar o Banco de Dados

```bash
cd GestaoChamadosAI_API
dotnet ef database update
```

### 2️⃣ Configurar a API

Edite o arquivo `GestaoChamadosAI_API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ConexaoPadrao": "Server=localhost;Database=GestaoChamadosAI;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "GeminiAI": {
    "ApiKey": "SUA_CHAVE_API_AQUI"
  }
}
```

### 3️⃣ Executar a API

```bash
cd GestaoChamadosAI_API
dotnet run
```

A API estará disponível em: `https://localhost:7296`

### 4️⃣ Executar a Aplicação Web

```bash
cd GestaoChamadosAI_Web
dotnet run
```

Acesse: `https://localhost:7001`

### 5️⃣ Executar o Aplicativo MAUI

**Windows Desktop:**
```bash
cd GestaoChamadosAI_MAUI
dotnet build -f net9.0-windows10.0.19041.0
dotnet run -f net9.0-windows10.0.19041.0
```

**Android:**
```bash
cd GestaoChamadosAI_MAUI
dotnet build -f net9.0-android
```

## 👥 Perfis de Usuário

### 🔵 Cliente
- Criar novos chamados
- Acompanhar status dos seus chamados
- Conversar via chat com o suporte
- Visualizar histórico

### 🟢 Suporte
- Visualizar todos os chamados
- Atender e responder chamados
- Alterar status e prioridade
- Receber sugestões da IA

### 🔴 Administrador
- Acesso completo ao sistema
- Gerenciar usuários
- Gerar relatórios
- Visualizar estatísticas gerais

## 📊 Funcionalidades por Plataforma

| Funcionalidade | Web | Desktop | Mobile |
|---|:---:|:---:|:---:|
| Login/Autenticação | ✅ | ✅ | ✅ |
| Dashboard | ✅ | ✅ | ✅ |
| Criar Chamado | ✅ | ✅ | ✅ |
| Lista de Chamados | ✅ | ✅ | ✅ |
| Chat em Tempo Real | ✅ | ✅ | ✅ |
| Análise de IA | ✅ | ✅ | ✅ |
| Anexar Arquivos | ✅ | ✅ | ✅ |
| Relatórios PDF | ✅ | ⚠️ | ⚠️ |
| Gerenciar Usuários | ✅ | ❌ | ❌ |

✅ Implementado | ⚠️ Parcial | ❌ Não disponível

## 🔐 Segurança

- **JWT (JSON Web Tokens)** para autenticação
- **Refresh Tokens** para renovação automática
- **BCrypt** para hash de senhas
- **Rate Limiting** para proteção contra abuso
- **HTTPS** obrigatório em produção
- **Validação de entrada** em todos os endpoints

## 📱 Capturas de Tela

### Aplicação Web
- Dashboard com estatísticas em tempo real
- Interface responsiva e moderna
- Chat integrado para suporte

### Desktop (Windows)
- Aplicativo nativo para Windows
- Performance otimizada
- Experiência fluida

### Mobile (Android)
- Interface touch-friendly
- Notificações push (futuro)
- Acesso offline limitado (futuro)

## 🧪 Testes

Para executar os testes da API:

```bash
cd TesteAPI_Console
dotnet run
```

## 📚 Documentação da API

Com a API em execução, acesse:

- **Swagger UI**: `https://localhost:7296/swagger`
- **OpenAPI JSON**: `https://localhost:7296/swagger/v1/swagger.json`

### Principais Endpoints

```
POST   /api/auth/login              # Autenticação
POST   /api/auth/refresh-token      # Renovar token

GET    /api/chamados                # Listar chamados
POST   /api/chamados                # Criar chamado
GET    /api/chamados/{id}           # Detalhes do chamado
PUT    /api/chamados/{id}           # Atualizar chamado

GET    /api/chat/{chamadoId}        # Mensagens do chat
POST   /api/chat/{chamadoId}        # Enviar mensagem

GET    /api/dashboard/stats         # Estatísticas do dashboard
GET    /api/relatorios/pdf/{id}     # Gerar relatório PDF
```

## 🤝 Contribuindo

Este projeto foi desenvolvido como Projeto Integrado Multidisciplinar referente ao 4º Semestre.

## 📝 Licença

Este projeto é de propriedade acadêmica e foi desenvolvido para fins educacionais.

## 👨‍💻 Autores

Desenvolvido como Projeto Integrado Multidisciplinar referente ao 4º Semestre.
Ari Modesto Neto
Lucas zanetti gil
Lúcio Guerra da Silva
Luis André Ozeas Azarias
Pedro Vinicius Tinti Poli
Rodrigo Augusto Soares Lopes

## 🎓 Instituição

Projeto desenvolvido como Projeto Integrado Multidisciplinar referente ao 4º Semestre na Instituição UNIP.

---

## 📞 Suporte

Para dúvidas sobre o projeto, consulte a documentação individual de cada módulo:

- [API REST](./GestaoChamadosAI_API/README.md)
- [Aplicação Web](./GestaoChamadosAI_Web/DOCUMENTACAO.md)
- [Aplicativo MAUI](./GestaoChamadosAI_MAUI/README.md)

---
