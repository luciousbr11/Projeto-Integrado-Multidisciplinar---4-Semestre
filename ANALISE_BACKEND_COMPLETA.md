# 📋 ANÁLISE COMPLETA DO BACKEND - Sistema de Gestão de Chamados com IA

## 📌 Visão Geral do Sistema

Sistema web ASP.NET Core MVC (.NET 9.0) para gestão de chamados de suporte técnico integrado com **Google Gemini AI** para análise automática, categorização e geração de respostas inteligentes.

---

## 🗄️ 1. ESTRUTURA DE DADOS (Models)

### 1.1 **Usuario** (`Models/Usuario.cs`)
```
Propriedades:
├─ Id (int) - PK
├─ Nome (string)
├─ Email (string) - Único
├─ Senha (string) - Texto plano (⚠️ não hasheada)
├─ Tipo (string) - "Cliente" | "Suporte" | "Administrador"
├─ DataCadastro (DateTime) - Default: DateTime.Now
└─ Chamados (ICollection<Chamado>) - Relação 1:N

Papéis:
├─ Cliente: Abre chamados, acompanha status
├─ Suporte: Atende chamados, usa chat
└─ Administrador: Acesso total + relatórios
```

### 1.2 **Chamado** (`Models/Chamado.cs`)
```
Propriedades:
├─ Id (int) - PK
├─ Titulo (string)
├─ Descricao (string)
├─ DataAbertura (DateTime) - Default: DateTime.Now
├─ Status (string) - "Aberto" | "Em Andamento" | "Concluído" | "Solucionado por IA"
│
├─ CAMPOS DE IA:
│  ├─ CategoriaIA (string) - Categoria automática do Gemini
│  ├─ SugestaoIA (string) - Sugestão do IAService legado
│  ├─ Prioridade (string) - "Baixa" | "Média" | "Alta"
│  ├─ RespostaIA (string) - Resposta automática gerada pelo Gemini
│  ├─ FeedbackResolvido (bool?) - true/false/null
│  └─ DataFeedback (DateTime?)
│
├─ RELACIONAMENTOS:
│  ├─ UsuarioId (int) - FK Cliente que abriu
│  ├─ Usuario (Usuario)
│  ├─ SuporteResponsavelId (int?) - FK Suporte atendendo
│  ├─ SuporteResponsavel (Usuario?)
│  └─ Mensagens (ICollection<MensagemChamado>)
```

### 1.3 **MensagemChamado** (`Models/MensagemChamado.cs`)
```
Propriedades:
├─ Id (int) - PK
├─ ChamadoId (int) - FK
├─ Chamado (Chamado)
├─ UsuarioId (int) - FK Remetente
├─ Usuario (Usuario)
├─ Mensagem (string) - Conteúdo
├─ DataEnvio (DateTime)
├─ LidaPorCliente (bool) - Default: false
└─ LidaPorSuporte (bool) - Default: false

Sistema de Chat:
├─ Mensagens bidirecionais cliente <-> suporte
├─ Marcação de leitura separada
└─ Histórico completo do atendimento
```

---

## 🔌 2. CAMADA DE DADOS (Data)

### 2.1 **AppDbContext** (`Data/AppDbContext.cs`)
```
DbSets:
├─ Usuarios
├─ Chamados
└─ MensagensChamados

Configurações (OnModelCreating):
├─ Chamado -> Usuario: DeleteBehavior.Restrict
├─ Chamado -> SuporteResponsavel: DeleteBehavior.Restrict
├─ MensagemChamado -> Chamado: DeleteBehavior.Cascade
└─ MensagemChamado -> Usuario: DeleteBehavior.Restrict

Connection String:
Server=localhost;
Database=GestaoChamadosAI;
Trusted_Connection=True;
TrustServerCertificate=True;
```

### 2.2 **Migrations**
```
Histórico:
├─ 20251101171532_Inicial - Estrutura básica
├─ 20251101185938_AdicionarCamposIA - CategoriaIA, SugestaoIA, Prioridade
├─ 20251101191508_AdicionarChatSuporte - MensagemChamado
├─ 20251101193551_AdicionarDataCadastroUsuario
├─ 20251101230116_AdicionarRespostaIA - RespostaIA
└─ 20251102001010_AdicionarFeedbackIA - FeedbackResolvido, DataFeedback
```

---

## 🤖 3. SERVIÇOS DE INTELIGÊNCIA ARTIFICIAL

### 3.1 **IAService** (`Services/IAService.cs`) - IA Legada
```
Tipo: Singleton
Método: Análise por palavras-chave

Base de Conhecimento (10 categorias):
├─ Senha/Login/Acesso
├─ Performance/Lentidão
├─ Erros/Bugs
├─ Impressora
├─ E-mail
├─ Rede/Internet
├─ Instalação de Software
├─ Backup/Recuperação
├─ Vídeo/Display
└─ Periféricos (Mouse/Teclado)

Métodos Principais:
├─ AnalisarChamado(titulo, descricao): string
│  └─ Retorna sugestão baseada em palavras-chave
├─ ClassificarPrioridade(titulo, descricao): string
│  └─ Retorna: "Alta" | "Média" | "Baixa"
└─ ObterEstatisticas(): string
   └─ Retorna info da base de conhecimento

Pontuação:
└─ Conta palavras-chave encontradas e retorna melhor match
```

