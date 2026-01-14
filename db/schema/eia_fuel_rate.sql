CREATE SCHEMA IF NOT EXISTS eia;

CREATE TABLE IF NOT EXISTS eia.fuel_rate (
    product_code  TEXT           NOT NULL,
    area_code     TEXT           NOT NULL,
    period        DATE           NOT NULL,
    value         NUMERIC(12,4)  NOT NULL,
    unit          TEXT,
    product_name  TEXT,
    area_name     TEXT,
    raw           JSONB,
    created_at    TIMESTAMPTZ    NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ    NOT NULL DEFAULT NOW(),
    PRIMARY KEY (product_code, area_code, period)
);

CREATE INDEX IF NOT EXISTS idx_fuel_rate_period  ON eia.fuel_rate (period);
CREATE INDEX IF NOT EXISTS idx_fuel_rate_product ON eia.fuel_rate (product_code);
CREATE INDEX IF NOT EXISTS idx_fuel_rate_area    ON eia.fuel_rate (area_code);

CREATE OR REPLACE FUNCTION eia.set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_fuel_rate_updated_at ON eia.fuel_rate;

CREATE TRIGGER trg_fuel_rate_updated_at
BEFORE UPDATE ON eia.fuel_rate
FOR EACH ROW
EXECUTE FUNCTION eia.set_updated_at();
