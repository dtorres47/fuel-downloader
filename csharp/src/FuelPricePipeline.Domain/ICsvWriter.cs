namespace FuelPricePipeline.Domain;

public interface ICsvWriter
{
    Task WriteAsync(string filePath, DieselFuelPrice fuelRate);
}