### 3.2 **GeminiService** (`Services/GeminiService.cs`) - IA Principal
```
Tipo: Singleton
Integração: Google Gemini AI via REST API
URL: generativelanguage.googleapis.com

Configuração (appsettings.json):
├─ ApiKey: "AIzaSyAli_1DftyIGb_LCvvQaJZ7Mto4tM8OLZg"
└─ Model: "gemini-2.0-flash"

Sistema de Fallback:
├─ Modelos: ["gemini-2.0-flash", "gemini-2.5-flash", "gemini-pro-latest"]
├─ Versões API: ["v1beta", "v1"]
└─ Tenta todas combinações até sucesso

Métodos Principais:

1. ChamarGeminiApiAsync(prompt): Task<string>
   ├─ Chamada HTTP POST com JSON
   ├─ Tratamento de erros (404, 400, 403)
   ├─ Troca automática de modelo em caso de erro
   └─ Logging detalhado

2. CategorizarChamadoAsync(titulo, descricao): Task<string>
   ├─ Prompt: Analisa e retorna categoria específica
   ├─ Regras: Máximo 3-4 palavras, específica
   └─ Fallback: "Problema Não Identificado"

3. GerarRespostaAsync(titulo, descricao, categoria): Task<string>
   ├─ Prompt: Resposta estruturada em tópicos
   ├─ Formato: Simples, direto, com emojis
   └─ Sem saudações formais

4. AnalisarPrioridadeAsync(titulo, descricao): Task<string>
   ├─ Retorna: "Baixa" | "Média" | "Alta"
   └─ Fallback: "Média"

5. TestarConexaoAsync(): Task<bool>
   └─ Valida conectividade com API
```

---

## 🎮 4. CONTROLLERS - FLUXO COMPLETO

### 4.1 **AccountController** - Autenticação

```
📍 POST /Account/Login
├─ Input: email, senha, returnUrl?
├─ Validação:
│  ├─ Busca usuário no banco
│  └─ ⚠️ Compara senha em texto plano
├─ Autenticação:
│  ├─ Cria Claims: NameIdentifier, Name, Email, Role
│  ├─ Cookie: HttpOnly, ExpireTimeSpan: 8h, SlidingExpiration
│  └─ SignInAsync(CookieAuthenticationDefaults)
└─ Redirect: returnUrl || /Dashboard/Index

📍 POST /Account/Logout
├─ SignOutAsync(CookieAuthenticationDefaults)
└─ Redirect: /Account/Login

📍 GET /Account/AccessDenied
└─ Exibe página de acesso negado

Segurança:
├─ [AllowAnonymous] em Login
├─ [Authorize] em Logout/Profile
└─ [ValidateAntiForgeryToken] em POSTs
```

### 4.2 **ChamadosController** - Gerenciamento de Chamados

```
📍 GET /Chamados/Index
├─ Filtros opcionais: status, suporteId, prioridade
├─ Lógica de permissões:
│  ├─ Cliente: Vê apenas seus chamados
│  └─ Suporte/Admin: Vê todos
├─ Query:
│  ├─ Include(Usuario, SuporteResponsavel)
│  ├─ Filtros aplicados
│  └─ OrderByDescending(DataAbertura)
└─ ViewBag: StatusFiltro, Suportes, PrioridadeFiltro

📍 GET /Chamados/Details/{id}
├─ Include: Usuario, SuporteResponsavel
├─ ViewBag:
│  ├─ Suportes (para transferência)
│  └─ UsuarioAtualId
└─ View com detalhes completos

📍 POST /Chamados/Create
├─ Input: Titulo, Descricao
├─ Validações:
│  ├─ Obtém UsuarioId das Claims
│  └─ Cliente: Valida se não tem chamado em aberto
├─ Processamento IA:
│  ├─ IAService.AnalisarChamado() - Sugestão legada
│  ├─ GeminiService.CategorizarChamadoAsync() - Categoria
│  ├─ GeminiService.AnalisarPrioridadeAsync() - Prioridade
│  └─ GeminiService.GerarRespostaAsync() - Resposta automática
├─ Try-Catch: Fallback para métodos legados
├─ Salva no banco
└─ Redirect: /Chamados/Feedback/{id}

📍 GET /Chamados/Feedback/{id}
├─ Exibe RespostaIA para o cliente
└─ Pergunta se resolveu o problema

📍 POST /Chamados/ProcessarFeedback
├─ Input: id, resolvido (bool)
├─ Atualiza:
│  ├─ FeedbackResolvido = resolvido
│  ├─ DataFeedback = DateTime.Now
│  └─ Status:
│     ├─ true → "Solucionado por IA"
│     └─ false → "Aberto"
└─ Redirect: /Chamados/Index

📍 POST /Chamados/Edit/{id}
├─ Input: Todos campos do chamado
├─ Validações:
│  ├─ Verifica se pode alterar status
│  └─ Não permite alterar status sem SuporteResponsavelId
├─ Atualiza e salva
└─ Redirect: /Chamados/Details/{id}

📍 POST /Chamados/Delete/{id}
├─ Remove chamado (e mensagens em cascade)
└─ Redirect: /Chamados/Index

📍 POST /Chamados/ObterSugestaoIA (AJAX)
├─ Input: titulo, descricao
├─ Retorna JSON:
│  ├─ sugestao (IAService)
│  └─ prioridade
└─ Usado em tempo real no formulário

📍 POST /Chamados/GerarRespostaIA
├─ Regenera RespostaIA usando Gemini
├─ Atualiza chamado
└─ Redirect: /Chamados/Details/{id}

📍 POST /Chamados/TransferirChamado
├─ [Authorize(Roles = "Suporte,Administrador")]
├─ Input: chamadoId, novoSuporteId
├─ Validações:
│  ├─ Não permite se Status = "Concluído" ou "Solucionado por IA"
│  └─ Valida se novo suporte existe e é do tipo "Suporte"
├─ Atualiza:
│  ├─ SuporteResponsavelId = novoSuporteId
│  └─ Status = "Aberto"
├─ Cria mensagem sistema: "🔄 Transferido de X para Y"
└─ Redirect: /Chamados/Details/{id}

📍 POST /Chamados/Reassumir
├─ Permite suporte reassumir chamado transferido
├─ Atualiza SuporteResponsavelId
├─ Cria mensagem: "🔁 Reassumido por X"
└─ Redirect: /Chamados/Details/{id}

📍 POST /Chamados/FinalizarChamado
├─ [Authorize(Roles = "Cliente")]
├─ Validações:
│  ├─ Verifica se chamado pertence ao cliente
│  └─ Permite apenas se Status = "Aberto" ou "Em Andamento"
├─ Status = "Concluído"
└─ Redirect: /Chamados/Index

📍 GET /Chamados/TestarGemini (DEBUG)
├─ Chama GeminiService.TestarConexaoAsync()
└─ Retorna resultado texto
```

