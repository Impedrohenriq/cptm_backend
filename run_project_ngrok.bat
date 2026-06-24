@echo off
setlocal EnableExtensions

title CPTM - Run Project com ngrok fixo

set "BACKEND_DIR=%~dp0"
if "%BACKEND_DIR:~-1%"=="\" set "BACKEND_DIR=%BACKEND_DIR:~0,-1%"
for %%I in ("%BACKEND_DIR%\..") do set "ROOT_DIR=%%~fI"
set "FRONTEND_DIR=%ROOT_DIR%\CPTM_Frontend"

echo.
echo [1/5] Validando pastas do projeto...
if not exist "%BACKEND_DIR%\CPTM_Backend.csproj" (
  echo [ERRO] Backend nao encontrado em %BACKEND_DIR%
  exit /b 1
)
if not exist "%FRONTEND_DIR%\package.json" (
  echo [ERRO] Frontend nao encontrado em %FRONTEND_DIR%
  exit /b 1
)

echo [2/5] Registrando token do ngrok...
ngrok config add-authtoken 3EBikh1VIfCtLAu9XogPNrz381U_iPDDcGmNKh4Cn6PChz4u >nul 2>&1
if errorlevel 1 ngrok authtoken 3EBikh1VIfCtLAu9XogPNrz381U_iPDDcGmNKh4Cn6PChz4u >nul 2>&1

echo [3/5] Encerrando processos antigos nas portas 5000, 5001 e 5173...
for %%P in (5000 5001 5173) do (
  for /f "tokens=5" %%A in ('netstat -ano ^| findstr /R /C:":%%P .*LISTENING"') do (
    echo - Porta %%P: finalizando PID %%A
    taskkill /PID %%A /F >nul 2>&1
  )
)

echo [4/5] Iniciando backend em HTTP (porta 5000)...
start "CPTM Backend" cmd /k "cd /d ""%BACKEND_DIR%"" && dotnet run --launch-profile http"

echo Aguardando backend estabilizar...
timeout /t 8 /nobreak >nul

echo [5/5] Iniciando ngrok na URL fixa...
start "CPTM ngrok" cmd /k "ngrok http --url=skinning-grape-egomaniac.ngrok-free.dev 5000"

echo.
echo Backend local: http://localhost:5000
echo URL publica fixa: https://skinning-grape-egomaniac.ngrok-free.dev
echo Frontend Vercel esperado: https://cptm-frontend-five.vercel.app
echo.
echo Fluxo esperado:
echo Vercel ^> ngrok ^> backend local ^> Oracle local
echo.
echo Se quiser testar localmente o frontend:
echo cd /d ""%FRONTEND_DIR%"" ^&^& npm run dev -- --host
exit /b 0
