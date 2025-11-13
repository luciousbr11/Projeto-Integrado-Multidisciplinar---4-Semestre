# 🌍 Guia Completo: ngrok - Expor API na Internet

## 📥 PASSO 1: Criar Conta e Baixar ngrok

### 1.1 Criar conta GRÁTIS
1. Acesse: **https://ngrok.com/**
2. Clique em **"Sign up"**
3. Use sua conta Google/GitHub ou crie com email
4. ✅ É 100% GRATUITO para uso básico!

### 1.2 Baixar o ngrok
1. Após login, você será redirecionado para: **https://dashboard.ngrok.com/get-started/setup**
2. Clique em **"Download for Windows"** (ou acesse https://ngrok.com/download)
3. Baixe o arquivo: `ngrok-v3-stable-windows-amd64.zip`
4. Extraia o arquivo ZIP em uma pasta (exemplo: `C:\ngrok\`)

### 1.3 Adicionar ngrok ao PATH (Opcional mas recomendado)
1. Copie o caminho da pasta (exemplo: `C:\ngrok\`)
2. Pressione `Win + R` → digite `sysdm.cpl` → Enter
3. Aba **"Avançado"** → **"Variáveis de Ambiente"**
4. Em **"Variáveis do sistema"**, encontre **"Path"** → **"Editar"**
5. Clique **"Novo"** → Cole `C:\ngrok\` → **OK**

---

## 🔑 PASSO 2: Autenticar (Obrigatório)

### 2.1 Pegar seu Token de Autenticação
1. No dashboard do ngrok: **https://dashboard.ngrok.com/get-started/your-authtoken**
2. Copie seu token (algo como: `2abc123def456ghi789jkl0mnop`)

### 2.2 Configurar o token no ngrok
Abra o PowerShell **NA PASTA DO NGROK** e execute:

```powershell
# Se você adicionou ao PATH:
ngrok config add-authtoken SEU_TOKEN_AQUI

# OU se não adicionou ao PATH, navegue até a pasta:
cd C:\ngrok
.\ngrok.exe config add-authtoken SEU_TOKEN_AQUI
```

**Exemplo:**
```powershell
ngrok config add-authtoken 2abc123def456ghi789jkl0mnop
```

✅ Você verá: `Authtoken saved to configuration file: C:\Users\SeuUsuario\.ngrok2\ngrok.yml`

---

## 🚀 PASSO 3: Iniciar sua API

### 3.1 Certifique-se que a API está rodando
1. Abra um terminal na pasta `GestaoChamadosAI_API`
2. Execute:
```powershell
dotnet run
```
3. ✅ Aguarde até ver: `Application started. Press Ctrl+C to shut down.`
4. ✅ API rodando em: `http://localhost:5000` (ou `http://0.0.0.0:5000`)

---

## 🌐 PASSO 4: Expor a API com ngrok

### 4.1 Abrir OUTRO terminal (deixe a API rodando)
Abra um **novo PowerShell**

### 4.2 Executar ngrok
```powershell
# Se adicionou ao PATH:
ngrok http 5000

# OU se não adicionou, navegue até a pasta:
cd C:\ngrok
.\ngrok.exe http 5000
```

### 4.3 O que você verá:
```
ngrok                                                                   

Session Status                online
Account                       Seu Nome (Plan: Free)
Version                       3.22.1
Region                        United States (us)
Latency                       45ms
Web Interface                 http://127.0.0.1:4040
Forwarding                    https://abc123def456.ngrok-free.app -> http://localhost:5000

Connections                   ttl     opn     rt1     rt5     p50     p90
                              0       0       0.00    0.00    0.00    0.00
```

✅ **COPIE A URL**: `https://abc123def456.ngrok-free.app`

**IMPORTANTE:**
- Essa URL é **temporária** - muda toda vez que você reiniciar o ngrok
- Para URL fixa, precisa do plano pago (~$8/mês)
- Com plano grátis, funciona perfeitamente mas precisa atualizar a URL no código a cada reinício

---

## 📱 PASSO 5: Configurar o App MAUI

### 5.1 Atualizar ApiService.cs
Abra: `GestaoChamadosAI_MAUI/Services/ApiService.cs`

Localize:
```csharp
#if ANDROID
    private const string BaseUrl = "http://192.168.200.107:5000";
#else
    private const string BaseUrl = "http://localhost:5000";
#endif
```

Altere para:
```csharp
#if ANDROID
    // COLE A URL DO NGROK AQUI (SEM A BARRA NO FINAL)
    private const string BaseUrl = "https://abc123def456.ngrok-free.app";
#else
    private const string BaseUrl = "http://localhost:5000";
#endif
```

### 5.2 Recompilar e Reinstalar
```powershell
cd c:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_MAUI
dotnet build -f net9.0-android
.\install-android.ps1
```

---

## ✨ PASSO 6: Testar!

### 6.1 No celular:
- Abra o app
- Faça login
- ✅ **FUNCIONA DE QUALQUER LUGAR!**
  - Wi-Fi diferente
  - 4G/5G
  - Outro país

### 6.2 Testar no navegador:
- Acesse a URL do ngrok no Chrome do celular
- Exemplo: `https://abc123def456.ngrok-free.app/swagger`
- ✅ Deve abrir o Swagger da API

---

## 🎯 VANTAGENS do ngrok:

✅ **Funciona de qualquer lugar** (não precisa estar na mesma rede)
✅ **HTTPS grátis** (seguro)
✅ **Sem configurar firewall/roteador**
✅ **Dashboard web** em `http://127.0.0.1:4040` para ver requisições
✅ **Perfeito para demonstrações/testes**

---

## ⚠️ LIMITAÇÕES (Plano Grátis):

❌ URL muda toda vez que reinicia o ngrok
❌ Limite de 40 conexões/minuto
❌ Aparece aviso do ngrok antes de acessar
❌ Sessão expira após 2 horas (mas pode reiniciar)

---

## 💰 PLANO PAGO (Opcional - $8/mês):

✅ URL fixa (exemplo: `https://seu-app.ngrok.io`)
✅ Sem limite de conexões
✅ Sem aviso/tela intermediária
✅ Múltiplos túneis simultâneos

**Assinar:** https://dashboard.ngrok.com/billing/plan

---

## 🛠️ Comandos Úteis:

### Ver túneis ativos:
```powershell
ngrok tunnels
```

### Parar ngrok:
Pressione **Ctrl + C** no terminal do ngrok

### Ver dashboard web (requisições ao vivo):
Acesse: **http://127.0.0.1:4040**

### Usar domínio customizado (apenas plano pago):
```powershell
ngrok http 5000 --domain=seu-dominio.ngrok.io
```

---

## 🔄 FLUXO DE TRABALHO DIÁRIO:

### Cada vez que for trabalhar:

1. **Terminal 1** - Iniciar API:
```powershell
cd c:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_API
dotnet run
```

2. **Terminal 2** - Iniciar ngrok:
```powershell
ngrok http 5000
```

3. **Copiar a URL** que apareceu (exemplo: `https://xyz789.ngrok-free.app`)

4. **SE A URL MUDOU**:
   - Atualizar `ApiService.cs` com a nova URL
   - Recompilar: `dotnet build -f net9.0-android`
   - Reinstalar: `.\install-android.ps1`

---

## 🎓 ALTERNATIVA: URL Fixa com Plano Grátis

Se você não quer pagar mas quer evitar trocar a URL toda hora, pode:

1. **Deixar o PC ligado 24/7** com ngrok rodando
2. **Usar o nome de domínio do ngrok grátis** (muda só quando reiniciar o PC)
3. **Configurar o app para pedir a URL** (campo de input na tela de login)

---

## 📞 Precisa de Ajuda?

- **Documentação oficial:** https://ngrok.com/docs
- **Dashboard:** https://dashboard.ngrok.com/
- **Status:** https://status.ngrok.com/

---

## ✅ RESUMO SUPER RÁPIDO:

```powershell
# 1. Baixe em: https://ngrok.com/download
# 2. Extraia em C:\ngrok\
# 3. Pegar token em: https://dashboard.ngrok.com/get-started/your-authtoken
# 4. Configurar token:
cd C:\ngrok
.\ngrok.exe config add-authtoken SEU_TOKEN

# 5. Rodar API (Terminal 1):
cd c:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_API
dotnet run

# 6. Rodar ngrok (Terminal 2):
cd C:\ngrok
.\ngrok.exe http 5000

# 7. Copiar URL e colocar no ApiService.cs
# 8. Recompilar e instalar no Android

# PRONTO! 🚀
```

---

**Criado em:** 10/11/2025
**Versão ngrok:** 3.x
**Sistema:** Windows 11
**Projeto:** Gestão de Chamados AI
