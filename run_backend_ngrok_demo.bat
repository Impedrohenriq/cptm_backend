@echo off
setlocal EnableExtensions

title CPTM - Backend + Ngrok (Demo)

set "BACKEND_DIR=%~dp0"
if "%BACKEND_DIR:~-1%"=="\" set "BACKEND_DIR=%BACKEND_DIR:~0,-1%"
set "NGROK_EXE="

for /f "delims=" %%I in ('where ngrok 2^>nul') do (
  set "NGROK_EXE=%%I"
  goto :found_ngrok
)

for /d %%D in ("%LOCALAPPDATA%\Microsoft\WinGet\Packages\Ngrok.Ngrok_*") do (
  if exist "%%~fD\ngrok.exe" (
    set "NGROK_EXE=%%~fD\ngrok.exe"
    goto :found_ngrok
  )
)

:found_ngrok

echo.
echo [1/3] Validando backend...
if not exist "%BACKEND_DIR%\CPTM_Backend.csproj" (
  echo [ERRO] Backend nao encontrado em %BACKEND_DIR%
  exit /b 1
)

if "%NGROK_EXE%"=="" (
  echo [ERRO] Ngrok nao encontrado.
  echo Instale com: winget install --id Ngrok.Ngrok --exact --source winget
  exit /b 1
)

if not exist "%NGROK_EXE%" (
  echo [ERRO] Caminho do ngrok invalido: %NGROK_EXE%
  echo Instale com: winget install --id Ngrok.Ngrok --exact --source winget
  exit /b 1
)

echo [2/3] Encerrando processos antigos nas portas 5000 e 5001...
for %%P in (5000 5001) do (
  for /f "tokens=5" %%A in ('netstat -ano ^| findstr /R /C:":%%P .*LISTENING"') do (
    echo - Porta %%P: finalizando PID %%A
    taskkill /PID %%A /F >nul 2>&1
  )
)

echo [3/3] Iniciando backend HTTP local...
start "CPTM Backend (HTTP)" cmd /k "cd /d "%BACKEND_DIR%" && dotnet run --launch-profile http"

echo Aguardando backend estabilizar...
timeout /t 6 /nobreak >nul

echo Iniciando ngrok para a porta 5000...
start "CPTM Ngrok" cmd /k ""%NGROK_EXE%" http 5000"

echo.
echo Demo iniciada.
echo 1) Veja a janela "CPTM Ngrok" e copie a URL HTTPS Forwarding.
echo 2) Configure essa URL no frontend publicado como base da API.
echo 3) Mantenha este PC ligado durante toda a apresentacao.
echo.
exit /b 0