### 4.3 **ChatController** - Sistema de Mensagens

```
📍 GET /Chat/Index/{chamadoId}
├─ Include: Usuario, SuporteResponsavel, Mensagens.Usuario
├─ Validações de acesso:
│  ├─ Cliente: Só seu chamado
│  └─ Suporte: Só se for SuporteResponsavelId
├─ Marca mensagens como lidas:
│  ├─ Cliente: LidaPorCliente = true
│  └─ Suporte: LidaPorSuporte = true
├─ ViewBag:
│  ├─ UsuarioAtualId
│  ├─ UsuarioRole
│  └─ Suportes (para transferência)
└─ View com chat completo

📍 POST /Chat/AssumirAtendimento
├─ [Authorize(Roles = "Suporte,Administrador")]
├─ Input JSON: { ChamadoId: int }
├─ Validações:
│  ├─ Usuário é Suporte/Admin
│  └─ Chamado não está "Concluído" ou "Solucionado por IA"
├─ Atualiza:
│  ├─ SuporteResponsavelId = userId
│  └─ Status = "Em Andamento"
├─ Mensagem sistema:
│  ├─ Se já tinha suporte: "⚡ Assumido por Admin"
│  └─ Se primeiro: "📢 Atendimento iniciado"
└─ Retorna JSON: { success: bool, message: string }

📍 POST /Chat/EnviarMensagem
├─ Input: chamadoId, mensagem
├─ Validações:
│  ├─ Mensagem não vazia
│  ├─ Cliente: Só seu chamado
│  └─ Suporte: Só se for SuporteResponsavelId
├─ Cria nova mensagem:
│  ├─ LidaPorCliente = (userRole != "Cliente")
│  └─ LidaPorSuporte = (userRole é Suporte/Admin)
└─ Retorna JSON com dados da mensagem

📍 GET /Chat/BuscarNovasMensagens (AJAX Polling)
├─ Input: chamadoId, ultimaMensagemId
├─ Busca mensagens com Id > ultimaMensagemId
├─ OrderBy(DataEnvio)
└─ Retorna JSON: { success: bool, mensagens: [] }

📍 POST /Chat/FinalizarAtendimento
├─ [Authorize(Roles = "Suporte,Administrador")]
├─ Status = "Concluído"
└─ Retorna JSON com redirectUrl

Sistema de Chat:
├─ Polling automático (JavaScript)
├─ Marcação de leitura bidirecional
├─ Suporte de emojis
└─ Histórico completo persistido
```

### 4.4 **DashboardController** - Painéis por Perfil

```
📍 GET /Dashboard/Index
├─ Detecta Role do usuário
└─ Redireciona:
   ├─ "Administrador" → /Dashboard/Administrador
   ├─ "Suporte" → /Dashboard/Suporte
   └─ "Cliente" → /Dashboard/Cliente

📍 GET /Dashboard/Administrador
├─ [Authorize(Roles = "Administrador")]
├─ Métricas:
│  ├─ TotalChamados
│  ├─ ChamadosAbertos
│  ├─ ChamadosEmAndamento
│  ├─ ChamadosResolvidos (incluindo "Solucionado por IA")
│  └─ ChamadosSolucionadosIA
├─ Dados:
│  └─ ChamadosRecentes (últimos 15)
└─ ViewBag com todas métricas

📍 GET /Dashboard/Suporte
├─ [Authorize(Roles = "Suporte,Administrador")]
├─ Métricas Gerais:
│  ├─ ChamadosPendentes (Status = "Aberto")
│  ├─ ChamadosEmAndamento
│  └─ ChamadosResolvidosHoje
├─ Dados Personalizados:
│  ├─ MeusChamados (SuporteResponsavelId = userId)
│  ├─ ChamadosTransferidos (enviou msg mas não é mais responsável)
│  ├─ ChamadosAlta (Prioridade alta aguardando)
│  └─ ChamadosAbertos (todos abertos, Take 15)
└─ ViewBag com listas

📍 GET /Dashboard/Cliente
├─ [Authorize(Roles = "Cliente")]
├─ Filtro: UsuarioId = userId
├─ Métricas:
│  ├─ TotalChamados
│  ├─ ChamadosAbertos
│  ├─ ChamadosEmAndamento
│  └─ ChamadosResolvidos
├─ Dados:
│  ├─ ChamadosAtivos (Take 10)
│  └─ ChamadosResolvidosRecentes (Take 10)
└─ ViewBag com dados
```

### 4.5 **UsuariosController** - CRUD de Usuários

