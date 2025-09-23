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
}
