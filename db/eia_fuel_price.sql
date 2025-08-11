-- Minimal schema for EIA gasoline (or other fuel) prices
CREATE SCHEMA IF NOT EXISTS eia;

CREATE TABLE IF NOT EXISTS eia.fuel_price (
                                              product_code  TEXT        NOT NULL,               -- e.g. EPD2D
                                              area_code     TEXT        NOT NULL,               -- e.g. US or state/region code
                                              period        DATE        NOT NULL,               -- first day of month (YYYY-MM-01)
                                              value         NUMERIC(12,4) NOT NULL,            -- price
    unit          MONEY,                               -- e.g. USD/gal
    product_name  TEXT,
    area_name     TEXT,
    raw           JSONB,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    PRIMARY KEY (product_code, area_code, period)
    );

-- Helpful indexes for common queries
CREATE INDEX IF NOT EXISTS idx_fuel_price_period  ON eia.fuel_price (period);
CREATE INDEX IF NOT EXISTS idx_fuel_price_product ON eia.fuel_price (product_code);
CREATE INDEX IF NOT EXISTS idx_fuel_price_area    ON eia.fuel_price (area_code);
