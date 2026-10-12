# fuel-price-pipeline

**A PostgreSQL pipeline that tracks EIA diesel prices to solve a real invoicing workflow. Chi-based REST API with CSV export. Implements the same job in two data-integration styles: ETL in C#, ELT in Go.**

I create freight invoices for my company. This project automates downloading diesel prices from [EIA](https://www.eia.gov/petroleum): fetch from the EIA v2 API, store in PostgreSQL, serve the data over REST, and export a CSV for invoicing.

Built primarily in **Go** (the focus), with a **C# implementation** reflecting my production .NET experience, and a lean **Angular** UI.

---

## Two styles, one pipeline

The two backends deliberately implement different data-integration patterns:

- **C# — ETL.** Extract from EIA, **transform in code** (validate, parse, cherry-pick fields), load only clean columns. This mirrors the production ETL systems I built professionally in C# and T-SQL.
- **Go — ELT.** Extract from EIA, **load first, transform in SQL**: raw responses land in PostgreSQL and views shape them into query-ready data (latest price per product/area, with monthly and annual aggregates derived from the weekly series).

```
C# (ETL):  EIA API ──Extract──▶ Transform in C# ──▶ PostgreSQL (clean columns) ──▶ CSV
Go (ELT):  EIA API ──Extract──▶ PostgreSQL ──Transform──▶ SQL views ──▶ REST API + CSV
```

Both fetch the same series: **weekly** No. 2 diesel retail prices (EIA product `EPD2D`). Monthly and annual numbers are derived in SQL, never fetched separately — one stored frequency, no mixed measurements.

The schema `fuel_price` is the general container; `diesel_fuel_price` is the first table under it, leaving room for other fuels as sibling tables. The canonical table definition lives in **`db/schema/`** — the single source of truth for the database objects.

---

## Go implementation (focus)

Clean architecture with clear layer boundaries:

```
go/
├── domain/     # DieselFuelPrice model, repository interface
├── service/    # EIA fetch + business logic
├── adapters/   # PostgreSQL (pgx) + CSV export
├── ports/      # Chi HTTP handlers
└── main.go     # composition root
```

**Stack:** Go · [Chi](https://github.com/go-chi/chi) router · [pgx](https://github.com/jackc/pgx) · PostgreSQL

**Endpoints:**

| Method | Route          | Purpose                               |
|--------|----------------|---------------------------------------|
| GET    | `/healthz`     | Health check (server up, DB reachable) |
| GET    | `/getEIAData`  | Fetch fresh prices from EIA           |
| GET    | `/getAll`      | Return stored prices from the DB      |
| POST   | `/save`        | Persist prices to the DB              |

---

## Run it

```bash
# 1. Start PostgreSQL + Adminer
docker compose up -d

# 2. One time only: create the schema and table
docker compose exec -T db psql -U postgres -d fuel_price < db/schema/diesel_fuel_price.sql

# 3. Set environment
export EIA_API_KEY="your-key"        # free from https://www.eia.gov/opendata/
export FUEL_DSN="postgres://postgres:postgres@localhost:5432/fuel_price"

# 4. Run the Go pipeline + API
cd go
go run .                 # full run: fetch from EIA, save, export CSV, serve API
go run . -serve-only     # skip the EIA fetch; just serve the API (local testing)
```

On Windows/PowerShell, set variables with `$env:EIA_API_KEY = "..."` and pipe the schema with `Get-Content db\schema\diesel_fuel_price.sql | docker compose exec -T db psql -U postgres -d fuel_price`.

Adminer (database web UI) runs at `http://localhost:3333` — server `db`, user/password `postgres`, database `fuel_price`.

Output CSV:

```
product_code,product_name,area_code,area_name,period,value,unit,generated_utc
EPD2D,No 2 Diesel,NUS,U.S.,2026-09-28,3.744,$/GAL,2026-10-11T06:39:46Z
```

---

## C# implementation · Angular UI

- **C#/.NET** — the ETL side, and the stack I have production experience in. Uses DI, `IHttpClientFactory`, Polly retries (transient errors only), Dapper, culture-invariant parsing, and the Result pattern. Interfaces live in the Domain project; the use case layer has no infrastructure dependency. Run `FuelPricePipeline.Cmd.Migrate` once to create the table, then `FuelPricePipeline.Cmd.FuelLatest` (reads `EIA_API_KEY`, `FUEL_DSN`, optional `FUEL_AREA`, default `NUS` — PADD regions use EIA codes such as `R10`, `R30`). See `csharp/`.
- **Angular** — a deliberately thin UI layer to demonstrate full-stack reach, not a frontend showcase. See `angular/`.

---

## Tech stack

**Go** · **C#/.NET** · **Angular** · **PostgreSQL** · **Chi** · **pgx** · **Docker**
