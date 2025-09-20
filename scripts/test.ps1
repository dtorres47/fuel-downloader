<#
.SYNOPSIS
    Lint, build, and run the fuel-downloader app.

.DESCRIPTION
    Runs golangci-lint, builds the binary, and executes it with environment variables.
    Outputs the latest CSV to Desktop (or FUEL_OUT if defined).
#>

Set-Location "$PSScriptRoot\..\go"

Write-Host "→ Running linter..."
golangci-lint run ./...
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Lint failed. Fix issues before building." -ForegroundColor Red
    pause
    exit 1
}

Write-Host "→ Building fuel-latest.exe..."
if (-not (Test-Path ".\bin")) {
    New-Item -ItemType Directory -Path ".\bin" | Out-Null
}
go build -o .\bin\fuel-latest.exe .\cmd\fuel-latest
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed." -ForegroundColor Red
    pause
    exit 1
}

Write-Host "→ Running app..."
.\bin\fuel-latest.exe
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ App failed." -ForegroundColor Red
    pause
    exit 1
}

Write-Host "✅ Test successful. CSV should be on your Desktop." -ForegroundColor Green
pause