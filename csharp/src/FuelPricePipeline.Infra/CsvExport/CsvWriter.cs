namespace FuelPricePipeline.Infra.CsvExport;

using FuelPricePipeline.Domain;
using System.Globalization;

public class CsvWriter : ICsvWriter
{
    public async Task WriteAsync(string filePath, DieselFuelPrice fuelRate)
    {
        using var writer = new StreamWriter(filePath, false); // overwrite
        await writer.WriteLineAsync("product_code,product_name,area_code,area_name,period,value,unit,generated_utc");

        await writer.WriteLineAsync(string.Join(",",
            Escape(fuelRate.ProductCode),
            Escape(fuelRate.ProductName),
            Escape(fuelRate.AreaCode),
            Escape(fuelRate.AreaName),
            fuelRate.Period.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            fuelRate.Value.ToString("F4", CultureInfo.InvariantCulture),
            Escape(fuelRate.Unit),
            DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)));
    }

    /// <summary>
    /// Adds quotes to a field when it contains a comma, quote, or line break.
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    private static string Escape(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }
        return field;
    }
}
