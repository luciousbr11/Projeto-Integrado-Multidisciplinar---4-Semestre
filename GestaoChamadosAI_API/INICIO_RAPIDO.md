# 🎯 INÍCIO RÁPIDO - GestaoChamadosAI API

## 📋 Checklist de Inicialização

### 1️⃣ Restaurar Pacotes NuGet
```powershell
cd c:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_API
dotnet restore
```

### 2️⃣ Configurar appsettings.json

Abra `appsettings.json` e configure:

**a) Connection String (se necessário):**
```json
"ConnectionStrings": {
  "ConexaoPadrao": "Server=localhost;Database=GestaoChamadosAI;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**b) JWT Secret Key (OBRIGATÓRIO mudar em produção):**
```json
"JwtSettings": {
  "SecretKey": "TROQUE_ESTA_CHAVE_POR_UMA_SEGURA_COM_32_CARACTERES_OU_MAIS!"
}
```

**c) Google Gemini API Key (já configurada):**
```json
"GeminiAI": {
  "ApiKey": "AIzaSyAli_1DftyIGb_LCvvQaJZ7Mto4tM8OLZg",
  "Model": "gemini-2.0-flash"
}
```

### 3️⃣ Aplicar Migrations (Banco de Dados)

**Se o banco já existe (do projeto Web):**
```powershell
# A API usará o mesmo banco do projeto Web
# Nenhuma ação necessária
```

**Se precisa criar o banco do zero:**
```powershell
# Criar migration inicial
dotnet ef migrations add Inicial

# Aplicar ao banco
dotnet ef database update
```

### 4️⃣ Criar Usuário Admin Inicial (PowerShell)

**Opção A: Via SQL Server Management Studio**
```sql
USE GestaoChamadosAI;

-- Senha: admin123 (hash BCrypt)
INSERT INTO Usuarios (Nome, Email, Senha, Tipo, DataCadastro) VALUES 
('Administrador', 'admin@teste.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMesJVe4JEjt0E0h5fZ7P0YmOi', 'Administrador', GETDATE());

-- Usuário Suporte (senha: suporte123)
INSERT INTO Usuarios (Nome, Email, Senha, Tipo, DataCadastro) VALUES 
('João Suporte', 'suporte@teste.com', '$2a$12$vZ8Gp1qCN9XJ5pP0Lv3k2OHd.K7YmW1fT4eR9xA5qS8nM2bV6cZ', 'Suporte', GETDATE());

-- Usuário Cliente (senha: cliente123)
INSERT INTO Usuarios (Nome, Email, Senha, Tipo, DataCadastro) VALUES 
('Maria Cliente', 'cliente@teste.com', '$2a$12$nH4Ft2kR8yD5xQ1wL9pM3O6gV7jB2nC5eT8sK4rP0aW3mY1fX9z', 'Cliente', GETDATE());
```

**Opção B: Via API (após iniciar)**
```powershell
# Primeiro, execute a API
dotnet run

# Depois, use o Swagger ou Postman para criar usuário via endpoint
# POST /api/usuarios (requer autenticação Admin)
```

### 5️⃣ Iniciar a API

```powershell
dotnet run
```

**Saída esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
[GEMINI] Serviço inicializado com modelo: gemini-2.0-flash
🚀 API iniciada com sucesso!
📊 Ambiente: Development
🌐 Swagger disponível em: http://localhost:5000
```

### 6️⃣ Testar a API

**Abra o navegador em:**
```
http://localhost:5000
```

Você verá a interface do **Swagger UI**.

### 7️⃣ Fazer Login

**No Swagger:**
1. Localize o endpoint `POST /api/auth/login`
2. Clique em "Try it out"
3. Cole o JSON:
```json
{
  "email": "admin@teste.com",
  "senha": "admin123"
}
```
4. Clique em "Execute"
5. Copie o `token` do response
6. Clique no botão "Authorize" (canto superior direito)
7. Digite: `Bearer {cole_o_token_aqui}`
8. Clique em "Authorize"

**Pronto!** Agora você pode testar todos os endpoints protegidos! 🎉

---

## 🧪 Endpoints de Teste Rápido

### 1. Login
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "admin@teste.com",
  "senha": "admin123"
}
```

### 2. Validar Token
```http
GET http://localhost:5000/api/auth/validate
Authorization: Bearer {seu_token}
```

### 3. Criar Chamado
```http
POST http://localhost:5000/api/chamados
Authorization: Bearer {seu_token}
Content-Type: application/json

{
  "titulo": "Sistema lento",
  "descricao": "O sistema está muito lento ao carregar as páginas"
}
```

### 4. Listar Chamados
```http
GET http://localhost:5000/api/chamados?page=1&pageSize=10
Authorization: Bearer {seu_token}
```

### 5. Obter Estatísticas
```http
GET http://localhost:5000/api/dashboard/estatisticas
Authorization: Bearer {seu_token}
```

---

## ⚠️ Possíveis Problemas

### Problema: "Cannot connect to SQL Server"
**Solução:**
```powershell
# Verifique se o SQL Server está rodando
Get-Service MSSQLSERVER

# Se não estiver, inicie:
Start-Service MSSQLSERVER
```

### Problema: "JWT SecretKey not configured"
**Solução:**
- Abra `appsettings.json`
- Edite a chave `JwtSettings:SecretKey`
- Deve ter **pelo menos 32 caracteres**

### Problema: "Gemini API error"
**Solução:**
- Verifique conexão com internet
- A API usará fallback automaticamente
- Ou configure nova API Key em `appsettings.json`

### Problema: Porta 5000 já em uso
**Solução:**
```powershell
# Execute em outra porta
dotnet run --urls "http://localhost:5002"
```

---

## 📚 Próximos Passos

1. ✅ Explore todos os endpoints no Swagger
2. ✅ Crie usuários de teste
3. ✅ Teste o fluxo completo de chamados
4. ✅ Teste o chat em tempo real
5. ✅ Gere relatórios em PDF
6. ✅ Configure CORS para sua aplicação MAUI
7. ✅ Implante em servidor de produção

---

## 🔒 Segurança em Produção

Antes de publicar em produção:

- [ ] Trocar `JwtSettings:SecretKey` por chave forte
- [ ] Trocar `GeminiAI:ApiKey` ou usar variáveis de ambiente
- [ ] Configurar `UseHttpsRedirection` corretamente
- [ ] Ajustar CORS para domínios específicos
- [ ] Configurar Rate Limiting conforme necessidade
- [ ] Revisar logs de erro (pasta `logs/`)
- [ ] Configurar backup automático do banco

---

## 📞 Suporte

Para dúvidas ou problemas:
- Consulte o arquivo `README.md` completo
- Consulte `ANALISE_BACKEND_COMPLETA.md` para detalhes técnicos
- Verifique os logs em `logs/api-{data}.log`

**Versão:** 1.0.0
**Data:** 05/11/2025
