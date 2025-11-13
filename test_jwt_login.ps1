# Script de teste para autenticação JWT

Write-Host "=== TESTE DE LOGIN JWT ===" -ForegroundColor Cyan
Write-Host ""

# 1. Testar Login
Write-Host "1. Fazendo login..." -ForegroundColor Yellow
try {
    $loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/Auth/login" `
        -Method POST `
        -Body '{"email":"admin@teste.com","senha":"admin123"}' `
        -ContentType "application/json"
    
    Write-Host "✅ Login bem-sucedido!" -ForegroundColor Green
    Write-Host "Token: $($loginResponse.data.token.Substring(0, 50))..." -ForegroundColor Gray
    Write-Host "Usuário: $($loginResponse.data.usuario.nome) ($($loginResponse.data.usuario.tipo))" -ForegroundColor Gray
    Write-Host ""
    
    $token = $loginResponse.data.token
    
    # 2. Testar endpoint protegido COM token
    Write-Host "2. Testando AssumirAtendimento COM token..." -ForegroundColor Yellow
    try {
        $headers = @{
            "Authorization" = "Bearer $token"
            "Content-Type" = "application/json"
        }
        
        $assumirResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/Chat/AssumirAtendimento" `
            -Method POST `
            -Headers $headers `
            -Body '{"ChamadoId":4}'
        
        Write-Host "✅ Endpoint respondeu!" -ForegroundColor Green
        Write-Host "Success: $($assumirResponse.success)" -ForegroundColor Gray
        Write-Host "Message: $($assumirResponse.message)" -ForegroundColor Gray
        Write-Host ""
    }
    catch {
        Write-Host "❌ Erro ao chamar endpoint: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        Write-Host ""
    }
    
    # 3. Testar endpoint protegido SEM token
    Write-Host "3. Testando AssumirAtendimento SEM token..." -ForegroundColor Yellow
    try {
        $semTokenResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/Chat/AssumirAtendimento" `
            -Method POST `
            -Body '{"ChamadoId":4}' `
            -ContentType "application/json"
        
        Write-Host "⚠️ Endpoint permitiu acesso sem token (ERRO DE SEGURANÇA)" -ForegroundColor Yellow
    }
    catch {
        if ($_.Exception.Response.StatusCode.value__ -eq 401) {
            Write-Host "✅ Bloqueado corretamente (401 Unauthorized)" -ForegroundColor Green
        } else {
            Write-Host "❌ Erro inesperado: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        }
        Write-Host ""
    }
}
catch {
    Write-Host "❌ Erro no login: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

Write-Host "=== TESTE CONCLUÍDO ===" -ForegroundColor Cyan
