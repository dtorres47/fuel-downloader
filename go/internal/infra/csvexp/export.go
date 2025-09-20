package csvexp

import (
	"encoding/csv"
	"fmt"
	"os"
	"path/filepath"
	"time"

	"github.com/dtorres47/fuel-downloader/go/internal/domain"
)

func Write(path string, fr domain.FuelRate) error {
	// Build timestamped filename
	dir := filepath.Dir(path)
	base := filepath.Base(path)
	ext := filepath.Ext(base)
	name := base[:len(base)-len(ext)]

	timestamp := time.Now().UTC().Format("20060102_150405")
	newFile := filepath.Join(dir, fmt.Sprintf("%s_%s%s", name, timestamp, ext))

	// Create the new file
	f, err := os.Create(newFile)
	if err != nil {
		return err
	}
	defer f.Close()

	w := csv.NewWriter(f)
	defer w.Flush()

	// Write headers
	_ = w.Write([]string{"product_code", "product_name", "area_code", "area_name", "period", "value", "unit", "generated_utc"})

	// Write one row
	return w.Write([]string{
		fr.ProductCode,
		fr.ProductName,
		fr.AreaCode,
		fr.AreaName,
		fr.Period.Format("2006-01"),
		fmt.Sprintf("%.4f", fr.Value),
		fr.Unit,
		time.Now().UTC().Format(time.RFC3339),
	})
}
