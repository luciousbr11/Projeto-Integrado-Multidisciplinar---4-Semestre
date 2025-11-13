# ?? Instruções para Tela de Login Mobile

## ? Correções Realizadas

### 1. **Layout Mobile Otimizado**
- ? Design responsivo e adaptativo para Android
- ?? Layout simplificado que se adapta ao tamanho da tela
- ?? Espaçamento e tamanhos de fonte otimizados para mobile
- ?? Logo e branding apropriados para cada plataforma

### 2. **Melhorias Visuais**
- Logo circular no topo (mobile)
- Campos de entrada com tamanho adequado (48px de altura)
- Botão de login com visual moderno
- Mensagens de erro claramente visíveis
- Indicador de carregamento durante o login

### 3. **Tratamento de Erros Melhorado**
- ?? Validação de campos vazios
- ?? Validação de formato de e-mail
- ?? Mensagens claras de erro de conexão
- ?? Feedback visual de erros de autenticação

## ?? Como Testar

### Pré-requisitos
1. **API Backend rodando**
   - A API deve estar rodando em `http://localhost:5006`
   - Para Android Emulator, o app usa `http://10.0.2.2:5006` automaticamente

2. **Configuração do Android Emulator**
   - Certifique-se de que o emulador tem acesso à rede
   - O código já está configurado para usar `10.0.2.2` (localhost do host)

### Passos para Testar

1. **Inicie a API Backend**
   ```bash
   # No diretório da API
   dotnet run
   ```
   Verifique se está rodando em: `http://localhost:5006`

2. **Execute o App MAUI**
   - Selecione o emulador Android ou dispositivo físico
   - Execute o projeto (F5 no Visual Studio)

3. **Teste o Login**
   - Digite um e-mail válido (ex: `admin@teste.com`)
   - Digite a senha
   - Clique em "Entrar"

### ?? Troubleshooting

#### Problema: "Erro de conexão"
**Solução:**
1. Verifique se a API está rodando:
   ```bash
   curl http://localhost:5006/api/health
   ```

2. Para Android Emulator, teste a conexão:
   ```bash
   # No adb shell do emulador
   adb shell
   ping 10.0.2.2
   ```

3. Verifique os logs da API para ver se as requisições estão chegando

#### Problema: Layout ainda quebrado
**Solução:**
1. Limpe e reconstrua o projeto:
   ```bash
   dotnet clean
   dotnet build
   ```

2. Limpe o cache do Android:
   - No Visual Studio: Build > Clean Solution
   - Delete as pastas `bin` e `obj`

#### Problema: "Email ou senha inválidos"
**Solução:**
1. Verifique as credenciais no banco de dados da API
2. Certifique-se de que o usuário existe
3. Teste diretamente na API usando Postman ou curl:
   ```bash
   curl -X POST http://localhost:5006/api/Auth/login \
     -H "Content-Type: application/json" \
     -d '{"email":"admin@teste.com","senha":"senha123"}'
   ```

## ?? Notas Importantes

### URL da API
O app está configurado para usar:
- **Windows**: `http://localhost:5006`
- **Android**: `http://10.0.2.2:5006` (localhost do host)

Se sua API estiver em outra porta, edite o arquivo `Services/ApiService.cs`:
```csharp
#if ANDROID
    private const string BaseUrl = "http://10.0.2.2:PORTA";
#else
    private const string BaseUrl = "http://localhost:PORTA";
#endif
```

### Logs de Debug
O app gera logs em:
- Desktop: `C:\Users\[SEU_USUARIO]\Desktop\maui_log.txt`
- Consulte este arquivo para detalhes de requisições HTTP

### Credenciais Padrão
Certifique-se de ter um usuário cadastrado na API. Exemplo:
- **Email**: `admin@teste.com`
- **Senha**: `senha123`

## ?? Recursos Visuais

### Layout Desktop (WinUI)
- Tela dividida em 2 colunas
- Lado esquerdo: Branding com logo e features
- Lado direito: Formulário de login

### Layout Mobile (Android)
- Layout vertical em coluna única
- Logo circular no topo
- Formulário centralizado
- Otimizado para telas pequenas

## ?? Próximos Passos

Se o login ainda não funcionar após estas correções:

1. **Verifique a conectividade de rede**
2. **Teste a API diretamente** (Postman/curl)
3. **Verifique os logs** do app (maui_log.txt)
4. **Verifique os logs** da API
5. **Teste com credenciais válidas**

## ?? Suporte

Se precisar de mais ajuda:
1. Verifique o arquivo `maui_log.txt` para detalhes do erro
2. Verifique os logs da API
3. Certifique-se de que usuário e senha existem no banco de dados
