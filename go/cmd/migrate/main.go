package main

import (
	"context"
	"fmt"
	"log"
	"os"
	"path/filepath"
	"time"

	"github.com/jackc/pgx/v5"
)

func main() {
	// DSN must come from Windows or GoLand env vars
	dsn := os.Getenv("FUEL_DSN")
	if dsn == "" {
		log.Fatal("❌ FUEL_DSN not set. Please set it in Windows or GoLand env vars.")
	}

	// Connect to Postgres
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	conn, err := pgx.Connect(ctx, dsn)
	if err != nil {
		log.Fatalf("❌ connect failed: %v", err)
	}
	defer conn.Close(ctx)

	// Dynamically locate project root and SQL file
	exePath, _ := os.Getwd()          // current working dir (e.g., ...\fuel-downloader\go)
	rootPath := filepath.Dir(exePath) // go up one level (repo root)
	sqlPath := filepath.Join(rootPath, "db", "eia_fuel_price.sql")

	sqlBytes, err := os.ReadFile(sqlPath)
	if err != nil {
		log.Fatalf("❌ read SQL file failed: %v", err)
	}

	// Execute migration
	_, err = conn.Exec(ctx, string(sqlBytes))
	if err != nil {
		log.Fatalf("❌ migration failed: %v", err)
	}

	fmt.Println("✅ Migration applied successfully from", sqlPath)
}
