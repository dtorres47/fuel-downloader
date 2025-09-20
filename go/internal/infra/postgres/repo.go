package postgres

import (
	"context"

	"github.com/dtorres47/fuel-downloader/go/internal/domain"
	"github.com/jackc/pgx/v5/pgxpool"
)

type Repo struct{ Pool *pgxpool.Pool }

func (r *Repo) Upsert(ctx context.Context, fr domain.FuelRate) error {
	_, err := r.Pool.Exec(ctx, `
		INSERT INTO eia.fuel_price
			(product_code, area_code, period, value, unit, product_name, area_name, raw)
		VALUES
			($1,$2,$3::date,$4,$5,$6,$7,$8)
		ON CONFLICT (product_code, area_code, period)
		DO UPDATE SET
			value = EXCLUDED.value,
			unit = EXCLUDED.unit,
			updated_at = NOW(),
			raw = EXCLUDED.raw
	`, fr.ProductCode, fr.AreaCode, fr.Period, fr.Value, fr.Unit, fr.ProductName, fr.AreaName, nil)
	return err
}