```
📍 GET /Usuarios/Index
├─ OrderBy(Nome)
└─ Lista todos usuários

📍 GET /Usuarios/FiltrarPorTipo?tipo={tipo}
├─ Filtra por Tipo
└─ Retorna View("Index")

📍 GET /Usuarios/Details/{id}
├─ Include(Chamados)
└─ Detalhes do usuário

📍 POST /Usuarios/Create
├─ Input: Nome, Email, Senha, Tipo
├─ Validações:
│  ├─ ModelState.IsValid
│  └─ Email único (verifica duplicidade)
└─ Redirect: /Usuarios/Index

📍 POST /Usuarios/Edit/{id}
├─ Input: Todos campos
├─ Validações:
│  ├─ Id corresponde
│  └─ Email único (exceto próprio)
├─ Try-Catch: DbUpdateConcurrencyException
└─ Redirect: /Usuarios/Index

📍 POST /Usuarios/Delete/{id}
├─ Include(Chamados)
├─ Remove:
│  ├─ Chamados do usuário (RemoveRange)
│  └─ Usuario
└─ Redirect: /Usuarios/Index

⚠️ Segurança:
└─ Sem [Authorize] - ABERTO PARA TODOS
```

### 4.6 **RelatoriosController** - Relatórios e PDFs

```
📍 GET /Relatorios/Index
├─ [Authorize(Roles = "Administrador")]
└─ Página com cards de acesso aos relatórios

📍 GET /Relatorios/UsuariosCadastrados
├─ Query:
│  ├─ Select com TotalChamados calculado
│  └─ OrderBy(Nome)
├─ Model:
│  ├─ Usuarios: List<UsuarioRelatorio>
│  ├─ TotalUsuarios
│  ├─ TotalClientes
│  ├─ TotalSuportes
│  └─ TotalAdministradores
└─ View com tabela

📍 GET /Relatorios/ChamadosPorPeriodo
├─ Input: dataInicio?, dataFim?
├─ Default: Últimos 30 dias
├─ Query:
│  ├─ Ajusta hora: inicio.Date, fim.Date+1d-1s
│  ├─ Include(Usuario)
│  └─ Where(DataAbertura between)
├─ Model:
│  ├─ ChamadosPorPeriodo: List<ChamadoRelatorio>
│  ├─ DataInicio, DataFim
│  ├─ TotalChamadosPeriodo
│  ├─ ChamadosAbertos
│  ├─ ChamadosEmAndamento
│  └─ ChamadosConcluidos
└─ View com gráficos/tabelas

📍 GET /Relatorios/ChamadosPorSuporte
├─ Query:
│  ├─ Where(Tipo = "Suporte" ou "Administrador")
│  └─ Select com agregações:
│     ├─ TotalChamados: Count
│     ├─ ChamadosAbertos: Count(Status)
│     ├─ ChamadosEmAndamento: Count(Status)
│     ├─ ChamadosConcluidos: Count(Status)
│     ├─ UltimoAtendimento: Max(DataAbertura)
│     └─ TempoMedioResolucao: Avg(DateDiffHour)
├─ Model:
│  ├─ ChamadosPorSuporte: List<SuporteRelatorio>
│  └─ TotalChamadosAtendidos: Sum
└─ View com estatísticas

📍 GET /Relatorios/ImprimirUsuariosPDF
├─ Biblioteca: iText 7
├─ Cria PDF:
│  ├─ Título: "Relatório de Usuários Cadastrados"
│  ├─ Resumo: Totais por tipo
│  └─ Tabela: Id, Nome, Email, Tipo, Data, Chamados
└─ Return: File(pdf, "application/pdf", "usuarios_{data}.pdf")

📍 GET /Relatorios/ImprimirChamadosPeriodoPDF
├─ Input: dataInicio?, dataFim?
├─ Cria PDF:
│  ├─ Título: "Relatório de Chamados por Período"
│  ├─ Período: {inicio} - {fim}
│  ├─ Resumo: Status counts
│  └─ Tabela: Id, Título, Status, Prioridade, Categoria, Data
└─ Return: File(pdf, "application/pdf", "chamados_{datas}.pdf")

📍 GET /Relatorios/ImprimirChamadosSuportePDF
├─ Cria PDF:
│  ├─ Título: "Relatório de Chamados por Suporte"
│  ├─ Resumo: Total atendidos
│  └─ Tabela: Suporte, Total, Abertos, Em And., Concl., Último
└─ Return: File(pdf, "application/pdf", "suportes_{data}.pdf")

Dependência:
└─ iText.Kernel.Pdf + iText.Layout
```

---

## ⚙️ 5. CONFIGURAÇÃO DO SISTEMA (Program.cs)

```csharp
Serviços Registrados:
├─ AddControllersWithViews() - MVC
├─ AddHttpClient() - Para requisições HTTP
├─ AddDbContext<AppDbContext>(SQL Server)
├─ AddSingleton<IAService>() - IA Legada
├─ AddSingleton<GeminiService>() - IA Principal
└─ AddAuthentication(CookieAuthenticationDefaults)
   └─ AddCookie:
      ├─ LoginPath: /Account/Login
      ├─ LogoutPath: /Account/Logout
      ├─ AccessDeniedPath: /Account/AccessDenied
      ├─ ExpireTimeSpan: 8 horas
      ├─ SlidingExpiration: true
      ├─ HttpOnly: true
      └─ IsEssential: true

Middleware Pipeline:
1. ExceptionHandler (se !Development)
2. UseHttpsRedirection
3. UseStaticFiles
4. UseRouting
5. UseAuthentication ⚠️ ANTES de Authorization
6. UseAuthorization
7. MapControllerRoute (default: Account/Login)

Rota Padrão:
{controller=Account}/{action=Login}/{id?}
```

---

## 🔐 6. SEGURANÇA E AUTENTICAÇÃO

