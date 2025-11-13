# ============================================
# Sincronização Automática de Uploads
# Monitora pasta Web e copia para API em tempo real
# ============================================

$sourceFolder = "C:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_Web\wwwroot\uploads"
$destinationFolder = "C:\wamp64\www\GestaoChamadosAI\GestaoChamadosAI_API\wwwroot\uploads"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Sincronização Automática de Uploads  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Origem:  $sourceFolder" -ForegroundColor Yellow
Write-Host "Destino: $destinationFolder" -ForegroundColor Yellow
Write-Host ""
Write-Host "Pressione Ctrl+C para parar..." -ForegroundColor Gray
Write-Host ""

# Criar pasta destino se não existir
if (-not (Test-Path $destinationFolder)) {
    New-Item -ItemType Directory -Path $destinationFolder -Force | Out-Null
    Write-Host "[CRIADO] Pasta de destino criada" -ForegroundColor Green
}

# Sincronização inicial
Write-Host "[SYNC] Sincronizando arquivos existentes..." -ForegroundColor Cyan
robocopy $sourceFolder $destinationFolder /MIR /NFL /NDL /NJH /NJS /R:3 /W:1
Write-Host "[OK] Sincronização inicial concluída!" -ForegroundColor Green
Write-Host ""

# Criar FileSystemWatcher
$watcher = New-Object System.IO.FileSystemWatcher
$watcher.Path = $sourceFolder
$watcher.Filter = "*.*"
$watcher.IncludeSubdirectories = $false
$watcher.EnableRaisingEvents = $true

# Função para copiar arquivo
function Copy-FileWithRetry {
    param(
        [string]$Source,
        [string]$Destination
    )
    
    $maxRetries = 5
    $retryCount = 0
    $success = $false
    
    while (-not $success -and $retryCount -lt $maxRetries) {
        try {
            # Aguardar um pouco para garantir que o arquivo foi completamente gravado
            Start-Sleep -Milliseconds 500
            
            Copy-Item -Path $Source -Destination $Destination -Force -ErrorAction Stop
            $success = $true
        }
        catch {
            $retryCount++
            if ($retryCount -lt $maxRetries) {
                Start-Sleep -Milliseconds 1000
            }
        }
    }
    
    return $success
}

# Event: Arquivo criado
$onCreated = Register-ObjectEvent -InputObject $watcher -EventName Created -Action {
    $file = $Event.SourceEventArgs.FullPath
    $fileName = $Event.SourceEventArgs.Name
    $destination = Join-Path $destinationFolder $fileName
    
    $timestamp = Get-Date -Format "HH:mm:ss"
    Write-Host "[$timestamp] [+] $fileName" -ForegroundColor Green
    
    if (Copy-FileWithRetry -Source $file -Destination $destination) {
        Write-Host "           [OK] Copiado para API" -ForegroundColor Gray
    }
    else {
        Write-Host "           [ERRO] Falha ao copiar" -ForegroundColor Red
    }
}

# Event: Arquivo modificado
$onChanged = Register-ObjectEvent -InputObject $watcher -EventName Changed -Action {
    $file = $Event.SourceEventArgs.FullPath
    $fileName = $Event.SourceEventArgs.Name
    $destination = Join-Path $destinationFolder $fileName
    
    $timestamp = Get-Date -Format "HH:mm:ss"
    Write-Host "[$timestamp] [~] $fileName" -ForegroundColor Yellow
    
    if (Copy-FileWithRetry -Source $file -Destination $destination) {
        Write-Host "           [OK] Atualizado na API" -ForegroundColor Gray
    }
    else {
        Write-Host "           [ERRO] Falha ao atualizar" -ForegroundColor Red
    }
}

# Event: Arquivo deletado
$onDeleted = Register-ObjectEvent -InputObject $watcher -EventName Deleted -Action {
    $fileName = $Event.SourceEventArgs.Name
    $destination = Join-Path $destinationFolder $fileName
    
    $timestamp = Get-Date -Format "HH:mm:ss"
    Write-Host "[$timestamp] [-] $fileName" -ForegroundColor Red
    
    if (Test-Path $destination) {
        Remove-Item -Path $destination -Force
        Write-Host "           [OK] Removido da API" -ForegroundColor Gray
    }
}

# Event: Arquivo renomeado
$onRenamed = Register-ObjectEvent -InputObject $watcher -EventName Renamed -Action {
    $oldName = $Event.SourceEventArgs.OldName
    $newName = $Event.SourceEventArgs.Name
    $oldDestination = Join-Path $destinationFolder $oldName
    $newDestination = Join-Path $destinationFolder $newName
    
    $timestamp = Get-Date -Format "HH:mm:ss"
    Write-Host "[$timestamp] [>] $oldName -> $newName" -ForegroundColor Magenta
    
    if (Test-Path $oldDestination) {
        Rename-Item -Path $oldDestination -NewName $newName -Force
        Write-Host "           [OK] Renomeado na API" -ForegroundColor Gray
    }
}

Write-Host "Monitoramento ativo! Aguardando alterações..." -ForegroundColor Green
Write-Host ""

# Loop infinito
try {
    while ($true) {
        Start-Sleep -Seconds 1
    }
}
finally {
    # Cleanup
    Unregister-Event -SourceIdentifier $onCreated.Name
    Unregister-Event -SourceIdentifier $onChanged.Name
    Unregister-Event -SourceIdentifier $onDeleted.Name
    Unregister-Event -SourceIdentifier $onRenamed.Name
    $watcher.Dispose()
    Write-Host ""
    Write-Host "Monitoramento encerrado." -ForegroundColor Yellow
}
