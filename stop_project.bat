@echo off
setlocal EnableExtensions EnableDelayedExpansion

title CPTM - Stop Project

echo.
echo [1/2] Encerrando processos nas portas do projeto...

for %%P in (5173 5000 5001) do (
  set "FOUND=false"
  for /f "tokens=5" %%A in ('netstat -ano ^| findstr /R /C:":%%P .*LISTENING"') do (
    set "FOUND=true"
    echo - Porta %%P: finalizando PID %%A
    taskkill /PID %%A /F >nul 2>&1
  )

  if /I "!FOUND!"=="false" (
    echo - Porta %%P: nenhum processo em escuta
  )
)

echo.
echo [2/2] Projeto parado (se estava em execucao nas portas padrao).
echo.
exit /b 0