### 6.1 Sistema de Autenticação
```
Método: Cookie-Based Authentication
Claims armazenadas:
├─ ClaimTypes.NameIdentifier → Usuario.Id
├─ ClaimTypes.Name → Usuario.Nome
├─ ClaimTypes.Email → Usuario.Email
└─ ClaimTypes.Role → Usuario.Tipo

Cookie Settings:
├─ HttpOnly: true - Protege contra XSS
├─ IsEssential: true - GDPR compliance
├─ ExpireTimeSpan: 8h
└─ SlidingExpiration: true - Renova automaticamente
```

### 6.2 Atributos de Autorização
```
[AllowAnonymous]
└─ Login, AccessDenied

[Authorize]
└─ Todos os controllers exceto Account

[Authorize(Roles = "Administrador")]
└─ RelatoriosController inteiro

[Authorize(Roles = "Suporte,Administrador")]
├─ Chat/AssumirAtendimento
├─ Chat/FinalizarAtendimento
├─ Chamados/TransferirChamado
└─ Chamados/Reassumir

[Authorize(Roles = "Cliente")]
└─ Chamados/FinalizarChamado
```

### 6.3 ⚠️ VULNERABILIDADES IDENTIFICADAS
```
CRÍTICO:
├─ Senhas em texto plano (sem hash)
├─ UsuariosController sem [Authorize]
└─ API Key do Gemini exposta no appsettings.json

ALTO:
├─ Sem rate limiting nas requisições de IA
├─ Sem validação de tamanho de arquivos/mensagens
└─ SQL Injection mitigado apenas pelo EF Core

MÉDIO:
├─ Sem log de ações de auditoria
├─ Sem 2FA (Two-Factor Authentication)
└─ Sem controle de sessões simultâneas
```

---

## 📊 7. FLUXOS DE NEGÓCIO COMPLETOS

### 7.1 **Fluxo: Cliente Abre Chamado**
```
1. Cliente faz login
   ├─ POST /Account/Login
   └─ Claims criadas + Cookie

2. Acessa Dashboard
   ├─ GET /Dashboard/Index
   └─ Redireciona para /Dashboard/Cliente

3. Cria novo chamado
   ├─ GET /Chamados/Create
   ├─ Preenche Titulo + Descricao
   └─ POST /Chamados/Create
      ├─ Validação: Não pode ter chamado em aberto
      ├─ IAService.AnalisarChamado() - Sugestão
      ├─ GeminiService.CategorizarChamadoAsync() - Categoria
      ├─ GeminiService.AnalisarPrioridadeAsync() - Prioridade
      ├─ GeminiService.GerarRespostaAsync() - Resposta automática
      ├─ Salva: Status = "Aberto"
      └─ Redirect: /Chamados/Feedback/{id}

4. Visualiza resposta da IA
   ├─ GET /Chamados/Feedback/{id}
   ├─ Cliente lê RespostaIA
   └─ Escolhe:
      A) POST /Chamados/ProcessarFeedback?resolvido=true
         ├─ Status = "Solucionado por IA"
         ├─ FeedbackResolvido = true
         └─ TempData: "Ótimo! 🎉"
      
      B) POST /Chamados/ProcessarFeedback?resolvido=false
         ├─ Status = "Aberto"
         ├─ FeedbackResolvido = false
         └─ Aguarda suporte humano

5. (Se não resolvido) Aguarda suporte assumir
```

### 7.2 **Fluxo: Suporte Atende Chamado**
```
1. Suporte faz login
   └─ Redireciona para /Dashboard/Suporte

2. Visualiza chamados disponíveis
   ├─ ViewBag.ChamadosAbertos
   └─ Filtra por prioridade/categoria

3. Acessa chamado
   ├─ GET /Chamados/Details/{id}
   └─ Vê histórico + RespostaIA + Dados cliente

4. Inicia atendimento
   ├─ Clica "Assumir Atendimento"
   └─ POST /Chat/AssumirAtendimento (AJAX)
      ├─ SuporteResponsavelId = userId
      ├─ Status = "Em Andamento"
      └─ Mensagem: "📢 Atendimento iniciado"

5. Usa sistema de chat
   ├─ GET /Chat/Index/{chamadoId}
   ├─ POST /Chat/EnviarMensagem
   │  └─ Mensagens bidirecionais
   ├─ GET /Chat/BuscarNovasMensagens (polling)
   └─ Marca mensagens como lidas

6. Opções durante atendimento:
   A) Transferir para outro suporte
      ├─ POST /Chamados/TransferirChamado
      ├─ Status = "Aberto"
      ├─ SuporteResponsavelId = novoSuporteId
      └─ Mensagem: "🔄 Transferido de X para Y"
   
   B) Alterar status manualmente
      └─ POST /Chamados/Edit/{id}
   
   C) Regenerar resposta IA
      └─ POST /Chamados/GerarRespostaIA

7. Finaliza atendimento
   ├─ POST /Chat/FinalizarAtendimento
   └─ Status = "Concluído"
```

### 7.3 **Fluxo: Administrador Gera Relatórios**
```
1. Admin faz login
   └─ Redireciona para /Dashboard/Administrador

2. Acessa Relatórios
   └─ GET /Relatorios/Index

3. Gera relatório de usuários
   ├─ GET /Relatorios/UsuariosCadastrados
   ├─ Visualiza dados na tela
   └─ GET /Relatorios/ImprimirUsuariosPDF
      └─ Download: usuarios_{data}.pdf

4. Gera relatório de chamados por período
   ├─ GET /Relatorios/ChamadosPorPeriodo?dataInicio=...&dataFim=...
   ├─ Visualiza filtrado
   └─ GET /Relatorios/ImprimirChamadosPeriodoPDF
      └─ Download: chamados_{datas}.pdf

5. Gera relatório de performance de suportes
   ├─ GET /Relatorios/ChamadosPorSuporte
   └─ GET /Relatorios/ImprimirChamadosSuportePDF
      └─ Download: suportes_{data}.pdf
```

