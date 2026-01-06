package service

import (
	"encoding/json"
	"fmt"
	"fuel-downloader/domain"
	"io"
	"net/http"
	"strconv"
	"time"
)

type FuelRate = domain.FuelRate
type EIAResponse = domain.EIAResponse

/*
GetLatestFuelRates

Gets diesel fuel prices from EIA API.
*/
func GetLatestFuelRates(apiKey string) ([]FuelRate, error) {
	// EIA API endpoint for diesel prices
	url := fmt.Sprintf("https://api.eia.gov/v2/petroleum/pri/gnd/data/?api_key=%s&frequency=weekly&data[0]=value&sort[0][column]=period&sort[0][direction]=desc&offset=0&length=10", apiKey)

	// Make HTTP request
	resp, err := http.Get(url)
	if err != nil {
		return nil, fmt.Errorf("failed to fetch from EIA: %w", err)
	}
	defer resp.Body.Close()

	// Check status code
	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("EIA API returned status %d", resp.StatusCode)
	}

	// Read response body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("failed to read response: %w", err)
	}

	// Parse JSON
	var eiaResp EIAResponse
	err = json.Unmarshal(body, &eiaResp)
	if err != nil {
		return nil, fmt.Errorf("failed to parse JSON: %w", err)
	}

	// Convert to FuelRate structs
	var fuelRates []FuelRate
	for _, item := range eiaResp.Response.Data {
		// Parse period string (format: "YYYY-MM-DD")
		period, err := time.Parse("2006-01-02", item.Period)
		if err != nil {
			continue // Skip invalid dates
		}

		// Parse value string to float64
		value, err := strconv.ParseFloat(item.Value, 64)
		if err != nil {
			continue // Skip invalid values
		}

		fuelRate := FuelRate{
			Product:     item.Product,
			ProductName: item.ProductName,
			DuoArea:     item.DuoArea,
			AreaName:    item.AreaName,
			Period:      period,
			Value:       value,
			Units:       item.Units,
		}
		fuelRates = append(fuelRates, fuelRate)
	}

	return fuelRates, nil
}
