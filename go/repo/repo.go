package repo

import (
	"context"
	"fmt"
	"fuel-downloader/domain"
	"log"

	"github.com/jackc/pgx/v5/pgxpool"
)

type Service struct {
	Pool *pgxpool.Pool
}

func New(connString string) (*Service, error) {
	pool, err := pgxpool.New(context.Background(), connString)
	if err != nil {
		return nil, fmt.Errorf("unable to create connection pool: %w", err)
	}

	// Test the connection
	err = pool.Ping(context.Background())
	if err != nil {
		return nil, fmt.Errorf("unable to ping database: %w", err)
	}

	log.Println("Database connected successfully")

	return &Service{Pool: pool}, nil
}

func (s *Service) Close() {
	s.Pool.Close()
}

/*
SaveFuelRates

Inserts or updates fuel rates in the database.
*/
func SaveFuelRates(db *Service, fuelRates []domain.FuelRate) error {
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
