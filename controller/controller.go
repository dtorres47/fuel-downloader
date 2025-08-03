package controller

import (
	"encoding/json"
	"go-api/service"
	"log"
	"net/http"
)

type FuelController struct {
	fuelService service.FuelService
}

func NewFuelController(fuelService service.FuelService) *FuelController {
	return &FuelController{fuelService: fuelService}
}

func (fc *FuelController) RatesHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}

	rates, err := fc.fuelService.GetRates()
	if err != nil {
		log.Printf("error fetching rates: %v", err)
		http.Error(w, err.Error(), http.StatusBadGateway)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	if err := json.NewEncoder(w).Encode(rates); err != nil {
		log.Printf("error encoding rates: %v", err)
	}
}
