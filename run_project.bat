@echo off
setlocal EnableExtensions

title CPTM - Run Project

set "BACKEND_DIR=%~dp0"
if "%BACKEND_DIR:~-1%"=="\" set "BACKEND_DIR=%BACKEND_DIR:~0,-1%"
for %%I in ("%BACKEND_DIR%\..") do set "ROOT_DIR=%%~fI"
set "FRONTEND_DIR=%ROOT_DIR%\CPTM_Frontend"

echo.
echo [1/3] Validando pastas do projeto...
if not exist "%BACKEND_DIR%\CPTM_Backend.csproj" (
  echo [ERRO] Backend nao encontrado em %BACKEND_DIR%
  exit /b 1
)
if not exist "%FRONTEND_DIR%\package.json" (
  echo [ERRO] Frontend nao encontrado em %FRONTEND_DIR%
  exit /b 1
)

echo [2/3] Iniciando backend...
start "CPTM Backend" cmd /k "cd /d "%BACKEND_DIR%" && dotnet run"

echo [3/3] Iniciando frontend...
start "CPTM Frontend" cmd /k "cd /d "%FRONTEND_DIR%" && npm run dev -- --host"

echo.
echo Projeto iniciado em janelas separadas.
echo Backend: http://localhost:5000
echo Frontend: http://localhost:5173
echo.
echo Para parar: feche as janelas "CPTM Backend" e "CPTM Frontend".
exit /b 0
