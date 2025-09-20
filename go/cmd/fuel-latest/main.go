package main

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"os"
	"path/filepath"
	"time"

	"github.com/dtorres47/fuel-downloader/go/internal/infra/eia"
	"github.com/dtorres47/fuel-downloader/go/internal/infra/postgres"
	"github.com/dtorres47/fuel-downloader/go/internal/usecase/getlatest"
	"github.com/jackc/pgx/v5/pgxpool"
)

func main() {
	// Read from environment variables
	dsn := os.Getenv("FUEL_DSN")
	apiKey := os.Getenv("EIA_API_KEY")
	area := os.Getenv("FUEL_AREA")
	if area == "" {
		area = "NUS" // default
	}
	out := os.Getenv("FUEL_OUT")
	if out == "" {
		out = defaultDesktopPath("fuel-latest.csv")
	}

	// Validate required env vars
	if dsn == "" || apiKey == "" {
		log.Fatal("missing required environment variables: FUEL_DSN or EIA_API_KEY")
	}

	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	pool, err := pgxpool.New(ctx, dsn)
	if err != nil {
		log.Fatalf("pgxpool.New: %v", err)
	}
	defer pool.Close()
	if err := pool.Ping(ctx); err != nil {
		log.Fatalf("db ping: %v", err)
	}

	deps := getlatest.Deps{
		EIA: &eia.Client{
			APIKey: apiKey,
			HTTP:   &http.Client{Timeout: 20 * time.Second},
		},
		Repo: &postgres.Repo{Pool: pool},
	}

	fr, err := getlatest.Execute(ctx, deps, area, out)
	if err != nil {
		log.Fatalf("run failed: %v", err)
	}

	fmt.Printf("✅ OK: %s %s %.4f %s → %s\n",
		fr.ProductCode, fr.Period.Format("2006-01"), fr.Value, fr.Unit, out)
}

func defaultDesktopPath(name string) string {
	home, _ := os.UserHomeDir()
	return filepath.Join(home, "Desktop", name)
}
