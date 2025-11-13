# Script para sincronizar pasta uploads entre Web e API
$source = "C:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_Web\wwwroot\uploads"
$destination = "C:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_API\wwwroot\uploads"

Write-Host "🔄 Sincronizando uploads..." -ForegroundColor Cyan

# Copiar todos os arquivos mais recentes
robocopy $source $destination /MIR /NFL /NDL /NJH /NJS

Write-Host "✅ Sincronização completa!" -ForegroundColor Green
