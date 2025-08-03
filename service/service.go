package service

import (
	"encoding/json"
	"fmt"
	"net/http"
	"os"
)

type FuelRate struct {
	Period string  `json:"period"`
	Value  float64 `json:"value"`
}

type FuelService interface {
	GetRates() ([]FuelRate, error)
}

type fuelServiceImpl struct {
	apiKey string
}

func NewFuelService() FuelService {
	return &fuelServiceImpl{
		apiKey: os.Getenv("API_KEY"),
	}
}

func (s *fuelServiceImpl) GetRates() ([]FuelRate, error) {
	if s.apiKey == "" {
		return nil, fmt.Errorf("API_KEY not set")
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
		s.apiKey,
	)

	resp, err := http.Get(url)
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("EIA API returned %s", resp.Status)
	}

	// EIA responds with a big JSON envelope, so unwrap it:
	var envelope struct {
		Response struct {
			Data []FuelRate `json:"data"`
		} `json:"response"`
	}
	if err := json.NewDecoder(resp.Body).Decode(&envelope); err != nil {
		return nil, err
	}
	return envelope.Response.Data, nil
}
