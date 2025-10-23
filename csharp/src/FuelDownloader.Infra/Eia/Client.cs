using FuelDownloader.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuelDownloader.Infra.Eia
{
    public class Client
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Client> _logger;

        public Client(string apiKey, IHttpClientFactory httpClientFactory)
        {
            _apiKey = apiKey;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<FuelRate?> FetchLatestDieselAsync(string area = "NUS")
        {
            try
            {
                var url = BuildUrl(area);
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("EIA API returned {StatusCode} for area {Area}",
                        response.StatusCode, area);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                return ParseResponse(json);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error getting fuel rate for area {Area}", area);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse EIA response for area {Area}", area);
                return null;
            }
        }

        private string BuildUrl(string area)
        {
            var builder = new StringBuilder("https://api.eia.gov/v2/petroleum/pri/gnd/data/?");
            builder.Append("frequency=monthly");
            builder.Append("&data[0]=value");
            builder.Append("&facets[product][]=EPD2D");
            builder.Append($"&facets[duoarea][]={Uri.EscapeDataString(area)}");
            builder.Append("&sort[0][column]=period&sort[0][direction]=desc");
            builder.Append("&offset=0&length=1");
            builder.Append($"&api_key={_apiKey}");
            return builder.ToString();
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private FuelRate? ParseResponse(string json)
        {
            var doc = JsonDocument.Parse(json, new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            });

            if (!doc.RootElement.TryGetProperty("response", out var response) ||
                !response.TryGetProperty("data", out var data) ||
                data.GetArrayLength() == 0)
            {
                return null;
            }

            JsonElement firstItem = data[0];
            return new FuelRate
            {
                ProductCode = firstItem.GetProperty("product").GetString() ?? "",
            };
        }
    }
}
