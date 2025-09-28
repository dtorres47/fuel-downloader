namespace FuelDownloader.Infra.Postgres;

using Npgsql;
using FuelDownloader.Domain;

public class Repo
{
    private readonly string _connectionString;

    public Repo(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task UpsertAsync(FuelRate fuelRate)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(@"
            INSERT INTO eia.fuel_price
                (product_code, area_code, period, value, unit, product_name, area_name, raw)
            VALUES
                (@product_code, @area_code, @period, @value, @unit, @product_name, @area_name, '{}'::jsonb)
            ON CONFLICT (product_code, area_code, period)
            DO UPDATE SET
                value = EXCLUDED.value,
                unit = EXCLUDED.unit,
                updated_at = NOW(),
                raw = EXCLUDED.raw;
        ", conn);

        cmd.Parameters.AddWithValue("product_code", fuelRate.ProductCode);
        cmd.Parameters.AddWithValue("area_code", fuelRate.AreaCode);
        cmd.Parameters.AddWithValue("period", fuelRate.Period);
        cmd.Parameters.AddWithValue("value", fuelRate.Value);
        cmd.Parameters.AddWithValue("unit", fuelRate.Unit);
        cmd.Parameters.AddWithValue("product_name", fuelRate.ProductName);
        cmd.Parameters.AddWithValue("area_name", fuelRate.AreaName);

        await cmd.ExecuteNonQueryAsync();
    }
}
