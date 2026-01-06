package domain

import "time"

type FuelRate struct {
	ID          int       `json:"id"`
	Product     string    `json:"product"`
	DuoArea     string    `json:"duoarea"`
	Period      time.Time `json:"period"`
	Value       float64   `json:"value"`
	Units       string    `json:"units"`
	ProductName string    `json:"product_name"`
	AreaName    string    `json:"area_name"`
	CreatedAt   time.Time `json:"created_at"`
	UpdatedAt   time.Time `json:"updated_at"`
}
