# 🚀 Gestão de Chamados AI - API REST

API REST completa para sistema de gestão de chamados de suporte técnico integrado com **Google Gemini AI**.

## 📋 Índice

- [Tecnologias](#tecnologias)
- [Requisitos](#requisitos)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Executando a API](#executando-a-api)
- [Documentação da API](#documentação-da-api)
- [Autenticação](#autenticação)
- [Endpoints](#endpoints)
- [Estrutura do Projeto](#estrutura-do-projeto)

## 🛠️ Tecnologias

- **.NET 9.0**
- **ASP.NET Core Web API**
- **Entity Framework Core** (SQL Server)
- **JWT Authentication** (Bearer Token)
- **Google Gemini AI** (Categorização e respostas automáticas)
- **BCrypt** (Hash de senhas)
- **Swagger/OpenAPI** (Documentação interativa)
- **Serilog** (Logging estruturado)
- **AspNetCoreRateLimit** (Limitação de requisições)
- **iText7** (Geração de PDFs)

## ✅ Requisitos

- **.NET SDK 9.0** ou superior
- **SQL Server** (LocalDB ou SQL Server Express)
- **Visual Studio 2022** ou **VS Code**
- **Postman** (opcional, para testes)

## 📦 Instalação

1. **Clone o repositório:**
```bash
cd c:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_API
```

2. **Restaure os pacotes NuGet:**
```powershell
dotnet restore
```

3. **Configure a connection string no `appsettings.json`:**
```json
{
  "ConnectionStrings": {
    "ConexaoPadrao": "Server=localhost;Database=GestaoChamadosAI;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

4. **Execute as migrations (se necessário):**
```powershell
dotnet ef database update
```

## ⚙️ Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "ConexaoPadrao": "SUA_CONNECTION_STRING_AQUI"
  },
  "JwtSettings": {
    "SecretKey": "SuaChaveSecretaSuperSeguraComMinimoDeCaracteres32BytesOuMais!",
    "Issuer": "GestaoChamadosAI_API",
    "Audience": "GestaoChamadosAI_Clients",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "GeminiAI": {
    "ApiKey": "SUA_API_KEY_GOOGLE_GEMINI",
    "Model": "gemini-2.0-flash"
  }
}
```

### Variáveis de Ambiente (Recomendado para Produção)

```bash
export JWT_SECRET_KEY="sua_chave_secreta_aqui"
export GEMINI_API_KEY="sua_api_key_gemini"
```

## 🚀 Executando a API

### Modo Desenvolvimento
```powershell
dotnet run
```

### Modo Produção
```powershell
dotnet run --environment Production
```

A API estará disponível em:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001
- **Swagger:** http://localhost:5000 (raiz)

## 📖 Documentação da API

A documentação interativa está disponível via **Swagger UI**:

**URL:** http://localhost:5000

No Swagger você pode:
- ✅ Ver todos os endpoints disponíveis
- ✅ Testar requisições diretamente no navegador
- ✅ Ver modelos de request/response
- ✅ Autenticar com JWT token

## 🔐 Autenticação

A API usa **JWT (JSON Web Token)** para autenticação.

### 1. Fazer Login

**Endpoint:** `POST /api/auth/login`

**Request Body:**
```json
{
  "email": "usuario@exemplo.com",
  "senha": "sua_senha"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login realizado com sucesso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64_refresh_token",
    "expiresAt": "2025-11-05T15:30:00Z",
    "usuario": {
      "id": 1,
      "nome": "João Silva",
      "email": "usuario@exemplo.com",
      "tipo": "Cliente",
      "dataCadastro": "2025-01-01T00:00:00Z"
    }
  }
}
```

### 2. Usar o Token

Em todas as requisições protegidas, adicione o header:

```
Authorization: Bearer {seu_token_aqui}
```

### 3. Validar Token

**Endpoint:** `GET /api/auth/validate`

Verifica se o token ainda é válido.

## 📡 Endpoints Principais

### Autenticação (`/api/auth`)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| POST | `/login` | Fazer login | ❌ |
| GET | `/profile` | Obter perfil do usuário logado | ✅ |
| POST | `/logout` | Fazer logout | ✅ |
| GET | `/validate` | Validar token | ✅ |

### Usuários (`/api/usuarios`)

| Método | Endpoint | Descrição | Roles |
|--------|----------|-----------|-------|
| GET | `/` | Listar usuários (paginado) | Admin, Suporte |
| GET | `/{id}` | Obter usuário por ID | Admin, Suporte |
| POST | `/` | Criar novo usuário | Admin |
| PUT | `/{id}` | Atualizar usuário | Admin |
| DELETE | `/{id}` | Excluir usuário | Admin |
| GET | `/tipo/{tipo}` | Listar por tipo | Admin, Suporte |

### Chamados (`/api/chamados`)

| Método | Endpoint | Descrição | Roles |
|--------|----------|-----------|-------|
| GET | `/` | Listar chamados (filtros) | Todos |
| GET | `/{id}` | Obter chamado detalhado | Todos |
| POST | `/` | Criar chamado (com IA) | Cliente, Admin |
| PUT | `/{id}` | Atualizar chamado | Suporte, Admin |
| DELETE | `/{id}` | Excluir chamado | Admin |
| POST | `/{id}/feedback` | Registrar feedback da IA | Cliente |
| POST | `/{id}/gerar-resposta-ia` | Gerar resposta IA | Suporte, Admin |
| POST | `/{id}/transferir` | Transferir para outro suporte | Suporte, Admin |
| POST | `/{id}/reassumir` | Reassumir chamado | Suporte, Admin |
| POST | `/{id}/finalizar` | Finalizar chamado | Cliente |
| GET | `/meus` | Listar meus chamados | Todos |
| POST | `/sugestao-ia` | Obter sugestão em tempo real | Todos |

### Chat (`/api/chat`)

| Método | Endpoint | Descrição | Roles |
|--------|----------|-----------|-------|
| GET | `/{chamadoId}` | Obter chat completo | Todos |
| POST | `/{chamadoId}/mensagens` | Enviar mensagem | Todos |
| GET | `/{chamadoId}/mensagens/novas` | Buscar novas mensagens | Todos |
| POST | `/{chamadoId}/assumir` | Assumir atendimento | Suporte, Admin |
| POST | `/{chamadoId}/finalizar` | Finalizar atendimento | Suporte, Admin |

### Dashboard (`/api/dashboard`)

| Método | Endpoint | Descrição | Roles |
|--------|----------|-----------|-------|
| GET | `/estatisticas` | Estatísticas gerais | Todos |
| GET | `/meus-chamados` | Meus chamados (dashboard) | Todos |
| GET | `/chamados-suporte` | Chamados do suporte | Suporte, Admin |

## 📁 Estrutura do Projeto

```
GestaoChamadosAI_API/
├── Controllers/           # Controllers da API
│   ├── AuthController.cs
│   ├── UsuariosController.cs
│   ├── ChamadosController.cs
│   ├── ChatController.cs
│   ├── DashboardController.cs
│   └── RelatoriosController.cs
├── DTOs/                  # Data Transfer Objects
│   ├── Auth/
│   ├── Usuarios/
│   ├── Chamados/
│   ├── Chat/
│   └── Relatorios/
├── Models/                # Modelos de domínio
│   ├── Usuario.cs
│   ├── Chamado.cs
│   └── MensagemChamado.cs
├── Data/                  # Contexto do banco
│   └── AppDbContext.cs
├── Services/              # Serviços de negócio
│   ├── AuthService.cs
│   ├── PasswordHashService.cs
│   ├── IAService.cs
│   └── GeminiService.cs
├── Middleware/            # Middleware customizado
│   └── ErrorHandlingMiddleware.cs
├── Helpers/               # Classes auxiliares
│   ├── ApiResponse.cs
│   ├── PagedResult.cs
│   └── JwtSettings.cs
├── Program.cs             # Configuração da aplicação
├── appsettings.json       # Configurações
└── GestaoChamadosAI_API.csproj
```

## 🧪 Testando a API

### Com Swagger

1. Acesse http://localhost:5000
2. Clique em "Authorize" no canto superior direito
3. Faça login em `/api/auth/login`
4. Copie o token retornado
5. Cole no campo "Value" como: `Bearer {seu_token}`
6. Teste os endpoints!

### Com Postman

Importe a collection do Postman (em breve) ou crie manualmente:

**1. Login:**
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "admin@teste.com",
  "senha": "123456"
}
```

**2. Criar Chamado:**
```http
POST http://localhost:5000/api/chamados
Authorization: Bearer {seu_token}
Content-Type: application/json

{
  "titulo": "Problema no login",
  "descricao": "Não consigo acessar o sistema"
}
```

## 🔒 Segurança Implementada

- ✅ **JWT Authentication** com expiração configurável
- ✅ **BCrypt** para hash de senhas (12 rounds)
- ✅ **Rate Limiting** (60 req/min geral, 5 req/min login)
- ✅ **CORS** configurado
- ✅ **HTTPS** redirect
- ✅ **Autorização baseada em Roles**
- ✅ **Validação de input** (Data Annotations)
- ✅ **Logging estruturado** (Serilog)
- ✅ **Tratamento centralizado de erros**

## 📊 Rate Limiting

Configurado em `appsettings.json`:

- **Geral:** 60 requisições por minuto
- **Login:** 5 requisições por minuto
- **IA:** 10 requisições por minuto

## 🤖 Integração com IA

A API integra com **Google Gemini AI** para:

1. **Categorização automática** de chamados
2. **Análise de prioridade** (Baixa, Média, Alta)
3. **Geração de respostas automáticas**
4. **Sugestões em tempo real**

### Fallback

Se o Gemini AI falhar, a API usa um sistema de IA baseado em palavras-chave como fallback.

## 📝 Logs

Logs são salvos em `logs/api-{data}.log` com as seguintes informações:

- Requisições HTTP
- Erros e exceções
- Operações de autenticação
- Operações de CRUD
- Chamadas à IA

## 🐛 Troubleshooting

### Erro: "Cannot connect to database"
- Verifique se o SQL Server está rodando
- Valide a connection string
- Execute `dotnet ef database update`

### Erro: "Unauthorized"
- Verifique se o token está correto
- Verifique se o token não expirou
- Certifique-se de usar `Bearer {token}` no header

### Erro: "Gemini API error"
- Verifique se a API Key está configurada
- Verifique conexão com internet
- O sistema usará fallback automaticamente

## 📄 Licença

Este projeto é privado e de uso interno.

## 👥 Equipe

Desenvolvido pela equipe de desenvolvimento da Gestão de Chamados AI.

---

**Data de criação:** 05/11/2025
**Versão:** 1.0.0
