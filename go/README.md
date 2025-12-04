# Fuel Downloader - Go Implementation

A command-line tool that retrieves the latest diesel fuel prices from the U.S. Energy Information Administration (EIA) API, stores them in PostgreSQL, and exports to CSV.

**This is a Go translation of the C# implementation**, demonstrating cross-language architectural thinking and idiomatic Go patterns while maintaining the same vertical slice architecture.

## Why This Project Exists

This project solves a **real business problem**: I create freight invoices for my corporation that require current diesel fuel prices from the [EIA website](https://www.eia.gov/petroleum/gasdiesel/). Instead of manually looking up prices each time, this tool automates the entire workflow—fetching, persisting, and exporting data for invoicing.

## Architecture

Built using **Vertical Slice Architecture** with idiomatic Go patterns:

```
├── domain/          # Core entities (FuelRate)
├── infra/           # External concerns (EIA API, PostgreSQL, CSV)
│   ├── eia/         # HTTP client for EIA API
│   ├── postgres/    # Database repository (pgx)
│   └── csv/         # CSV export logic
├── usecase/         # Business logic (GetLatest)
└── cmd/             # Entry points (fuel-latest, migrate)
```

The structure mirrors the C# implementation while following Go conventions and best practices.

## Current Implementation

The Go version implements the core ELT pipeline with:
- ✅ EIA API integration with Chi router patterns
- ✅ PostgreSQL persistence using pgx/v5
- ✅ CSV export functionality
- ✅ Vertical slice architecture
- ✅ Error handling with wrapped errors
- 🔄 Database migration tooling (in progress)
- 🔄 Retry logic for resilience (planned)
- 🔄 Comprehensive unit tests (planned)

The C# implementation serves as the reference architecture, with the Go version demonstrating the same design patterns translated to idiomatic Go.

## Features

- Fetches latest diesel prices from EIA API
- Stores data in PostgreSQL with automatic upserts
- Exports to CSV format for invoicing
- Structured logging
- Input validation
- Clean architecture with dependency injection

## Prerequisites

- Go 1.22+
- PostgreSQL 16+
- EIA API key (free from https://www.eia.gov/opendata/)

## Quick Start

### 1. Set Environment Variables

**PowerShell:**
```powershell
$env:FUEL_DSN = "postgres://postgres:postgres@localhost:5432/fuel?sslmode=disable"
$env:EIA_API_KEY = "your_api_key_here"
$env:FUEL_OUT = "C:\path\to\fuel-latest.csv"
$env:FUEL_AREA = "NUS"
```

**Bash:**
```bash
export FUEL_DSN="postgres://postgres:postgres@localhost:5432/fuel?sslmode=disable"
export EIA_API_KEY="your_api_key_here"
export FUEL_OUT="/path/to/fuel-latest.csv"
export FUEL_AREA="NUS"
```

### 2. Install Dependencies

```bash
cd go
go mod tidy
go get github.com/jackc/pgx/v5@latest
```

### 3. Run Database Migration

```bash
go run ./cmd/migrate
```

Or build and run:
```bash
go build -o ./bin/migrate ./cmd/migrate
./bin/migrate
```

### 4. Fetch Latest Fuel Prices

```bash
go run ./cmd/fuel-latest
```

Or use the provided batch script:
```bash
./scripts/fuel-latest.bat
```

### 5. Run Tests (when implemented)

```bash
go test ./...
```

## Valid Area Codes

- `NUS` - U.S. National
- `PADD1`, `PADD1A`, `PADD1B`, `PADD1C` - East Coast
- `PADD2` - Midwest
- `PADD3` - Gulf Coast
- `PADD4` - Rocky Mountain
- `PADD5` - West Coast

## Technology Stack

- **Go 1.22+** - Language runtime
- **Chi** - Lightweight HTTP router
- **pgx/v5** - PostgreSQL driver and toolkit
- **Standard library** - CSV, HTTP, logging
- **Vertical Slice Architecture** - Feature-focused organization

## Project Structure

```
go/
├── domain/
│   └── fuelrate.go              # Core domain model
├── infra/
│   ├── eia/
│   │   └── client.go            # EIA API client
│   ├── postgres/
│   │   └── repo.go              # Database repository
│   └── csv/
│       └── writer.go            # CSV export
├── usecase/
│   └── getlatest/
│       └── executor.go          # Main business logic
├── cmd/
│   ├── fuel-latest/
│   │   └── main.go              # CLI entry point
│   └── migrate/
│       └── main.go              # Database migration
├── scripts/
│   └── fuel-latest.bat          # Windows batch helper
├── go.mod
└── go.sum
```

## Sample Output

**Console:**
```
2025/12/04 15:30:00 INFO Fetching latest diesel price area=NUS
2025/12/04 15:30:01 INFO Successfully processed fuel rate product=EPD2D period=2025-08 value=3.744
EPD2D 2025-08 3.744 $/GAL → fuel-latest.csv
```

**CSV File:**
```csv
product_code,product_name,area_code,area_name,period,value,unit,generated_utc
EPD2D,No 2 Diesel,NUS,U.S.,2025-08,3.744,$/GAL,2025-12-04T15:30:01Z
```

**Database:**
```sql
SELECT * FROM eia.fuel_price;
-- Shows stored fuel price data with timestamps
```

## Key Design Patterns

- **Vertical Slice Architecture** - Feature-focused organization
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Constructor injection for loose coupling
- **Error Wrapping** - Go 1.13+ error handling with context
- **Interface Segregation** - Small, focused interfaces

## Design Philosophy

This Go implementation demonstrates:
1. **Cross-Language Architecture** - Same design patterns as C# version
2. **Idiomatic Go** - Following Go conventions and best practices
3. **Production-Ready Code** - Not a tutorial, but a real solution
4. **Learning Journey** - C# → Go translation to prove adaptability

## Comparison with C# Implementation

| Aspect | C# | Go |
|--------|----|----|
| HTTP Client | HttpClient | net/http |
| Database | Npgsql + Dapper | pgx/v5 |
| DI Container | Microsoft.Extensions.DI | Constructor injection |
| Retry Logic | Polly | (Planned) Custom/library |
| Testing | MSTest + Moq | testing + testify (planned) |
| Error Handling | Exceptions | Error values + wrapping |

Both implementations follow the same vertical slice architecture with equivalent business logic.

## Future Enhancements

The Go implementation will continue to evolve with:
- Comprehensive unit test coverage
- Retry logic for transient failures
- Structured logging with slog
- API endpoints for frontend integration
- Docker containerization
- GitHub Actions CI/CD

## Related Projects

- **C# Implementation** - `/csharp` - Reference architecture
- **Angular UI** - `/angular` - Web interface (planned integration)
- **API Layer** - RESTful endpoints (planned)

---

*This Go implementation proves cross-language architectural thinking by translating the C# design patterns into idiomatic Go while maintaining the same business logic and vertical slice structure.*
