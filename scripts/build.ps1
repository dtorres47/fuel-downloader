<#
.SYNOPSIS
    Build script for fuel-downloader Go project.

.DESCRIPTION
    Compiles the Go code into go/bin/fuel-latest.exe.
    Run this from the project root or directly from the scripts folder.
#>

# Move to Go project folder
Set-Location "$PSScriptRoot\..\go"

# Ensure dependencies are up to date
Write-Host "→ Tidying modules..."
go mod tidy

# Create bin folder if missing
if (-not (Test-Path ".\bin")) {
    New-Item -ItemType Directory -Path ".\bin" | Out-Null
}

# Build the executable
Write-Host "→ Building fuel-latest.exe..."
go build -o .\bin\fuel-latest.exe .\cmd\fuel-latest

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful: go\bin\fuel-latest.exe" -ForegroundColor Green
} else {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}
