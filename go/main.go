package main

import (
	"context"
	"fmt"
	"fuel-downloader/adapters"
	"fuel-downloader/service"
	"log"
	"os"
	"strings"
)

func main() {

	// Configuration
	// TODO: add this to a config
	apiKey := strings.TrimSpace(os.Getenv("EIA_API_KEY"))
	if apiKey == "" {
		log.Fatal("EIA_API_KEY environment variable not set. Exiting now...")
	}
	connString := "postgresql://postgres:postgres@localhost:5432/fuel_downloader"
	csvFilename := "fuel_rates.csv"

	postgresRepo, err := adapters.NewPostgresRepository(connString)
	if err != nil {
		log.Fatal("Failed to connect to database:", err)
	}

	fuelService := service.NewFuelService(postgresRepo)
	ctx := context.Background()

	// TODO: refactor http methods to use chi router
	//router := chi.NewRouter()
	//router.Use(middleware.Logger)
	//router.Use(httprate.LimitByIP(10, time.Minute))
	// router.Get("/", func(w http.ResponseWriter, r *http.Request) {})

	// Get latest fuel rates from EIA
	fmt.Println("Downloading fuel rates from EIA API...")
	fuelRates, err := fuelService.GetFromEIA(apiKey)
	if err != nil {
		log.Fatal("Failed to get fuel rates:", err)
	}

	fmt.Printf("Downloaded %d fuel rates\n", len(fuelRates))

	// Save to database
	fmt.Println("Saving to database...")
	err = postgresRepo.Save(ctx, fuelRates)
	if err != nil {
		log.Fatal("Failed to save fuel rates:", err)
	}

	fmt.Println("Saved to database successfully")

	// Export to CSV
	fmt.Println("Exporting to CSV...")
	err = adapters.ExportToCSV(postgresRepo, csvFilename)
	if err != nil {
		log.Fatal("Failed to export CSV:", err)
	}
	fmt.Printf("Exported to %s successfully\n", csvFilename)

	fmt.Println("Complete!")
}
