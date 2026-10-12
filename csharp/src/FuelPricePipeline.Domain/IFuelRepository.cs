namespace FuelPricePipeline.Domain;

public interface IFuelRepository
{
    Task UpsertAsync(DieselFuelPrice fuelRate);
}