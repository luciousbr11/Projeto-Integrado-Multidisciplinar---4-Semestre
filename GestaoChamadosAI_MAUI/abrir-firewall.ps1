# EXECUTE ESTE SCRIPT COMO ADMINISTRADOR
# Clique com botao direito > Executar como Administrador

Write-Host "Abrindo porta 5000 no Firewall do Windows..." -ForegroundColor Cyan

try {
    # Remover regra antiga se existir
    netsh advfirewall firewall delete rule name="GestaoChamadosAI API" 2>$null
    
    # Adicionar nova regra
    netsh advfirewall firewall add rule name="GestaoChamadosAI API" dir=in action=allow protocol=TCP localport=5000
    
    Write-Host ""
    Write-Host "Sucesso! Porta 5000 aberta no Firewall!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Agora seu celular podera acessar a API em:" -ForegroundColor Yellow
    Write-Host "http://192.168.200.107:5000" -ForegroundColor Cyan
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Host "Erro ao abrir porta no Firewall!" -ForegroundColor Red
    Write-Host "Execute este script como Administrador:" -ForegroundColor Yellow
    Write-Host "  1. Clique com botao direito no arquivo" -ForegroundColor Gray
    Write-Host "  2. Selecione 'Executar como Administrador'" -ForegroundColor Gray
    Write-Host ""
}

Read-Host "Pressione ENTER para fechar"
