@echo off
setlocal EnableExtensions

title CPTM - Restaurar Modo Local

set "BACKEND_DIR=%~dp0"
if "%BACKEND_DIR:~-1%"=="\" set "BACKEND_DIR=%BACKEND_DIR:~0,-1%"
for %%I in ("%BACKEND_DIR%\..") do set "ROOT_DIR=%%~fI"
set "FRONTEND_ENV=%ROOT_DIR%\CPTM_Frontend\.env"
set "BACKEND_SETTINGS=%BACKEND_DIR%\appsettings.Development.json"

echo.
echo [1/4] Validando caminhos...
if not exist "%BACKEND_SETTINGS%" (
  echo [ERRO] Nao encontrei: %BACKEND_SETTINGS%
  exit /b 1
)
if not exist "%FRONTEND_ENV%" (
  echo [ERRO] Nao encontrei: %FRONTEND_ENV%
  exit /b 1
)

echo [2/4] Restaurando appsettings.Development.json para modo local...
(
  echo {
  echo   "ConnectionStrings": {
  echo     "OracleDB": "User Id=cptmapp;Password=Oracle@Cptm;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XEPDB1)));"
  echo   },
  echo   "Jwt": {
  echo     "Issuer": "CPTM.Backend",
  echo     "Audience": "CPTM.Frontend",
  echo     "Key": "DEV_ONLY_CHANGE_THIS_JWT_KEY_TO_AT_LEAST_32_CHARS"
  echo   }
  echo }
) > "%BACKEND_SETTINGS%"

echo [3/4] Restaurando .env do frontend para API local...
(
  echo # URL base da API .NET (ajuste para o IP/porta do servidor quando em producao)
  echo VITE_API_BASE_URL=https://localhost:5001
) > "%FRONTEND_ENV%"

echo [4/4] Concluido.
echo.
echo Projeto restaurado para modo local.
echo Proximo passo:
echo - Backend/Frontend: execute run_project.bat
exit /b 0
