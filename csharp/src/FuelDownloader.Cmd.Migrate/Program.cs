namespace FuelDownloader.Cmd.Migrate;

using FuelDownloader.Infra.Postgres;

class Program
{
    static async Task Main(string[] args)
    {
        var dsn = Environment.GetEnvironmentVariable("FUEL_DSN") ?? "";

        if (string.IsNullOrWhiteSpace(dsn))
        {
            Console.WriteLine("❌ Missing FUEL_DSN environment variable.");
            return;
        }

        // For now, just test connection
        await DbInitializer.ApplyMigrationsAsync(dsn);
        Console.WriteLine("✅ Migration applied successfully");
    }
}
