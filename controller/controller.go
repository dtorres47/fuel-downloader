package controller

import (
	"encoding/json"
	"go-api/service"
	"log"
	"net/http"
)

type FuelController struct {
	userService service.UserService
}

func NewFuelController(userService service.UserService) *FuelController {
	//return &FuelController{userService: userService}
	return &FuelController{}
}

func (fc *FuelController) RatesHandler(responseWriter http.ResponseWriter, request *http.Request) {
	if request.Method != http.MethodGet {
		responseWriter.WriteHeader(http.StatusMethodNotAllowed)
		return
	}
	apiKey := os.Getenv("S8oR4yf6NugwNKVp6KELAlboeRXt8I7zhyw6EbZl")
	if apiKey == "" {
		http.Error(responseWriter, "EIA_API_KEY not set", http.StatusInternalServerError)
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
		http.Error(w, "failed fetching fuel rates", http.StatusBadGateway)
		return
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		log.Printf("EIA bad status: %s", resp.Status)
		http.Error(w, "EIA API returned non‑200", http.StatusBadGateway)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	// simply proxy the JSON through
	if _, err := w.ReadFrom(resp.Body); err != nil {
		log.Printf("error streaming body: %v", err)
	}
}
