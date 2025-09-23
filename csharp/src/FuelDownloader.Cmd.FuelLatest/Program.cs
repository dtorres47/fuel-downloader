using FuelDownloader.Infra.Eia;
using FuelDownloader.Infra.Postgres;
using FuelDownloader.UseCase.GetLatest;

class Program
{
    static async Task Main(string[] args)
    {
        // TODO: read env vars for API key, DSN, and output path

        var apiKey = "TODO-get-from-env";
        var dsn = "TODO-get-from-env";
        var outputPath = "fuel-latest.csv";

        var client = new Client(apiKey);
        var repo = new Repo(dsn);
        var executor = new Executor(client, repo);

        var result = await executor.ExecuteAsync(outputPath);

        Console.WriteLine(result != null
            ? $"✅ Success: {result.ProductCode} {result.Period:yyyy-MM} {result.Value} {result.Unit}"
            : "❌ Failed to fetch fuel rate");
    }
}
