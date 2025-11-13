@echo off
echo ========================================
echo   ABRINDO FIREWALL PORTA 5000
echo ========================================
echo.
echo Adicionando regra de firewall...
netsh advfirewall firewall add rule name="API GestaoChamados Port 5000" dir=in action=allow protocol=TCP localport=5000
echo.
if %errorlevel% == 0 (
    echo ========================================
    echo   SUCESSO! FIREWALL CONFIGURADO!
    echo ========================================
    echo.
    echo Agora teste no Chrome do celular:
    echo http://192.168.200.107:5000/api
) else (
    echo ========================================
    echo   ERRO! Execute como ADMINISTRADOR
    echo ========================================
    echo.
    echo Clique com botao direito neste arquivo
    echo e escolha "Executar como administrador"
)
echo.
pause
