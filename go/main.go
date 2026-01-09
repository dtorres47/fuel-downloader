package main

import (
	"fmt"
	"fuel-downloader/adapters"
	"fuel-downloader/service"
	"log"
	"os"
	"strings"
)

func main() {
	// Configuration
	apiKey := strings.TrimSpace(os.Getenv("EIA_API_KEY"))
	if apiKey == "" {
		log.Fatal("EIA_API_KEY environment variable not set. Exiting now...")
	}

	connString := "postgresql://postgres:postgres@localhost:5432/fuel_downloader"
	csvFilename := "fuel_rates.csv"

	// Connect to database
	db, err := adapters.New(connString)
	if err != nil {
		log.Fatal("Failed to connect to database:", err)
	}
	defer db.Close()

	// Get latest fuel rates from EIA
	fmt.Println("Downloading fuel rates from EIA API...")
	fuelRates, err := service.GetLatestFuelRates(apiKey)
	if err != nil {
		log.Fatal("Failed to get fuel rates:", err)
	}
	fmt.Printf("Downloaded %d fuel rates\n", len(fuelRates))

	// Save to database
	fmt.Println("Saving to database...")
	err = adapters.SaveFuelRates(db, fuelRates)
	if err != nil {
		log.Fatal("Failed to save fuel rates:", err)
	}
	fmt.Println("Saved to database successfully")

	// Export to CSV
	fmt.Println("Exporting to CSV...")
	err = adapters.ExportToCSV(db, csvFilename)
	if err != nil {
		log.Fatal("Failed to export CSV:", err)
	}
	fmt.Printf("Exported to %s successfully\n", csvFilename)

	fmt.Println("Complete!")
}
