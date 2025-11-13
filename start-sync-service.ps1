# ============================================
# Iniciar Sincronização como Serviço
# Executa em segundo plano sem bloquear terminal
# ============================================

$scriptPath = "C:\wamp64\www\GestaoChamadosAI\sync-uploads-watch.ps1"

Write-Host "Iniciando sincronização em segundo plano..." -ForegroundColor Cyan

# Verificar se já está rodando
$existing = Get-Job -Name "UploadSync" -ErrorAction SilentlyContinue
if ($existing) {
    Write-Host "Sincronização já está rodando!" -ForegroundColor Yellow
    Write-Host "Use 'stop-sync-service.ps1' para parar." -ForegroundColor Gray
    exit
}

# Iniciar como Job em segundo plano
Start-Job -Name "UploadSync" -ScriptBlock {
    & $using:scriptPath
} | Out-Null

Write-Host "[OK] Sincronização iniciada em segundo plano!" -ForegroundColor Green
Write-Host ""
Write-Host "Comandos úteis:" -ForegroundColor Yellow
Write-Host "  Ver status:  Get-Job -Name UploadSync" -ForegroundColor Gray
Write-Host "  Ver logs:    Receive-Job -Name UploadSync -Keep" -ForegroundColor Gray
Write-Host "  Parar:       .\stop-sync-service.ps1" -ForegroundColor Gray
