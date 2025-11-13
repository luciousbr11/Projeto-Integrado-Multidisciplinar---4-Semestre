# 🚀 Como Testar o App GestaoChamadosAI MAUI

## ⚡ Forma Mais Rápida (Windows Desktop)

### Opção 1: Executar direto (sem compilar)
```powershell
cd GestaoChamadosAI_MAUI
.\start-app.ps1
```

### Opção 2: Compilar e executar
```powershell
cd GestaoChamadosAI_MAUI
.\run-app.ps1
```

### Opção 3: Manual
```powershell
# Compilar
dotnet build GestaoChamadosAI_MAUI.csproj -f net9.0-windows10.0.19041.0

# Executar
.\bin\Debug\net9.0-windows10.0.19041.0\win10-x64\GestaoChamadosAI_MAUI.exe
```

---

## 📱 Como Testar Responsividade

### No Windows Desktop:
1. Execute o aplicativo
2. **Redimensione a janela** para menos de 600px de largura
3. Observe os layouts mudarem automaticamente:
   - ✅ Grids de múltiplas colunas → 1 coluna
   - ✅ Cards empilham verticalmente
   - ✅ Botões ficam em linha única
   - ✅ Todos os elementos se ajustam

### Telas para Testar:
- ✅ **Login** - Layout 2 colunas → 1 coluna
- ✅ **Dashboard** - Cards de estatísticas empilham
- ✅ **Novo Chamado** - Botões empilham
- ✅ **Usuários** - Grid de 4 colunas → 1 coluna
- ✅ **Relatórios** - Todos os gráficos/stats empilham
- ✅ **Feedback IA** - Botões empilham

---

## 📱 Testar em Android Real (Opcional)

### Pré-requisitos:
1. Celular Android com cabo USB
2. Ativar Modo Desenvolvedor no celular
3. Ativar Depuração USB

### Passos:
```powershell
# 1. Conectar celular via USB
# 2. Verificar se está conectado
adb devices

# 3. Compilar e instalar
dotnet build -t:Run -f net9.0-android
```

---

## 🎯 Configuração da API

O app está configurado para:
- **Desktop**: `http://localhost:5000`
- **Android**: `http://10.0.2.2:5000` (emulador) ou configure seu IP local

### Para testar com API real:
1. Certifique-se que a API Web está rodando na porta 5000
2. Execute o app desktop
3. Faça login e teste as funcionalidades

---

## 🐛 Resolução de Problemas

### Erro "Nenhum dispositivo disponível" ao tentar Android:
✅ **Use o app Desktop** - É mais rápido e fácil para desenvolvimento!

### App não abre:
```powershell
# Limpar e recompilar
dotnet clean
dotnet build -f net9.0-windows10.0.19041.0
```

### Testar mudanças:
```powershell
# Após fazer alterações no código
dotnet build -f net9.0-windows10.0.19041.0
.\bin\Debug\net9.0-windows10.0.19041.0\win10-x64\GestaoChamadosAI_MAUI.exe
```

---

## 💡 Dicas

- 🔥 **Hot Reload**: Use o Visual Studio para aproveitar hot reload
- 🪟 **Teste Desktop**: Mais rápido que qualquer emulador
- 📏 **Responsividade**: Redimensione a janela para <600px
- 🔄 **Breakpoint**: 600px é o ponto de mudança mobile/desktop
- 📱 **Celular Real**: Melhor opção para testes mobile finais

---

## ✨ Recursos Implementados

- ✅ Login com autenticação JWT
- ✅ Dashboard responsivo para Admin/Suporte/Cliente
- ✅ Gestão de chamados com IA
- ✅ Chat de suporte
- ✅ Relatórios com filtros
- ✅ Gestão de usuários
- ✅ **100% das telas responsivas** (mobile + desktop)

---

**Criado em:** Novembro 2025
**Tecnologia:** .NET MAUI 9.0
