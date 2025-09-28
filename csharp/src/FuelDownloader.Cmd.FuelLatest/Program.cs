namespace FuelDownloader.Cmd.FuelLatest;

using FuelDownloader.Infra.Eia;
using FuelDownloader.Infra.Postgres;
using FuelDownloader.UseCase.GetLatest;

class Program
{
    static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("EIA_API_KEY") ?? "";
        var dsn = Environment.GetEnvironmentVariable("FUEL_DSN") ?? "";
        var outputPath = Environment.GetEnvironmentVariable("FUEL_OUT") ?? "fuel-latest.csv";
        var area = Environment.GetEnvironmentVariable("FUEL_AREA") ?? "NUS";

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(dsn))
        {
            Console.WriteLine("❌ Missing required environment variables: EIA_API_KEY or FUEL_DSN");
            return;
        }

        var client = new Client(apiKey);
        var repo = new Repo(dsn);
        var executor = new Executor(client, repo);

        var fr = await executor.ExecuteAsync(outputPath, area);

        if (fr != null)
        {
            Console.WriteLine($"✅ {fr.ProductCode} {fr.Period:yyyy-MM} {fr.Value} {fr.Unit} → {outputPath}");
        }
        else
        {
            Console.WriteLine("❌ Failed to fetch fuel rate");
        }
    }
}
