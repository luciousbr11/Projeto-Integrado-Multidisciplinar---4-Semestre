# Script para instalar o app no celular Android
# Uso: .\install-android.ps1

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  INSTALANDO APP NO CELULAR ANDROID" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se o celular está conectado
Write-Host "1. Verificando dispositivos conectados..." -ForegroundColor Yellow
$devices = adb devices
Write-Host $devices

if ($devices -like "*device*" -and $devices -notlike "*offline*") {
    Write-Host "Dispositivo Android encontrado!" -ForegroundColor Green
    Write-Host ""
    
    # Compilar e instalar
    Write-Host "2. Compilando e instalando no dispositivo..." -ForegroundColor Yellow
    Write-Host "   Isso pode demorar 1-2 minutos na primeira vez..." -ForegroundColor Gray
    Write-Host ""
    
    dotnet build -t:Run -f net9.0-android
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "  SUCESSO! APP INSTALADO NO CELULAR!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Proximo passo:" -ForegroundColor Yellow
        Write-Host "  1. Verifique se a API Web esta rodando na porta 5000" -ForegroundColor Gray
        Write-Host "  2. Celular e PC devem estar na mesma rede Wi-Fi" -ForegroundColor Gray
        Write-Host "  3. Abra o app no celular e teste!" -ForegroundColor Gray
        Write-Host ""
        Write-Host "IP configurado: 192.168.200.107:5000" -ForegroundColor Cyan
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "ERRO ao compilar/instalar!" -ForegroundColor Red
        Write-Host "Verifique os erros acima." -ForegroundColor Yellow
    }
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "  NENHUM DISPOSITIVO ENCONTRADO!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Passos para conectar seu celular:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "1. NO CELULAR:" -ForegroundColor Cyan
    Write-Host "   - Ative Modo Desenvolvedor" -ForegroundColor Gray
    Write-Host "     (Configuracoes > Sobre > Toque 7x em 'Numero da compilacao')" -ForegroundColor Gray
    Write-Host "   - Ative Depuracao USB" -ForegroundColor Gray
    Write-Host "     (Configuracoes > Opcoes do desenvolvedor > Depuracao USB)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. CONECTE:" -ForegroundColor Cyan
    Write-Host "   - Conecte o celular no PC via cabo USB" -ForegroundColor Gray
    Write-Host "   - No celular, aceite 'Permitir depuracao USB?'" -ForegroundColor Gray
    Write-Host ""
    Write-Host "3. TENTE NOVAMENTE:" -ForegroundColor Cyan
    Write-Host "   - Execute: .\install-android.ps1" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Para mais detalhes, veja: SETUP-ANDROID.md" -ForegroundColor Yellow
    Write-Host ""
}
