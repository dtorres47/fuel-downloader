package router

import (
	"go-api/controller"
	"net/http"
)

func RegisterRoutes(fuelController *controller.FuelController) {
	http.HandleFunc("/fuel/rates", fuelController.RatesHandler)
}
