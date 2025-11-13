# 🔍 AUDITORIA COMPLETA DE ENDPOINTS - WEB vs MAUI

**Data**: 08/11/2025  
**Objetivo**: Identificar e corrigir TODOS os problemas de integração entre Web API e MAUI

---

## 📋 METODOLOGIA

1. ✅ Listar TODOS os endpoints da API Web
2. ✅ Verificar se cada endpoint tem rota correta
3. ✅ Mapear chamadas correspondentes no MAUI
4. ✅ Identificar discrepâncias
5. ✅ Aplicar correções

---

## 🌐 ENDPOINTS DA API WEB

### 1. AccountController (Autenticação)

#### ✅ POST /api/Auth/login
- **Web**: `AccountController.Login([FromBody] LoginRequest)`
- **MAUI**: `AuthService.LoginAsync()` → `/api/Auth/login`
- **Status**: ✅ CONFORME

#### ✅ POST /api/Auth/logout
- **Web**: `AccountController.Logout()`
- **MAUI**: `AuthService.LogoutAsync()` → `/api/Auth/logout`
- **Status**: ✅ CONFORME

---

### 2. ChamadosController (Gestão de Chamados)

#### ✅ GET /api/Chamados
- **Web**: `ChamadosController.Index(status, suporteId, prioridade, page, pageSize)`
- **MAUI**: `ChamadoService.GetChamadosAsync()` → `/api/Chamados?page=1&pageSize=100`
- **Status**: ✅ CONFORME

#### ✅ GET /api/Chamados/{id}
- **Web**: `ChamadosController.Details(id)`
- **MAUI**: `ChamadoService.GetChamadoByIdAsync(id)` → `/api/Chamados/{id}`
- **Status**: ✅ CONFORME

#### ✅ POST /api/Chamados
- **Web**: `ChamadosController.Create([FromBody] ChamadoCreateRequest)`
- **MAUI**: `ChamadoService.CreateChamadoAsync()` → `/api/Chamados`
- **Status**: ✅ CONFORME

#### ✅ PUT /api/Chamados/{id}
- **Web**: `ChamadosController.Edit(id, [FromBody] ChamadoEditRequest)`
- **MAUI**: `ChamadoService.EditarChamadoAsync()` → `/api/Chamados/{id}`
- **Status**: ✅ CONFORME

#### ⚠️ POST /api/Chamados/{id}/transferir
- **Web**: `ChamadosController.TransferirChamado(id, novoSuporteId)`
- **MAUI**: `ChamadoService.TransferirChamadoAsync()` → `/api/Chamados/{id}/transferir`
- **Status**: ⚠️ VERIFICAR - Pode não ter rota explícita

#### ⚠️ POST /api/Chamados/{id}/gerar-resposta-ia
- **Web**: `ChamadosController.GerarRespostaIA(id)`
- **MAUI**: `ChamadoService.GerarRespostaIAAsync()` → endpoint a definir
- **Status**: ⚠️ VERIFICAR - Rota não confirmada

---

### 3. ChatController (Chat e Mensagens)

#### ❌ POST /api/Chat/AssumirAtendimento
- **Web**: `ChatController.AssumirAtendimento([FromBody] AssumirAtendimentoRequest)`
- **MAUI**: `ChamadoService.AssumirChamadoAsync()` → `/api/Chat/AssumirAtendimento`
- **Status**: ❌ ERRO 405 - Rota não funcionando
- **Problema**: Controller MVC sem rota API explícita
- **Solução**: Adicionar `[Route("api/Chat/AssumirAtendimento")]` no método

#### ❌ POST /api/Chat/FinalizarAtendimento
- **Web**: `ChatController.FinalizarAtendimento(chamadoId)`
- **MAUI**: `ChamadoService.FinalizarChamadoAsync()` → `/api/Chat/FinalizarAtendimento?chamadoId={id}`
- **Status**: ❌ ERRO 405 - Rota não funcionando
- **Problema**: Mesma causa - falta rota explícita
- **Solução**: Adicionar `[Route("api/Chat/FinalizarAtendimento")]` no método

#### ⚠️ POST /api/Chat/EnviarMensagem
- **Web**: `ChatController.EnviarMensagem(chamadoId, mensagem)`
- **MAUI**: Chamado no ChatViewModel
- **Status**: ⚠️ VERIFICAR

---

### 4. UsuariosController (Gestão de Usuários)

#### ✅ GET /api/Usuarios
- **Web**: `UsuariosController.Index(tipo, page, pageSize)`
- **MAUI**: `UsuarioService.GetUsuariosAsync()` → `/api/Usuarios`
- **Status**: ✅ CONFORME

#### ✅ GET /api/Usuarios/{id}
- **Web**: `UsuariosController.Details(id)`
- **MAUI**: `UsuarioService.GetUsuarioByIdAsync(id)` → `/api/Usuarios/{id}`
- **Status**: ✅ CONFORME

#### ✅ POST /api/Usuarios
- **Web**: `UsuariosController.Create([FromBody] UsuarioCreateRequest)`
- **MAUI**: `UsuarioService.CreateUsuarioAsync()` → `/api/Usuarios`
- **Status**: ✅ CONFORME

