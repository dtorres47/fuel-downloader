<#
.SYNOPSIS
    Lint and build the fuel-downloader project.

.DESCRIPTION
    Runs golangci-lint and, if clean, compiles the fuel-latest binary.
#>

Set-Location "$PSScriptRoot\..\go"

Write-Host "→ Running linter..."
golangci-lint run ./...
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Lint failed. Fix issues before building." -ForegroundColor Red
    exit 1
}

Write-Host "→ Building fuel-latest.exe..."
go build -o .\bin\fuel-latest.exe .\cmd\fuel-latest
Write-Host "✅ Build successful: go\bin\fuel-latest.exe" -ForegroundColor Green