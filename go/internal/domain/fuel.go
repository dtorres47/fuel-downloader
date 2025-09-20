package domain

import "time"

type FuelRate struct {
	ProductCode string
	ProductName string
	AreaCode    string
	AreaName    string
	Period      time.Time // first day of month (UTC)
	Value       float64
	Unit        string
}
