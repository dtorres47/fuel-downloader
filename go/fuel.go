package main

import (
	"context"
	"fmt"
)

/*
SaveFuelRates

Inserts or updates fuel rates in the database.
*/
func SaveFuelRates(db *Service, fuelRates []FuelRate) error {
	sqlQuery := `
		INSERT INTO eia.fuel_rate 
		(product, duoarea, period, value, units, product_name, area_name, raw, created_at, updated_at)
		VALUES ($1, $2, $3, $4, $5, $6, $7, '{}'::jsonb, NOW(), NOW())
		ON CONFLICT (product, duoarea, period)
		DO UPDATE SET
			value = EXCLUDED.value,
			units = EXCLUDED.units,
			product_name = EXCLUDED.product_name,
			area_name = EXCLUDED.area_name,
			updated_at = NOW()
	`

	for _, fr := range fuelRates {
		_, err := db.Pool.Exec(context.Background(), sqlQuery,
			fr.Product, fr.DuoArea, fr.Period, fr.Value, fr.Units,
			fr.ProductName, fr.AreaName)
		if err != nil {
			return fmt.Errorf("failed to insert fuel rate: %w", err)
		}
	}

	return nil
}
