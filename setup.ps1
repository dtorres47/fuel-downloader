<#
.SYNOPSIS
    Setup script for fuel-downloader development environment.

.DESCRIPTION
    Installs Go dependencies and common tools needed to build/test the project.
    Run this from the project root:  .\setup.ps1
#>

Write-Host "🚀 Setting up fuel-downloader environment..." -ForegroundColor Cyan

# Change to Go project folder
Set-Location "$PSScriptRoot\go"

# Ensure go.mod is tidy
Write-Host "→ Running go mod tidy..."
go mod tidy

# Install required dependency (Postgres driver)
Write-Host "→ Installing pgx..."
go get github.com/jackc/pgx/v5@latest

# Install optional global tools (formatter, linter)
Write-Host "→ Installing developer tools..."
go install golang.org/x/tools/cmd/goimports@latest
go install github.com/golangci/golangci-lint/cmd/golangci-lint@latest

Write-Host "✅ Setup complete. You can now build the project." -ForegroundColor Green
