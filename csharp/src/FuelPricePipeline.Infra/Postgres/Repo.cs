namespace FuelPricePipeline.Infra.Postgres;

using FuelPricePipeline.Domain;
using Npgsql;
using Polly;
using Dapper;

public class Repo : IFuelRepository
{
    private readonly string _connectionString;
    private readonly IAsyncPolicy _retryPolicy;

    public Repo(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Timeout = 30
        };
        _connectionString = builder.ConnectionString;

        // Retry only transient failures.
        _retryPolicy = Policy
            .Handle<NpgsqlException>(ex => ex.IsTransient)
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public async Task UpsertAsync(DieselFuelPrice fuelRate)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO fuel_price.diesel_fuel_price
                    (product_code, area_code, period, value, unit, product_name, area_name)
                VALUES
                    (@ProductCode, @AreaCode, @Period, @Value, @Unit, @ProductName, @AreaName)
                ON CONFLICT (product_code, area_code, period)
                DO UPDATE SET
                    value        = EXCLUDED.value,
                    unit         = EXCLUDED.unit,
                    product_name = EXCLUDED.product_name,
                    area_name    = EXCLUDED.area_name,
                    updated_at   = NOW();
";

            await conn.ExecuteAsync(sql, fuelRate);
        });
    }
}