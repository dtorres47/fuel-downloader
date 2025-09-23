# fuel-downloader

This project started from a **real-world need in my own work**.  
I create freight invoices for my corporation, and part of that process requires referencing the [EIA website](https://www.eia.gov/petroleum/gasdiesel/) to look up diesel fuel prices.  

Until now, I’ve been doing this **manually** — navigating to the site, finding the latest value, and copying it into my workflow. That’s repetitive and error-prone.  

The **fuel-downloader** project automates that task:  
- Fetches the latest U.S. diesel price from the EIA API.  
- Persists the data in Postgres for historical tracking.  
- Exports a CSV for invoicing.  
- Exposes a **Go microservice API** (via [Chi](https://github.com/go-chi/chi)) to serve fuel prices to other apps.  

---

## 🌱 Methodology

- My **primary focus** is leveling up in **Go**. I’m deliberately using an **enterprise scaffold** (vertical slice: domain, infra, usecase, cmd, api) to simulate real-world design.  
- I built this project around a **real business problem** to ensure it’s practical, not just academic.  
- I implement first in **C#/.NET**, then translate into **Go**, to accelerate learning while proving cross-language adaptability.  
- I also provide an **Angular front-end** as a tangible UI layer.  
- All three implementations — Go, C#, and Angular — follow the **same vertical slice structure**, so the design is consistent and comparable across stacks.  

This methodology demonstrates:  
- **Go learning journey** → API, persistence, and automation.  
- **Cross-language strength** → C#, Angular, Go.  
- **Full-stack capability** → DB + API + UI.  

---

## 🔧 Prerequisites
- [Go 1.22+](https://go.dev/dl/)  
- .NET 8 (for C# version)  
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

_Default port: 5432 (use your own if different)_  

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
