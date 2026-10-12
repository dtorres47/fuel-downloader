using FuelPricePipeline.Domain;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace FuelPricePipeline.Infra.Eia
{
    public class Client : IEiaClient
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Client> _logger;

        public Client(string apiKey, IHttpClientFactory httpClientFactory, ILogger<Client> logger)
        {
            _apiKey = apiKey;
            _httpClient = httpClientFactory.CreateClient("EiaClient");
            _logger = logger;
        }

        // EIA duoarea codes for the gasoline/diesel price series.
        private static readonly HashSet<string> ValidAreas = new HashSet<string>()
        {
            "NUS", // U.S. National
            "R10", // PADD 1 (East Coast)
            "R1X", // PADD 1A (New England)
            "R1Y", // PADD 1B (Central Atlantic)
            "R1Z", // PADD 1C (Lower Atlantic)
            "R20", // PADD 2 (Midwest)
            "R30", // PADD 3 (Gulf Coast)
            "R40", // PADD 4 (Rocky Mountain)
            "R50"  // PADD 5 (West Coast)
        };

        public async Task<DieselFuelPrice?> FetchLatestDieselAsync(string area = "NUS")
        {
            if (ValidAreas.Contains(area) == false)
            {
                _logger.LogError("Invalid area code: {Area}", area);
                throw new ArgumentException($"Invalid area code: {area}. Valid codes: {string.Join(", ", ValidAreas)}", nameof(area));
            }

            try
            {
                // Build the query without the API key so it can be logged safely;
                // the key is appended only on the actual request.
                var query = BuildQuery(area);
                _logger.LogDebug("Fetching fuel rate from EIA API: {Url}", BaseUrl + query);

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

        private static string BuildQuery(string area)
        {
            var builder = new StringBuilder();
            builder.Append("frequency=weekly");
            builder.Append("&data[0]=value");
            builder.Append("&facets[product][]=EPD2D");
            builder.Append($"&facets[duoarea][]={Uri.EscapeDataString(area)}");
            builder.Append("&sort[0][column]=period&sort[0][direction]=desc");
            builder.Append("&offset=0&length=1");
            return builder.ToString();
        }

        private DieselFuelPrice? ParseResponse(string json)
        {
            using var doc = JsonDocument.Parse(json, new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            });

            if (!doc.RootElement.TryGetProperty("response", out var response) ||
                !response.TryGetProperty("data", out var data) ||
                data.GetArrayLength() == 0)
            {
                _logger.LogWarning("EIA response contains no data");
                return null;
            }

            var firstItem = data[0];

            // Period and value are required. Don't save invalid data.
            var periodText = GetString(firstItem, "period");

            if (!DateTime.TryParseExact(periodText, new[] { "yyyy-MM-dd", "yyyy-MM" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var period))
            {
                _logger.LogWarning("EIA response has a missing or invalid period: '{Period}'", periodText);
                return null;
            }

            if (!TryGetDecimal(firstItem, "value", out var value))
            {
                _logger.LogWarning("EIA response has a missing or invalid value for period {Period}", periodText);
                return null;
            }

            return new DieselFuelPrice
            {
                ProductCode = GetString(firstItem, "product"),
                ProductName = GetString(firstItem, "product-name"),
                AreaCode = GetString(firstItem, "duoarea"),
                AreaName = GetString(firstItem, "area-name"),
                Period = period,
                Value = value,
                Unit = GetString(firstItem, "units")
            };
        }

        /// <summary>
        /// Safely gets JSON string element.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetString(JsonElement item, string name)
        {

            return item.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String
                ? prop.GetString() ?? ""
                : "";
        }

        /// <summary>
        /// Safely gets JSON decimel element.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool TryGetDecimal(JsonElement item, string name, out decimal value)
        {
            value = 0m;

            if (!item.TryGetProperty(name, out var prop))
            {
                return false;
            }

            if (prop.ValueKind == JsonValueKind.Number)
            {
                return prop.TryGetDecimal(out value);
            }

            if (prop.ValueKind == JsonValueKind.String)
            {
                return decimal.TryParse(prop.GetString(), NumberStyles.Number,
                    CultureInfo.InvariantCulture, out value);
            }

            return false;
        }
    }
}