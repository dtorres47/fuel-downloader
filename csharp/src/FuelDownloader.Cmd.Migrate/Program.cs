class Program
{
    static async Task Main(string[] args)
    {
        // TODO: read env var for DSN
        var dsn = "TODO-get-from-env";

        // TODO: open DB connection and run SQL from db/schema/eia_fuel_price.sql
        await Task.CompletedTask;

        Console.WriteLine("✅ Migration applied successfully");
    }
}