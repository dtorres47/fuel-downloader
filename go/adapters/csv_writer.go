package adapters

import (
	"context"
	"encoding/csv"
	"fmt"
	"fuel-downloader/domain"
	"os"
)

// ExportToCSV queries all fuel rates and writes them to a CSV file
func ExportToCSV(db *Service, filename string) error {
	// Query all fuel rates from database
	sqlQuery := `
		SELECT product, product_name, duoarea, area_name, 
		       period, value, units, created_at 
		FROM eia.fuel_rate 
		ORDER BY period DESC
	`

	rows, err := db.Pool.Query(context.Background(), sqlQuery)
	if err != nil {
		return fmt.Errorf("failed to query fuel rates: %w", err)
	}
	defer rows.Close()

	// Create CSV file
	file, err := os.Create(filename)
	if err != nil {
		return fmt.Errorf("failed to create CSV file: %w", err)
	}
	defer file.Close()

	// Create CSV writer
	writer := csv.NewWriter(file)
	defer writer.Flush()

	// Write header row
	header := []string{"Product", "Product Name", "Duo Area", "Area Name", "Period", "Value", "Units", "Created At"}
	if err := writer.Write(header); err != nil {
		return fmt.Errorf("failed to write CSV header: %w", err)
	}

	// Write data rows
	for rows.Next() {
		var fr domain.FuelRate
		err := rows.Scan(&fr.Product, &fr.ProductName, &fr.DuoArea, &fr.AreaName,
			&fr.Period, &fr.Value, &fr.Units, &fr.CreatedAt)
		if err != nil {
			return fmt.Errorf("failed to scan row: %w", err)
		}

		// Convert to string slice for CSV
		record := []string{
			fr.Product,
			fr.ProductName,
			fr.DuoArea,
			fr.AreaName,
			fr.Period.Format("2006-01-02"),
			fmt.Sprintf("%.4f", fr.Value),
			fr.Units,
			fr.CreatedAt.Format("2006-01-02 15:04:05"),
		}

		if err := writer.Write(record); err != nil {
			return fmt.Errorf("failed to write CSV row: %w", err)
		}
	}

	// Check for errors during iteration
	if err = rows.Err(); err != nil {
		return fmt.Errorf("error iterating rows: %w", err)
	}

	return nil
}
