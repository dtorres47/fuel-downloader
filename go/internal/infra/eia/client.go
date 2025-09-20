package eia

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"
	"time"

	"github.com/dtorres47/fuel-downloader/go/internal/domain"
)

type Client struct {
	HTTP   *http.Client
	APIKey string
}

type eiaResp struct {
	Response struct {
		Data []struct {
			AreaName    string `json:"area-name"`
			DuoArea     string `json:"duoarea"`
			Period      string `json:"period"`
			Product     string `json:"product"`
			ProductName string `json:"product-name"`
			Units       string `json:"units"`
			Value       string `json:"value"`
		} `json:"data"`
	} `json:"response"`
}

func (c *Client) FetchLatestDiesel(ctx context.Context, area string) (domain.FuelRate, error) {
	if area == "" {
		area = "NUS"
	}
	url := "https://api.eia.gov/v2/petroleum/pri/gnd/data/?" +
		"frequency=monthly" +
		"&data[0]=value" +
		"&facets[product][]=EPD2D" +
		"&facets[duoarea][]=" + area +
		"&sort[0][column]=period&sort[0][direction]=desc" +
		"&offset=0&length=1" +
		"&api_key=" + c.APIKey

	req, _ := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
	if c.HTTP == nil {
		c.HTTP = &http.Client{Timeout: 20 * time.Second}
	}
	resp, err := c.HTTP.Do(req)
	if err != nil {
		return domain.FuelRate{}, err
	}
	defer resp.Body.Close()
	if resp.StatusCode != http.StatusOK {
		return domain.FuelRate{}, fmt.Errorf("EIA non-200: %s", resp.Status)
	}

	var er eiaResp
	if err := json.NewDecoder(resp.Body).Decode(&er); err != nil {
		return domain.FuelRate{}, err
	}
	if len(er.Response.Data) == 0 {
		return domain.FuelRate{}, fmt.Errorf("no data")
	}
	row := er.Response.Data[0]

	// Period -> first of month (UTC)
	t, err := time.Parse("2006-01", row.Period)
	if err != nil {
		if t2, err2 := time.Parse("2006-01-02", row.Period); err2 == nil {
			t = time.Date(t2.Year(), t2.Month(), 1, 0, 0, 0, 0, time.UTC)
		} else {
			return domain.FuelRate{}, fmt.Errorf("bad period %q: %v", row.Period, err)
		}
	} else {
		t = time.Date(t.Year(), t.Month(), 1, 0, 0, 0, 0, time.UTC)
	}

	val, err := strconv.ParseFloat(row.Value, 64)
	if err != nil {
		return domain.FuelRate{}, fmt.Errorf("bad value %q: %v", row.Value, err)
	}

	return domain.FuelRate{
		ProductCode: row.Product,
		ProductName: row.ProductName,
		AreaCode:    row.DuoArea,
		AreaName:    row.AreaName,
		Period:      t,
		Value:       val,
		Unit:        row.Units,
	}, nil
}
