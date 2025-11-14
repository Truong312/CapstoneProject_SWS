<#
run_windows.ps1 - Start uvicorn (FastAPI) and a static HTTP server for test.html on Windows PowerShell
Usage: .\run_windows.ps1 [-Model <string>] [-Compute <string>]
Example: .\run_windows.ps1 -Model small -Compute ""
#>
param(
    [string]$Model = "small",
    [string]$Compute = ""
)

$Port = 8001
$StaticPort = 8080

# Move to script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "Using MODEL=$Model, COMPUTE='$Compute'"

# Kill any process listening on the port
try {
    $listeners = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction Stop
    foreach ($conn in $listeners) {
        $pid = $conn.OwningProcess
        Write-Host "Killing PID $pid on :$Port"
        Stop-Process -Id $pid -Force -ErrorAction SilentlyContinue
    }
} catch {
    # Get-NetTCPConnection may not exist on older Windows; ignore failures
}

# Ensure virtualenv exists and install requirements if needed
if (-not (Test-Path -Path ".venv\Scripts\Activate.ps1")) {
    Write-Host "Creating virtualenv .venv and installing requirements..."
    python -m venv .venv
    .\.venv\Scripts\Activate.ps1
    python -m pip install --upgrade pip setuptools wheel
    python -m pip install -r requirements.txt
} else {
    Write-Host "Found virtualenv .venv"
}

# Set env for this session
$env:WHISPER_MODEL = $Model
$env:WHISPER_COMPUTE = $Compute

# Start uvicorn as a background process and redirect output to uvicorn.log
$uvicornExe = Join-Path $PSScriptRoot ".venv\Scripts\uvicorn.exe"
if (-not (Test-Path $uvicornExe)) {
    # If uvicorn.exe doesn't exist, try using module via python -m
    $uvCmd = "python -m uvicorn main:app --host 127.0.0.1 --port $Port --log-level info"
    Write-Host "Starting uvicorn via python -m uvicorn"
    Start-Process -FilePath "powershell" -ArgumentList "-NoProfile","-Command","$env:WHISPER_MODEL='$Model'; $env:WHISPER_COMPUTE='$Compute'; $uvCmd > uvicorn.log 2>&1" -WindowStyle Hidden
} else {
    $args = "main:app --host 127.0.0.1 --port $Port --log-level info"
    if ($Compute) {
        Start-Process -FilePath $uvicornExe -ArgumentList $args -RedirectStandardOutput uvicorn.log -RedirectStandardError uvicorn.log -WindowStyle Hidden
    } else {
        Start-Process -FilePath $uvicornExe -ArgumentList $args -RedirectStandardOutput uvicorn.log -RedirectStandardError uvicorn.log -WindowStyle Hidden
    }
}
Write-Host "uvicorn started (logs -> uvicorn.log)"

# Start static http.server for test.html
Write-Host "Starting static server on port $StaticPort (serving test.html)"
Start-Process -FilePath "python" -ArgumentList "-m http.server $StaticPort" -RedirectStandardOutput httpserver.log -RedirectStandardError httpserver.log -WindowStyle Hidden
Write-Host "Static server started (logs -> httpserver.log)"

Write-Host "API: http://127.0.0.1:$Port/"
Write-Host "Test page: http://127.0.0.1:$StaticPort/test.html"
Write-Host "To view uvicorn logs in real time (PowerShell): Get-Content uvicorn.log -Wait"

