package main

import (
	"fmt"
	"net/http"

	"github.com/dtorres47/fuel-downloader/go/internal/infra/api"
	"github.com/go-chi/chi/v5"
)

func main() {
	r := chi.NewRouter()

	// Register placeholder handler
	r.Get("/fuel/latest", api.LatestHandler)

	fmt.Println("🚀 API running on :8080")
	http.ListenAndServe(":8080", r)
}
