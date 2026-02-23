$ErrorActionPreference = "Stop"

Write-Host "Installing requirements..."
.\venv\Scripts\pip install -r requirements.txt | Out-Null

Write-Host "Starting API server..."
$serverJob = Start-Job -ScriptBlock {
    & .\venv\Scripts\python.exe -m uvicorn main:app --host 0.0.0.0 --port 8000 > server_log.txt 2>&1
}

Start-Sleep -Seconds 10

Write-Host "Running tests..."
try {
    & .\venv\Scripts\python.exe test_api.py
    if ($LASTEXITCODE -ne 0) {
        throw "Tests failed with exit code $LASTEXITCODE"
    }
    Write-Host "Tests passed successfully!"
}
catch {
    Write-Host "Test execution failed. Server log:"
    if (Test-Path server_log.txt) {
        Get-Content server_log.txt
    }
    throw $_
}
finally {
    Write-Host "Stopping API server..."
    Stop-Job $serverJob
    Remove-Job $serverJob
    if (Test-Path server_log.txt) {
        Remove-Item server_log.txt
    }
}
