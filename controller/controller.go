package controller

import (
	"fmt"
	"io"
	"log"
	"net/http"
	"os"
)

type FuelController struct {
}

func NewFuelController() *FuelController {
	return &FuelController{}
}

func (fuelController *FuelController) RatesHandler(responseWriter http.ResponseWriter, request *http.Request) {
	if request.Method != http.MethodGet {
		http.Error(responseWriter, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}
	apiKey := os.Getenv("API_KEY")
	if apiKey == "" {
		http.Error(responseWriter, "API_KEY not set", http.StatusInternalServerError)
	}

	url := fmt.Sprintf(
		"https://api.eia.gov/v2/petroleum/pri/gnd/data/"+
			"?frequency=monthly"+
			"&data[0]=value"+
			"&facets[product][]=EPD2D"+
			"&start=2025-01"+
			"&sort[0][column]=period"+
			"&sort[0][direction]=desc"+
			"&offset=0"+
			"&length=5000"+
			"&api_key=%s",
		apiKey,
	)

	resp, err := http.Get(url)
	if err != nil {
		log.Printf("EIA request failed: %v", err)
		http.Error(responseWriter, "Failed getting fuel rates", http.StatusBadGateway)
		return
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		log.Printf("EIA API returned non-200 status: %s", resp.Status)
		http.Error(responseWriter, "EIA API returned non-200", http.StatusBadGateway)
		return
	}

	responseWriter.Header().Set("Content-Type", "application/json")
	if _, err := io.Copy(responseWriter, resp.Body); err != nil {
		log.Printf("Error streaming response: %v", err)
	}
}