---

## 🔄 8. INTEGRAÇÕES E DEPENDÊNCIAS

### 8.1 Pacotes NuGet Necessários
```
Microsoft.EntityFrameworkCore (>= 9.0)
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Authentication.Cookies
iText7 (PDF generation)
System.Text.Json
```

### 8.2 Integrações Externas
```
Google Gemini AI:
├─ Endpoint: https://generativelanguage.googleapis.com
├─ Autenticação: API Key via query string
├─ Rate Limit: Não implementado (⚠️)
└─ Timeout: Padrão do HttpClient

SQL Server:
├─ Versão: Compatível com LocalDB
├─ Trusted_Connection: True (Windows Auth)
└─ TrustServerCertificate: True (dev only)
```

---

## 📈 9. MÉTRICAS E ESTATÍSTICAS COLETADAS

### 9.1 Dashboard Administrador
```
- Total de Chamados
- Chamados Abertos
- Chamados Em Andamento
- Chamados Resolvidos (incluindo IA)
- Chamados Solucionados por IA (específico)
- Chamados Recentes (últimos 15)
```

### 9.2 Dashboard Suporte
```
- Chamados Pendentes (globais)
- Chamados Em Andamento (globais)
- Chamados Resolvidos Hoje
- Meus Chamados (atribuídos a mim)
- Chamados Transferidos (que já atendi)
- Chamados Alta Prioridade (aguardando)
- Todos Chamados Abertos (15 primeiros)
```

### 9.3 Dashboard Cliente
```
- Total de Chamados (meus)
- Chamados Abertos (meus)
- Chamados Em Andamento (meus)
- Chamados Resolvidos (meus)
- Chamados Ativos (10 primeiros)
- Chamados Resolvidos Recentes (10 primeiros)
```

### 9.4 Relatórios
```
Usuários:
├─ Total por tipo
└─ Chamados por usuário

Chamados por Período:
├─ Total no período
├─ Distribuição por status
├─ Distribuição por prioridade
└─ Distribuição por categoria IA

Suportes:
├─ Total atendidos
├─ Distribuição por status
├─ Último atendimento
└─ Tempo médio de resolução
```

---

## 🎯 10. ENDPOINTS DA API (Para Criação da API REST)

### 10.1 **Authentication** (`/api/auth`)
```
POST   /api/auth/login
       Body: { email, senha }
       Response: { token, usuario: { id, nome, email, tipo } }

POST   /api/auth/logout
       Headers: Authorization: Bearer {token}
       Response: { success: true }

GET    /api/auth/profile
       Headers: Authorization: Bearer {token}
       Response: Usuario
```

### 10.2 **Usuarios** (`/api/usuarios`)
```
GET    /api/usuarios
       Response: Usuario[]

GET    /api/usuarios/{id}
       Response: Usuario

POST   /api/usuarios
       Body: { nome, email, senha, tipo }
       Response: Usuario

PUT    /api/usuarios/{id}
       Body: { nome, email, senha, tipo }
       Response: Usuario

DELETE /api/usuarios/{id}
       Response: { success: true }

GET    /api/usuarios/tipo/{tipo}
       Response: Usuario[]
```

### 10.3 **Chamados** (`/api/chamados`)
```
GET    /api/chamados
       Query: ?status=...&suporteId=...&prioridade=...
       Response: Chamado[]

GET    /api/chamados/{id}
       Response: Chamado (include Usuario, SuporteResponsavel, Mensagens)

POST   /api/chamados
       Body: { titulo, descricao }
       Process: IA automática
       Response: Chamado

PUT    /api/chamados/{id}
       Body: { titulo, descricao, status }
       Response: Chamado

DELETE /api/chamados/{id}
       Response: { success: true }

POST   /api/chamados/{id}/feedback
       Body: { resolvido: bool }
       Response: Chamado

POST   /api/chamados/{id}/gerar-resposta-ia
       Response: { respostaIA: string }

POST   /api/chamados/{id}/transferir
       Body: { novoSuporteId: int }
       Response: Chamado

POST   /api/chamados/{id}/reassumir
       Response: Chamado

POST   /api/chamados/{id}/finalizar
       Response: Chamado

GET    /api/chamados/meus
       Response: Chamado[] (filtrado por usuário logado)

POST   /api/chamados/sugestao-ia
       Body: { titulo, descricao }
       Response: { sugestao, prioridade, categoria }
```

### 10.4 **Chat** (`/api/chat`)
```
GET    /api/chat/{chamadoId}
       Response: { chamado, mensagens: MensagemChamado[] }

POST   /api/chat/{chamadoId}/mensagens
       Body: { mensagem: string }
       Response: MensagemChamado

GET    /api/chat/{chamadoId}/mensagens/novas
       Query: ?ultimaMensagemId=...
       Response: MensagemChamado[]

POST   /api/chat/{chamadoId}/assumir
       Response: { success: true, mensagem: string }

POST   /api/chat/{chamadoId}/finalizar
       Response: { success: true }

PUT    /api/chat/{chamadoId}/mensagens/{mensagemId}/marcar-lida
       Response: { success: true }
```

