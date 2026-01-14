package domain

// EIAResponse EIA response struct (simplified for diesel prices)
type EIAResponse struct {
	Response struct {
		Data []struct {
			Product     string `json:"product"`
			ProductName string `json:"product-name"`
			DuoArea     string `json:"duoarea"`
			AreaName    string `json:"area-name"`
			Period      string `json:"period"`
			Value       string `json:"value"`
			Units       string `json:"units"`
		} `json:"data"`
	} `json:"response"`
}
