using FuelDownloader.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuelDownloader.Infra.Eia
{
    public class Client
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public Client(string apiKey, HttpClient? httpClient = null)
        {
            _apiKey = apiKey;
            _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        }

        public async Task<FuelRate?> FetchLatestDieselAsync(string area = "NUS")
        {
            var url = $"https://api.eia.gov/v2/petroleum/pri/gnd/data/?" +
                      $"frequency=monthly" +
                      $"&data[0]=value" +
                      $"&facets[product][]=EPD2D" +
                      $"&facets[duoarea][]={area}" +
                      $"&sort[0][column]=period&sort[0][direction]=desc" +
                      $"&offset=0&length=1" +
                      $"&api_key={_apiKey}";

            var resp = await _httpClient.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var data = doc.RootElement.GetProperty("response").GetProperty("data")[0];

            return new FuelRate
            {
                ProductCode = data.GetProperty("product").GetString() ?? "",
                ProductName = data.GetProperty("product-name").GetString() ?? "",
                AreaCode = data.GetProperty("duoarea").GetString() ?? "",
                AreaName = data.GetProperty("area-name").GetString() ?? "",
                Period = DateTime.Parse(data.GetProperty("period").GetString() ?? DateTime.UtcNow.ToString("yyyy-MM")),
                Value = decimal.Parse(data.GetProperty("value").GetString() ?? "0"),
                Unit = data.GetProperty("units").GetString() ?? ""
            };
        }
    }
}