### 10.5 **Dashboard** (`/api/dashboard`)
```
GET    /api/dashboard/estatisticas
       Response: {
         totalChamados,
         chamadosAbertos,
         chamadosEmAndamento,
         chamadosResolvidos,
         chamadosSolucionadosIA
       }

GET    /api/dashboard/meus-chamados
       Response: Chamado[]

GET    /api/dashboard/chamados-suporte
       Response: {
         chamadosPendentes,
         chamadosEmAndamento,
         chamadosResolvidosHoje,
         meusChamados,
         chamadosAlta
       }
```

### 10.6 **Relatorios** (`/api/relatorios`)
```
GET    /api/relatorios/usuarios
       Response: {
         usuarios: UsuarioRelatorio[],
         totalUsuarios,
         totalClientes,
         totalSuportes,
         totalAdministradores
       }

GET    /api/relatorios/chamados-periodo
       Query: ?dataInicio=...&dataFim=...
       Response: {
         chamados: ChamadoRelatorio[],
         dataInicio,
         dataFim,
         totalChamadosPeriodo,
         chamadosAbertos,
         chamadosEmAndamento,
         chamadosConcluidos
       }

GET    /api/relatorios/suportes
       Response: {
         suportes: SuporteRelatorio[],
         totalChamadosAtendidos
       }

GET    /api/relatorios/usuarios/pdf
       Response: File (application/pdf)

GET    /api/relatorios/chamados-periodo/pdf
       Query: ?dataInicio=...&dataFim=...
       Response: File (application/pdf)

GET    /api/relatorios/suportes/pdf
       Response: File (application/pdf)
```

### 10.7 **IA** (`/api/ia`)
```
POST   /api/ia/analisar
       Body: { titulo, descricao }
       Response: { sugestao, prioridade }

POST   /api/ia/categorizar
       Body: { titulo, descricao }
       Response: { categoria }

POST   /api/ia/gerar-resposta
       Body: { titulo, descricao, categoria }
       Response: { resposta }

POST   /api/ia/prioridade
       Body: { titulo, descricao }
       Response: { prioridade }

GET    /api/ia/testar
       Response: { sucesso: bool, mensagem: string }
```

---

## 🔧 11. RECOMENDAÇÕES PARA A API

### 11.1 Segurança
```
✅ IMPLEMENTAR:
├─ JWT Authentication (em vez de Cookies)
├─ Hashing de senhas (BCrypt, Argon2, Identity)
├─ Rate Limiting (AspNetCoreRateLimit)
├─ CORS configurado corretamente
├─ Validação de input (FluentValidation)
├─ API Key do Gemini em User Secrets ou Azure Key Vault
└─ Logs de auditoria (Serilog)

✅ ADICIONAR:
├─ Refresh Tokens
├─ Token Expiration (15-60 min)
├─ IP Whitelisting (opcional)
└─ Proteção contra XSS/CSRF em APIs REST
```

### 11.2 Arquitetura
```
✅ ESTRUTURA SUGERIDA:
GestaoChamadosAI_API/
├─ Controllers/
│  ├─ AuthController
│  ├─ UsuariosController
│  ├─ ChamadosController
│  ├─ ChatController
│  ├─ DashboardController
│  ├─ RelatoriosController
│  └─ IAController
├─ DTOs/
│  ├─ Auth/
│  ├─ Usuarios/
│  ├─ Chamados/
│  └─ Chat/
├─ Services/
│  ├─ IAService
│  ├─ GeminiService
│  ├─ AuthService
│  └─ RelatorioService
├─ Repositories/
│  ├─ IUsuarioRepository
│  ├─ IChamadoRepository
│  └─ IMensagemRepository
├─ Data/
│  └─ AppDbContext
├─ Models/
│  ├─ Usuario
│  ├─ Chamado
│  └─ MensagemChamado
└─ Middleware/
   ├─ JwtMiddleware
   ├─ ErrorHandlingMiddleware
   └─ RateLimitMiddleware

✅ PADRÕES:
├─ Repository Pattern
├─ Unit of Work
├─ DTOs para input/output
├─ AutoMapper para mapeamento
└─ Dependency Injection
```

### 11.3 Performance
```
✅ OTIMIZAÇÕES:
├─ Caching (IMemoryCache ou Redis)
│  ├─ Dashboard stats
│  ├─ Listas de usuários
│  └─ Respostas IA (cache por conteúdo)
├─ Paginação em todas listas
│  └─ PagedResult<T> { Items, TotalCount, PageSize, CurrentPage }
├─ Lazy Loading desabilitado (use Include explícito)
├─ Índices no banco:
│  ├─ Chamado.Status
│  ├─ Chamado.SuporteResponsavelId
│  ├─ Chamado.UsuarioId
│  └─ Usuario.Email
└─ Connection pooling configurado
```

### 11.4 Logging e Monitoramento
```
✅ IMPLEMENTAR:
├─ Serilog
│  ├─ Console Sink
│  ├─ File Sink
│  └─ Application Insights (Azure)
├─ Logs estruturados:
│  ├─ Request/Response
│  ├─ Erros de IA
│  ├─ Autenticação
│  └─ Operações críticas
└─ Health Checks:
   ├─ /health
   ├─ /health/ready
   └─ /health/live
```

### 11.5 Documentação
```
✅ ADICIONAR:
├─ Swagger/OpenAPI
│  ├─ Descrições de endpoints
│  ├─ Exemplos de request/response
│  └─ Autenticação JWT configurada
├─ README.md completo
├─ Postman Collection
└─ Arquivo .http para testes
```

### 11.6 Testes
```
✅ COBERTURA:
├─ Unit Tests
│  ├─ Services
│  ├─ Repositories
│  └─ Validações
├─ Integration Tests
│  ├─ Controllers
│  ├─ Database
│  └─ IA Services
└─ Frameworks:
   ├─ xUnit
   ├─ Moq
   └─ FluentAssertions
```

