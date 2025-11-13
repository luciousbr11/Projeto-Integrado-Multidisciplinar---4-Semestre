# ============================================
# Parar Sincronização
# Para o serviço de sincronização em segundo plano
# ============================================

Write-Host "Parando sincronização..." -ForegroundColor Cyan

$job = Get-Job -Name "UploadSync" -ErrorAction SilentlyContinue

if ($job) {
    Stop-Job -Name "UploadSync"
    Remove-Job -Name "UploadSync"
    Write-Host "[OK] Sincronização parada!" -ForegroundColor Green
}
else {
    Write-Host "[INFO] Nenhuma sincronização em execução." -ForegroundColor Yellow
}
