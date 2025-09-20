package getlatest

import (
	"context"

	"github.com/dtorres47/fuel-downloader/go/internal/domain"
	"github.com/dtorres47/fuel-downloader/go/internal/infra/csvexp"
	"github.com/dtorres47/fuel-downloader/go/internal/infra/eia"
	"github.com/dtorres47/fuel-downloader/go/internal/infra/postgres"
)

type Deps struct {
	EIA  *eia.Client
	Repo *postgres.Repo
}

func Execute(ctx context.Context, d Deps, area, outPath string) (domain.FuelRate, error) {
	fr, err := d.EIA.FetchLatestDiesel(ctx, area)
	if err != nil {
		return domain.FuelRate{}, err
	}
	if err := d.Repo.Upsert(ctx, fr); err != nil {
		return domain.FuelRate{}, err
	}
	if err := csvexp.Write(outPath, fr); err != nil {
		return domain.FuelRate{}, err
	}
	return fr, nil
}
