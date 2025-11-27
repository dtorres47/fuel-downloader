# Fuel Downloader (Go)

Downloads fuel price data from the EIA API, stores it in PostgreSQL, and exports to CSV.

## Setup

1. Get an API key from [EIA Open Data](https://www.eia.gov/opendata/)
2. Set environment variable: `$env:EIA_API_KEY = "your-key"`
3. Create PostgreSQL database: `fuel_downloader`
4. Run schema: `fuel-downloader.sql`

## Run
```bash
go run .
```

## Output

- Database: `eia.fuel_rate` table
- CSV file: `fuel_rates.csv`