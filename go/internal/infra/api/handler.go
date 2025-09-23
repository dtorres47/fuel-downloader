package api

import (
	"fmt"
	"net/http"
)

// LatestHandler is a placeholder HTTP handler.
func LatestHandler(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json")
	fmt.Fprint(w, `{"message":"latest fuel price placeholder"}`)
}
