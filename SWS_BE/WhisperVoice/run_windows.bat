@echo off
REM run_windows.bat - Start FastAPI uvicorn and static server (Windows CMD)
REM Usage: run_windows.bat [MODEL] [COMPUTE]
REM Example: run_windows.bat small float16

setlocal EnableDelayedExpansion
set PORT=8001
set STATIC_PORT=8080
set MODEL=%1
set COMPUTE=%2
if "%MODEL%"=="" set MODEL=small
if "%COMPUTE%"=="" set COMPUTE=

cd /d "%~dp0"

echo Using MODEL=%MODEL%, COMPUTE=%COMPUTE%

REM Kill process listening on PORT (Windows)
for /f "tokens=5" %%a in ('netstat -ano ^| findstr /R /C:":%PORT% .*LISTENING"') do (
  echo Killing PID %%a on port %PORT%
  taskkill /PID %%a /F >nul 2>&1 || echo Failed to kill %%a
)

REM Create virtualenv if missing
if not exist ".venv\Scripts\activate" (
  echo Creating virtualenv and installing requirements...
  python -m venv .venv
  call .venv\Scripts\activate
  python -m pip install --upgrade pip setuptools wheel
  python -m pip install -r requirements.txt
) else (
  echo Found .venv
)

REM Activate venv for subsequent commands
call .venv\Scripts\activate

REM Set environment variables for this process
set WHISPER_MODEL=%MODEL%
set WHISPER_COMPUTE=%COMPUTE%

REM Start uvicorn in background, redirect logs
if "%COMPUTE%"=="" (
  start "uvicorn" /B cmd /C ".venv\Scripts\uvicorn.exe main:app --host 127.0.0.1 --port %PORT% --log-level info > uvicorn.log 2>&1"
) else (
  start "uvicorn" /B cmd /C "set WHISPER_MODEL=%MODEL%&& set WHISPER_COMPUTE=%COMPUTE%&& .venv\Scripts\uvicorn.exe main:app --host 127.0.0.1 --port %PORT% --log-level info > uvicorn.log 2>&1"
)

echo Starting static file server on port %STATIC_PORT% (serves test.html)
start "http" /B cmd /C "python -m http.server %STATIC_PORT% > httpserver.log 2>&1"

echo Servers started.
echo API: http://127.0.0.1:%PORT%/
echo Test page: http://127.0.0.1:%STATIC_PORT%/test.html

echo To view logs (uvicorn):
echo   powershell -NoProfile -Command "Get-Content uvicorn.log -Wait"

echo Press any key to exit this window (services continue running in background)...
pause >nul

endlocal

