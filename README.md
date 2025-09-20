# fuel-downloader

A portfolio-ready Go project that fetches the latest U.S. diesel fuel price from the [EIA API](https://www.eia.gov/petroleum/gasdiesel/), upserts it into Postgres, and exports a CSV to your Desktop.  
This project demonstrates **clean architecture, database integration, API consumption, and automation scripts**.  

---

## 🚀 Features
- **Enterprise-style architecture** (domain, infra, usecase, cmd).  
- **Postgres persistence** with upsert logic.  
- **CSV export** with clean headers and UTC timestamp.  
- **Migration tool** (`migrate`) to apply schema without `psql`.  
- **Helper scripts** for setup, build, lint, and test.  
- **Environment-driven configuration** (no secrets in code).  

---

## 📂 Project Structure
```
fuel-downloader/
├── go/                         # Go implementation
│   ├── go.mod
│   ├── cmd/
│   │   ├── fuel-latest/        # Main app (fetch → upsert → export)
│   │   └── migrate/            # Migration helper (applies SQL schema)
│   └── internal/
│       ├── domain/             # Core entity
│       ├── infra/              # External systems (EIA, Postgres, CSV)
│       └── usecase/            # Orchestration logic
│
├── db/
│   └── eia_fuel_price.sql      # Schema (idempotent, safe to rerun)
│
├── scripts/
│   ├── fuel-latest.bat         # Double-click to run app
│   ├── build.ps1               # Build binary
│   ├── lint-build.ps1          # Lint + build
│   └── test.ps1                # Lint + build + run end-to-end
│
├── .gitignore
└── (future) csharp/            # Parallel .NET implementation
```

---

## 🔧 Prerequisites
- [Go 1.22+](https://go.dev/dl/)  
- PostgreSQL (local or Docker)  
- EIA API Key (free from [EIA Open Data](https://www.eia.gov/opendata/))  

---

## ⚙️ Setup

### 1. Environment Variables
Set in **Windows** (global) or in **GoLand run/debug config** (local):

```
EIA_API_KEY=your-real-api-key
FUEL_DSN=postgres://postgres:postgres@localhost:5433/fuel?sslmode=disable
FUEL_AREA=NUS
FUEL_OUT=C:\Users\<YourUser>\Desktop\fuel-latest.csv
```

- `EIA_API_KEY` → required.  
- `FUEL_DSN` → required. Connection string for Postgres.  
- `FUEL_AREA` → optional, defaults to `NUS` (U.S. aggregate).  
- `FUEL_OUT` → optional, defaults to `Desktop\fuel-latest.csv`.  

---

### 2. Install Dependencies
From the `go` folder:

```powershell
cd V:\Coding\fuel-downloader\go

go mod tidy
go get github.com/jackc/pgx/v5@latest

# Optional dev tools
go install golang.org/x/tools/cmd/goimports@latest
go install github.com/golangci/golangci-lint/cmd/golangci-lint@latest
```

---

### 3. Apply Database Migration
Run the migration helper:

```powershell
cd V:\Coding\fuel-downloader\go
go run .\cmd\migrate
```

or build/run it:

```powershell
go build -o .\bin\migrate.exe .\cmd\migrate
.\bin\migrate.exe
```

This applies `db\eia_fuel_price.sql` automatically.  

---

### 4. Run the App
From GoLand:  
- Select the `fuel-latest` run/debug config.  
- Make sure env vars are set.  
- Run → CSV appears on your Desktop.  

From PowerShell:  
```powershell
cd V:\Coding\fuel-downloader\go
go run .\cmd\fuel-latest
```

From double-click:  
- Use `scripts\fuel-latest.bat`.  
- If env vars are missing, it shows an error and pauses.  

---

## ▶️ Helper Scripts
- `scripts\build.ps1` → Build binary into `go/bin/`.  
- `scripts\lint-build.ps1` → Lint then build.  
- `scripts\test.ps1` → Lint + build + run full workflow.  
- `scripts\fuel-latest.bat` → Double-click to fetch latest price into CSV.  

---

## 📊 Example Output
Generated `fuel-latest.csv`:

```
product_code,product_name,area_code,area_name,period,value,unit,generated_utc
EPD2D,No 2 Diesel,NUS,U.S.,2025-08,3.744,$/GAL,2025-09-20T06:39:46Z
```
