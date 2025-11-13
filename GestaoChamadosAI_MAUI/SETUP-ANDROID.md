# 📱 Como Testar no Celular Android Real

## Passo 1: Habilitar Modo Desenvolvedor no Celular

### Android 10 ou superior:
1. Abra **Configurações**
2. Vá em **Sobre o telefone** (ou **Sistema** → **Sobre o telefone**)
3. Encontre **Número da compilação** (ou **Versão da compilação**)
4. **Toque 7 vezes** no número da compilação
5. Aparecerá: "Você agora é um desenvolvedor!"

### Se não encontrar "Número da compilação":
- Procure em: **Configurações** → **Sistema** → **Informações do software**
- Ou: **Configurações** → **Sobre** → **Informações do software**

---

## Passo 2: Ativar Depuração USB

1. Volte para **Configurações**
2. Procure **Opções do desenvolvedor** (ou **Developer options**)
   - Pode estar em: **Sistema** → **Avançado** → **Opções do desenvolvedor**
3. **Ative** as Opções do desenvolvedor (toggle no topo)
4. Role para baixo e ative:
   - ✅ **Depuração USB**
   - ✅ **Instalar via USB** (se disponível)
   - ✅ **Verificação de apps via USB** → Desativar (opcional, facilita instalação)

---

## Passo 3: Conectar o Celular no PC

1. Conecte o celular no PC via **cabo USB**
2. No celular, selecione: **Transferência de arquivos** (ou **MTP**)
   - Aparece uma notificação quando conecta
3. Aparecerá um popup: **"Permitir depuração USB?"**
   - ✅ Marque: "Sempre permitir neste computador"
   - Toque em: **Permitir** (ou **OK**)

---

## Passo 4: Verificar se o PC Reconhece o Celular

Abra o PowerShell e execute:
```powershell
adb devices
```

### ✅ Resultado esperado:
```
List of devices attached
ABC123XYZ    device
```

### ❌ Se aparecer "adb não é reconhecido":
ADB não está instalado. Vamos instalar!

---

## Passo 5: Instalar ADB (Se necessário)

### Opção A - Via Chocolatey (Recomendado):
```powershell
# Instalar Chocolatey (se não tiver)
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

# Instalar ADB
choco install adb -y
```

### Opção B - Via Winget:
```powershell
winget install Google.PlatformTools
```

### Opção C - Manual:
1. Baixe: https://developer.android.com/tools/releases/platform-tools
2. Extraia em: `C:\platform-tools`
3. Adicione ao PATH do Windows

---

## Passo 6: Configurar IP da API

Seu celular precisa acessar a API no seu PC!

### Descobrir seu IP local:
```powershell
ipconfig
```
Procure por: **IPv4 Address** (ex: 192.168.1.100)

### Editar ApiService.cs:
Mude a BaseUrl para Android usar seu IP:
```csharp
#if ANDROID
    private const string BaseUrl = "http://SEU_IP_AQUI:5000/api";
#else
    private const string BaseUrl = "http://localhost:5000/api";
#endif
```

---

## Passo 7: Compilar e Instalar no Celular

```powershell
cd c:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_MAUI

# Compilar e instalar
dotnet build -t:Run -f net9.0-android
```

---

## 🎯 Comandos Úteis

### Verificar dispositivos conectados:
```powershell
adb devices
```

### Apenas compilar (não instalar):
```powershell
dotnet build -f net9.0-android
```

### Instalar APK manualmente:
```powershell
adb install bin\Debug\net9.0-android\com.companyname.gestãochamadosai_maui-Signed.apk
```

### Ver logs do app:
```powershell
adb logcat
```

### Desinstalar do celular:
```powershell
adb uninstall com.companyname.gestãochamadosai_maui
```

---

## 🔥 Troubleshooting

### "Nenhum dispositivo disponível"
- ✅ Celular conectado via USB?
- ✅ Depuração USB ativada?
- ✅ Permitiu depuração no popup?
- ✅ Execute: `adb devices` para verificar

### "Unauthorized"
- ✅ Desconecte e reconecte o cabo
- ✅ Revogue autorizações: **Opções do desenvolvedor** → **Revogar autorizações de depuração USB**
- ✅ Conecte novamente e aceite o popup

### "Device offline"
- ✅ Execute: `adb kill-server` e depois `adb devices`
- ✅ Reinicie o celular

### App não conecta na API
- ✅ Celular e PC na mesma rede Wi-Fi?
- ✅ IP correto no ApiService.cs?
- ✅ API Web rodando na porta 5000?
- ✅ Firewall do Windows não está bloqueando?

---

## ✨ Dicas

- 🔌 Use cabo USB original ou de boa qualidade
- 📶 Mantenha celular e PC na mesma rede Wi-Fi
- 🔄 Primeira instalação pode demorar 1-2 minutos
- ⚡ Instalações seguintes são mais rápidas
- 📱 App fica instalado no celular normalmente

---

## 🎮 Testando Responsividade

Após instalar, teste:
- ✅ Todas as telas se adaptam ao tamanho do celular
- ✅ Grids empilham em coluna única
- ✅ Botões ficam full-width
- ✅ Interface totalmente navegável
- ✅ Touch funciona perfeitamente

---

**Pronto para testar no Android real! 🚀**
