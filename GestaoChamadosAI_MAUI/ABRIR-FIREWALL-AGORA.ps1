# ============================================
#  ABRIR FIREWALL PARA PORTA 5000
#  Execute como ADMINISTRADOR!
# ============================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  CONFIGURANDO FIREWALL DO WINDOWS" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Remover regra antiga se existir
Write-Host "1. Removendo regras antigas..." -ForegroundColor White
netsh advfirewall firewall delete rule name="API GestaoChamados Port 5000" | Out-Null

# Adicionar nova regra
Write-Host "2. Criando nova regra de firewall..." -ForegroundColor White
$result = netsh advfirewall firewall add rule name="API GestaoChamados Port 5000" dir=in action=allow protocol=TCP localport=5000

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  FIREWALL CONFIGURADO COM SUCESSO!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "A porta 5000 agora aceita conexoes externas!" -ForegroundColor White
    Write-Host ""
    Write-Host "Proximos passos:" -ForegroundColor Cyan
    Write-Host "  1. Certifique-se que a API esta rodando" -ForegroundColor White
    Write-Host "  2. No celular, abra o Chrome" -ForegroundColor White
    Write-Host "  3. Acesse: http://192.168.200.107:5000/api" -ForegroundColor Yellow
    Write-Host "  4. Deve aparecer uma pagina ou JSON" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "  ERRO AO CONFIGURAR FIREWALL!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Execute este script como ADMINISTRADOR!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Clique com botao direito no arquivo e escolha:" -ForegroundColor White
    Write-Host "  'Executar com o PowerShell' como Administrador" -ForegroundColor Cyan
    Write-Host ""
}

Write-Host ""
Write-Host "Pressione qualquer tecla para fechar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
