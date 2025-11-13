# Script para executar o app rapidamente (sem compilar)
# Uso: .\start-app.ps1

Write-Host "Iniciando GestaoChamadosAI MAUI..." -ForegroundColor Cyan

$exePath = "bin\Debug\net9.0-windows10.0.19041.0\win10-x64\GestaoChamadosAI_MAUI.exe"

if (Test-Path $exePath) {
    Start-Process $exePath
    Write-Host "Aplicativo iniciado!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Dicas de teste:" -ForegroundColor Yellow
    Write-Host "   - Redimensione a janela para menos de 600px de largura" -ForegroundColor Gray
    Write-Host "   - Todos os layouts mudarao automaticamente!" -ForegroundColor Gray
    Write-Host "   - Teste todas as telas: Dashboard, Relatorios, Usuarios, etc." -ForegroundColor Gray
} else {
    Write-Host "Executavel nao encontrado!" -ForegroundColor Red
    Write-Host "Execute primeiro: dotnet build" -ForegroundColor Yellow
}
