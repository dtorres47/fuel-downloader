package domain

import (
	"context"
)

type Repository interface {
	GetAll(ctx context.Context) ([]FuelRate, error)
	Save(ctx context.Context, fuelRates []FuelRate) error
}
