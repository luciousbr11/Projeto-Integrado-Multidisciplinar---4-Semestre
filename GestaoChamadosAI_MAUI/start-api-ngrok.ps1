# ============================================
#  SCRIPT: Iniciar API + ngrok
#  Uso: .\start-api-ngrok.ps1
# ============================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  INICIANDO API + NGROK" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verifica se ngrok está instalado
$ngrokPath = Get-Command ngrok -ErrorAction SilentlyContinue
if (-not $ngrokPath) {
    Write-Host "ERRO: ngrok não encontrado!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Instale o ngrok primeiro:" -ForegroundColor Yellow
    Write-Host "  1. Acesse: https://ngrok.com/download" -ForegroundColor White
    Write-Host "  2. Baixe e extraia em C:\ngrok\" -ForegroundColor White
    Write-Host "  3. Configure o token (ver GUIA-NGROK-COMPLETO.md)" -ForegroundColor White
    Write-Host ""
    Read-Host "Pressione Enter para sair"
    exit
}

Write-Host "1. Iniciando API na pasta GestaoChamadosAI_API..." -ForegroundColor White
Write-Host ""

# Inicia a API em um novo terminal
$apiPath = "c:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_API"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$apiPath'; dotnet run"

Write-Host "Aguardando 5 segundos para a API iniciar..." -ForegroundColor Gray
Start-Sleep -Seconds 5

Write-Host ""
Write-Host "2. Iniciando ngrok para expor na porta 5000..." -ForegroundColor White
Write-Host ""

# Inicia o ngrok em um novo terminal
Start-Process powershell -ArgumentList "-NoExit", "-Command", "ngrok http 5000"

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  SUCESSO!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Proximos passos:" -ForegroundColor Cyan
Write-Host "  1. Veja a URL gerada no terminal do ngrok" -ForegroundColor White
Write-Host "     Exemplo: https://abc123.ngrok-free.app" -ForegroundColor Yellow
Write-Host ""
Write-Host "  2. Copie essa URL (SEM a barra no final)" -ForegroundColor White
Write-Host ""
Write-Host "  3. Edite Services/ApiService.cs:" -ForegroundColor White
Write-Host "     Linha 27, altere BaseUrl para a URL do ngrok" -ForegroundColor Gray
Write-Host ""
Write-Host "  4. Recompile e instale:" -ForegroundColor White
Write-Host "     dotnet build -f net9.0-android" -ForegroundColor Gray
Write-Host "     .\install-android.ps1" -ForegroundColor Gray
Write-Host ""
Write-Host "  5. Teste o app no celular!" -ForegroundColor White
Write-Host "     Funciona de qualquer lugar (Wi-Fi diferente, 4G, etc)" -ForegroundColor Green
Write-Host ""
Write-Host "Para parar: Feche as janelas ou pressione Ctrl+C" -ForegroundColor Yellow
Write-Host ""
