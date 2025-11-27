# Fuel Downloader - C# Implementation

A command-line tool that gets the latest diesel fuel prices from the U.S. Energy Information Administration (EIA) API, stores them in PostgreSQL, and exports to CSV.

## Architecture

Built using **Vertical Slice Architecture**:

```
├── Domain/          # Core entities (FuelRate)
├── Infra/           # External concerns (EIA API, PostgreSQL, CSV)
├── UseCase/         # Business logic (GetLatest)
└── Cmd/             # Entry points (FuelLatest, Migrate)
```

## Features

- Gets latest diesel prices from EIA API
- Stores data in PostgreSQL with automatic upserts
- Exports to CSV format
- Retry logic for database operations (Polly)
- Structured logging
- Input validation
- Unit tested

## Prerequisites

- .NET 8 SDK
- PostgreSQL 16
- EIA API key (free from https://www.eia.gov/opendata/)

## Quick Start

### 1. Set Environment Variables

**PowerShell:**
```powershell
$env:FUEL_DSN = "postgres://postgres:postgres@localhost:5432/fuel?sslmode=disable"
$env:EIA_API_KEY = "your_api_key_here"
$env:FUEL_OUT = "fuel-latest.csv"
$env:FUEL_AREA = "NUS"
```

**Bash:**
```bash
export FUEL_DSN="postgres://postgres:postgres@localhost:5432/fuel?sslmode=disable"
export EIA_API_KEY="your_api_key_here"
export FUEL_OUT="fuel-latest.csv"
export FUEL_AREA="NUS"
```

### 2. Run Database Migration

```bash
dotnet run --project src/FuelDownloader.Cmd.Migrate/FuelDownloader.Cmd.Migrate.csproj
```

### 3. Fetch Latest Fuel Prices

```bash
dotnet run --project src/FuelDownloader.Cmd.FuelLatest/FuelDownloader.Cmd.FuelLatest.csproj
```

### 4. Run Tests

```bash
dotnet test
```

## Valid Area Codes

- `NUS` - U.S. National
- `PADD1`, `PADD1A`, `PADD1B`, `PADD1C` - East Coast
- `PADD2` - Midwest
- `PADD3` - Gulf Coast
- `PADD4` - Rocky Mountain
- `PADD5` - West Coast

## Technology Stack

- **.NET 8** - LTS framework
- **Npgsql** - PostgreSQL data provider
- **Dapper** - Micro-ORM for database access
- **Polly** - Resilience and retry policies
- **MS Test** - Unit testing framework
- **Moq** - Mocking framework

## Project Structure

```
FuelDownloader.sln
├── Domain/                    # Core domain model
│   └── FuelRate.cs
├── Infra/                     # Infrastructure layer
│   ├── Eia/Client.cs         # EIA API client
│   ├── Postgres/Repo.cs      # Database repository
│   └── CsvExport/Writer.cs   # CSV export
├── UseCase/                   # Application logic
│   └── GetLatest/Executor.cs # Main use case
├── Cmd.FuelLatest/           # CLI entry point
│   └── Program.cs
├── Cmd.Migrate/              # Database migration tool
│   └── Program.cs
└── Tests/                    # Unit tests
    ├── ExecutorTests.cs
    └── ClientTests.cs
```

## Sample Output

**Console:**
```
info: Fetching latest diesel price for area NUS
info: Successfully processed fuel rate: EPD2D 2025-08 3.744
EPD2D 2025-08 3.744 $/GAL → fuel-latest.csv
```

**CSV File:**
```csv
product_code,product_name,area_code,area_name,period,value,unit,generated_utc
EPD2D,No 2 Diesel,NUS,U.S.,2025-08,3.7440,$/GAL,2025-10-27T20:30:00.0000000Z
```

**Database:**
```sql
SELECT * FROM eia.fuel_price;
-- Shows stored fuel price data with timestamps
```

## Key Design Patterns

- **Vertical Slice Architecture** - Feature-focused organization
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling
- **Result Pattern** - Explicit success/failure handling
- **Retry Pattern** - Resilience for transient failures

## Next Steps

This C# implementation serves as the reference for:
1. **Go Translation** - Implementing the same architecture in Go
2. **Angular UI** - Building a web interface
3. **API Layer** - RESTful endpoints for the frontend

See the Go implementation in `/go` directory for the translated version.