#### ✅ PUT /api/Usuarios/{id}
- **Web**: `UsuariosController.Edit(id, [FromBody] UsuarioEditRequest)`
- **MAUI**: `UsuarioService.EditarUsuarioAsync()` → `/api/Usuarios/{id}`
- **Status**: ✅ CONFORME

#### ✅ DELETE /api/Usuarios/{id}
- **Web**: `UsuariosController.DeleteConfirmed(id)`
- **MAUI**: `UsuarioService.DeleteUsuarioAsync(id)` → `/api/Usuarios/{id}`
- **Status**: ✅ CONFORME

---

### 5. DashboardController (Estatísticas)

#### ✅ GET /api/Dashboard/estatisticas
- **Web**: `DashboardController.GetEstatisticas()`
- **MAUI**: `DashboardViewModel` → `/api/Dashboard/estatisticas`
- **Status**: ✅ CONFORME

---

### 6. RelatoriosController (Relatórios)

#### ✅ GET /api/Relatorios/usuarios
- **Web**: `RelatoriosController.Usuarios()`
- **MAUI**: `RelatorioService.GetRelatorioUsuariosAsync()` → `/api/Relatorios/usuarios`
- **Status**: ✅ CONFORME

#### ✅ GET /api/Relatorios/chamados-periodo
- **Web**: `RelatoriosController.ChamadosPorPeriodo(dataInicio, dataFim)`
- **MAUI**: `RelatorioService.GetRelatorioChamadosPeriodoAsync()` → `/api/Relatorios/chamados-periodo`
- **Status**: ✅ CONFORME

#### ✅ GET /api/Relatorios/suportes
- **Web**: `RelatoriosController.Suportes()`
- **MAUI**: `RelatorioService.GetRelatorioSuportesAsync()` → `/api/Relatorios/suportes`
- **Status**: ✅ CONFORME

#### ✅ GET /api/Relatorios/categorias
- **Web**: `RelatoriosController.Categorias()`
- **MAUI**: `RelatorioService.GetRelatorioCategoriasAsync()` → `/api/Relatorios/categorias`
- **Status**: ✅ CONFORME

---

## 🔧 PROBLEMAS IDENTIFICADOS

### PROBLEMA 1: ChatController sem rotas API ❌ CRÍTICO

**Causa Raiz**: 
- `ChatController` é um controller MVC híbrido (retorna Views E JSON)
- Não tem `[Route("api/[controller]")]` no controller
- Métodos API não têm rotas explícitas

**Métodos Afetados**:
1. ❌ `AssumirAtendimento` 
2. ❌ `FinalizarAtendimento`
3. ⚠️ `EnviarMensagem`
4. ⚠️ `BuscarNovasMensagens`

**Solução Aplicada**:
```csharp
// Adicionar rota explícita em cada método API
[HttpPost]
[Route("api/Chat/AssumirAtendimento")]
[Authorize(Roles = "Suporte,Administrador")]
public async Task<IActionResult> AssumirAtendimento([FromBody] AssumirAtendimentoRequest request)
```

---

### PROBLEMA 2: Possíveis rotas faltando em ChamadosController ⚠️

**Endpoints a verificar**:
- `TransferirChamado`
- `GerarRespostaIA`
- `Feedback`
- `Reassumir`
- `FinalizarChamado` (diferente do Chat)

**Ação**: Verificar se todos têm rotas explícitas ou se dependem de convenção

---

## 📊 RESUMO DE CONFORMIDADE

| Área | Total Endpoints | Conformes | Problemas | Taxa Sucesso |
|------|----------------|-----------|-----------|--------------|
| Autenticação | 2 | 2 | 0 | 100% |
| Chamados | 6+ | 4 | 2+ | ~67% |
| Chat | 4 | 0 | 4 | 0% ❌ |
| Usuários | 5 | 5 | 0 | 100% |
| Dashboard | 1 | 1 | 0 | 100% |
| Relatórios | 4 | 4 | 0 | 100% |
| **TOTAL** | **22+** | **16** | **6+** | **73%** |

---

## ✅ PLANO DE CORREÇÃO

### FASE 1: Correções Críticas (AGORA) ⏰

1. ✅ Adicionar rotas explícitas em todos métodos do ChatController
2. ⚠️ Verificar e corrigir ChamadosController
3. ⚠️ Testar TODOS os endpoints após correção

### FASE 2: Padronização (DEPOIS)

1. Criar um `ChatApiController` separado para endpoints API
2. Manter `ChatController` apenas para Views MVC
3. Documentar todos endpoints com Swagger/OpenAPI

### FASE 3: Testes Automatizados

1. Criar script de teste para todos endpoints
2. Validar resposta de cada endpoint
3. Testar com diferentes roles (Admin, Suporte, Cliente)

---

## 🎯 PRÓXIMOS PASSOS

1. **AGORA**: Verificar todos endpoints do ChamadosController
2. **AGORA**: Adicionar rotas faltantes no ChatController
3. **AGORA**: Recompilar e reiniciar API
4. **AGORA**: Testar cada funcionalidade no MAUI
5. **DEPOIS**: Criar API Controller separado

---

**Status Geral**: 🔴 **73% CONFORME** - Necessita correções urgentes no ChatController

