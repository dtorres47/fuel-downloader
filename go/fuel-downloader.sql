CREATE TABLE IF NOT EXISTS eia.fuel_rate (
    id BIGSERIAL PRIMARY KEY,
    product TEXT NOT NULL,
    duoarea TEXT NOT NULL,
    period DATE NOT NULL,
    value NUMERIC(10, 4) NOT NULL,
    units TEXT NOT NULL,
    product_name TEXT,
    area_name TEXT,
    raw JSONB DEFAULT '{}'::jsonb,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT uq_fuel_rate_product_area_period 
        UNIQUE (product, duoarea, period)
);