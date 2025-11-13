# Script para executar o app MAUI Desktop facilmente
# Uso: .\run-app.ps1

Write-Host "Compilando GestaoChamadosAI MAUI..." -ForegroundColor Cyan

# Compilar o projeto
dotnet build "GestaoChamadosAI_MAUI.csproj" -f net9.0-windows10.0.19041.0

if ($LASTEXITCODE -eq 0) {
    Write-Host "Compilacao bem-sucedida!" -ForegroundColor Green
    Write-Host "Iniciando aplicativo..." -ForegroundColor Cyan
    
    # Executar o app
    Start-Process "bin\Debug\net9.0-windows10.0.19041.0\win10-x64\GestaoChamadosAI_MAUI.exe"
    
    Write-Host ""
    Write-Host "Aplicativo iniciado!" -ForegroundColor Green
    Write-Host "Dica: Redimensione a janela para menos de 600px para ver a responsividade mobile!" -ForegroundColor Yellow
} else {
    Write-Host "Erro na compilacao!" -ForegroundColor Red
}
