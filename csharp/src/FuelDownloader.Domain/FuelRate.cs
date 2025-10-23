using System.Text.Json.Serialization;

namespace FuelDownloader.Domain
{
    public class FuelRate
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string AreaCode { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
        public DateTime Period { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    // TODO: Add DTOs
    //public class EiaResponse
    //{
    //    public ResponseData Response { get; set; }
    //}

    //public class ResponseData
    //{
    //    public List<FuelData> Data { get; set; }
    //}

    //public class FuelData
    //{
    //    [JsonPropertyName("product")]
    //    public string ProductCode { get; set; }

    //    [JsonPropertyName("product-name")]
    //    public string ProductName { get; set; }

    //    // ... etc
    //}
}