---

## 📦 12. MIGRAÇÃO WEB → API

### 12.1 Diferenças Principais
```
WEB (MVC):
├─ Retorna Views (HTML)
├─ Cookie Authentication
├─ ViewBag/ViewData
├─ RedirectToAction
└─ Model Binding com Views

API (REST):
├─ Retorna JSON
├─ JWT Authentication
├─ DTOs
├─ ActionResult<T> / IActionResult
└─ Status Codes (200, 201, 400, 401, 404, 500)
```

### 12.2 Exemplo de Conversão
```csharp
// WEB (MVC)
public async Task<IActionResult> Details(int? id)
{
    var chamado = await _context.Chamados
        .Include(c => c.Usuario)
        .FirstOrDefaultAsync(c => c.Id == id);
    
    if (chamado == null)
        return NotFound();
    
    return View(chamado); // Retorna HTML
}

// API (REST)
[HttpGet("{id}")]
public async Task<ActionResult<ChamadoDto>> GetChamado(int id)
{
    var chamado = await _context.Chamados
        .Include(c => c.Usuario)
        .Include(c => c.SuporteResponsavel)
        .Include(c => c.Mensagens)
        .FirstOrDefaultAsync(c => c.Id == id);
    
    if (chamado == null)
        return NotFound(new { message = "Chamado não encontrado" });
    
    var dto = _mapper.Map<ChamadoDto>(chamado);
    return Ok(dto); // Retorna JSON
}
```

### 12.3 DTOs Necessários
```csharp
// Authentication
public class LoginRequestDto
{
    public string Email { get; set; }
    public string Senha { get; set; }
}

public class LoginResponseDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public UsuarioDto Usuario { get; set; }
}

// Usuario
public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Tipo { get; set; }
    public DateTime DataCadastro { get; set; }
}

public class CreateUsuarioDto
{
    [Required]
    public string Nome { get; set; }
    
    [Required, EmailAddress]
    public string Email { get; set; }
    
    [Required, MinLength(6)]
    public string Senha { get; set; }
    
    [Required]
    public string Tipo { get; set; }
}

// Chamado
public class ChamadoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataAbertura { get; set; }
    public string Status { get; set; }
    public string CategoriaIA { get; set; }
    public string Prioridade { get; set; }
    public string RespostaIA { get; set; }
    public bool? FeedbackResolvido { get; set; }
    public UsuarioDto Usuario { get; set; }
    public UsuarioDto SuporteResponsavel { get; set; }
    public List<MensagemDto> Mensagens { get; set; }
}

public class CreateChamadoDto
{
    [Required, MaxLength(200)]
    public string Titulo { get; set; }
    
    [Required, MaxLength(2000)]
    public string Descricao { get; set; }
}

// Mensagem
public class MensagemDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNome { get; set; }
    public string Mensagem { get; set; }
    public DateTime DataEnvio { get; set; }
    public bool LidaPorCliente { get; set; }
    public bool LidaPorSuporte { get; set; }
}

public class CreateMensagemDto
{
    [Required, MaxLength(1000)]
    public string Mensagem { get; set; }
}

// Relatórios (reutilizar ViewModels existentes)
```

---

## 🚀 13. PRÓXIMOS PASSOS PARA API

```
1. Criar projeto GestaoChamadosAI_API
   ├─ dotnet new webapi -n GestaoChamadosAI_API
   └─ Referenciar projeto Web ou copiar Models/Data

2. Configurar autenticação JWT
   ├─ Install-Package Microsoft.AspNetCore.Authentication.JwtBearer
   └─ Configurar em Program.cs

3. Criar DTOs e AutoMapper profiles
   ├─ Install-Package AutoMapper.Extensions.Microsoft.DependencyInjection
   └─ Criar pasta DTOs/

4. Implementar Controllers REST
   ├─ Converter lógica dos Controllers MVC
   ├─ Usar ActionResult<T>
   └─ Retornar status codes apropriados

5. Adicionar Swagger
   ├─ Install-Package Swashbuckle.AspNetCore
   └─ Configurar em Program.cs

6. Implementar segurança
   ├─ Hash de senhas (BCrypt)
   ├─ Rate limiting
   └─ CORS

7. Testes
   ├─ Criar projeto de testes
   └─ Testar todos endpoints

8. Documentação
   ├─ README.md
   ├─ Postman Collection
   └─ Arquivo .http

9. Deploy
   ├─ Azure App Service
   ├─ Docker
   └─ CI/CD (GitHub Actions)
```

---

## 📝 CONCLUSÃO

Este sistema possui uma arquitetura MVC bem estruturada com integração avançada de IA. A transição para API REST será relativamente simples, pois a lógica de negócios já está separada nos Services e a camada de dados usa EF Core.

**Pontos Fortes:**
- ✅ Integração com Google Gemini AI funcional
- ✅ Sistema de chat em tempo real
- ✅ Relatórios completos com geração de PDF
- ✅ Dashboards personalizados por perfil
- ✅ Base de código limpa e documentada

**Melhorias Necessárias para API:**
- ⚠️ Segurança (JWT, hashing de senhas)
- ⚠️ DTOs e validações
- ⚠️ Rate limiting
- ⚠️ Paginação
- ⚠️ Caching
- ⚠️ Logs estruturados
- ⚠️ Testes automatizados

**Complexidade da Migração:** MÉDIA
**Tempo Estimado:** 2-3 semanas (com testes completos)

---

**Autor:** Análise gerada automaticamente
**Data:** 05/11/2025
**Versão do Sistema:** 1.0 (Web MVC)
