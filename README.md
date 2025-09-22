# fuel-downloader

This repository showcases my **methodology for skilling up in Go** while **demonstrating my skills across multiple stacks**.  
I'm structuring this project to reflect how I **learn, translate, and apply concepts** across languages — from my strong .NET and Angular background into Go.  

This project started from a **real-world need in my own work**.  

I create freight invoices for my corporation, **manually** referencing the [EIA website](https://www.eia.gov/petroleum/gasdiesel/) to look up diesel fuel prices.  

The **fuel-downloader** automates that task:  
- It fetches the latest U.S. diesel price from the EIA API.  
- It persists the data in Postgres for historical tracking.  
- It exports a CSV for immediate use in invoicing.  

---

## 🌱 Methodology (Go-Centric)

This project demonstrates how I can implement a Go API across stacks.
- It uses **scaffolded enterprise architecture** (vertical slice style: domain, infra, usecase, cmd).  
- One microservice (API) is anchored using a **C# (.NET)** design.  
- A second microservice is **translateed from .NET to Go**.  
- I provide an **Angular front-end** to provide a tangible UI.  
- Includes **Postgres persistence**.

---


## 📂 Project Structure
```
fuel-downloader/
├── go/                         # Go implementation (learning + showcase)
│   ├── go.mod
│   ├── cmd/
│   │   ├── fuel-latest/        # Main app (fetch → upsert → export)
│   │   └── migrate/            # Migration helper (applies SQL schema)
│   └── internal/
│       ├── domain/             # Core entity
│       ├── infra/              # External systems (EIA, Postgres, CSV)
│       └── usecase/            # Orchestration logic
│
├── csharp/                     # .NET implementation (parallel)
│   └── src/...
│
├── angular/                    # Angular front-end (parallel)
│   └── src/...
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
└── README.md
```

---

## 🔧 Prerequisites
- [Go 1.22+](https://go.dev/dl/)  
- .NET 8 (for C# version).
- Angular CLI (for Angular front-end)  
- PostgreSQL (local or Docker)  
- EIA API Key (free from [EIA Open Data](https://www.eia.gov/opendata/))  

---

## ⚙️ Setup (Go)

### 1. Environment Variables
Set in **Windows** (global) or in **GoLand run/debug config** (local):

```
EIA_API_KEY=your-real-api-key
FUEL_DSN=postgres://postgres:postgres@localhost:[port]/fuel?sslmode=disable
FUEL_AREA=NUS
FUEL_OUT=[your directory]\fuel-latest.csv
```

---

### 2. Install Dependencies
From the `go` folder:

```powershell
cd [your directory]\fuel-downloader\go

go mod tidy
go get github.com/jackc/pgx/v5@latest

# Optional dev tools
go install golang.org/x/tools/cmd/goimports@latest
go install github.com/golangci/golangci-lint/cmd/golangci-lint@latest
```

---

### 3. Apply Database Migration
```powershell
cd [your directory]\fuel-downloader\go
go run .\cmd\migrate
```

or build/run it:

```powershell
go build -o .\bin\migrate.exe .\cmd\migrate
.\bin\migrate.exe
```

---

### 4. Run the App
From GoLand:  
- Select the `fuel-latest` run/debug config.  
- Run → CSV appears on your Desktop.  

From PowerShell:  
```powershell
cd [your directory]\fuel-downloader\go
go run .\cmd\fuel-latest
```

From double-click:  
- Use `scripts\fuel-latest.bat`.  
- If env vars are missing, it shows an error and pauses.  

---

## 📊 Example Output
Generated `fuel-latest.csv`:

```
product_code,product_name,area_code,area_name,period,value,unit,generated_utc
EPD2D,No 2 Diesel,NUS,U.S.,2025-08,3.744,$/GAL,2025-09-20T06:39:46Z
```

---

## 🎯 Why This Matters

This project is my **Go learning journey** that **automates my real-world need**.
- It shows my process for **adopting Go** while staying grounded in my .NET and Angular expertise.  
- It is a **real-world app** with **clean architecture** translated **across stacks**.

---
