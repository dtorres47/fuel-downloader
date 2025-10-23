namespace FuelDownloader.Cmd.FuelLatest;

using FuelDownloader.Infra.Eia;
using FuelDownloader.Infra.Postgres;
using FuelDownloader.UseCase.GetLatest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Configuration
                var apiKey = Environment.GetEnvironmentVariable("EIA_API_KEY") ?? "";
                var dsn = Environment.GetEnvironmentVariable("FUEL_DSN") ?? "";

                // Register services
                services.AddHttpClient("EiaClient");
                services.AddSingleton(sp => new Client(apiKey,
                    sp.GetRequiredService<IHttpClientFactory>()));
                services.AddSingleton(new Repo(dsn));
                services.AddTransient<Executor>();
                services.AddLogging(builder => builder.AddConsole());
            })
            .Build();

        var executor = host.Services.GetRequiredService<Executor>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        var outputPath = Environment.GetEnvironmentVariable("FUEL_OUT") ?? "fuel-latest.csv";
        var area = Environment.GetEnvironmentVariable("FUEL_AREA") ?? "NUS";

        var result = await executor.ExecuteAsync(outputPath, area);

        if (result.IsSuccess && result.Data != null)
        {
            var fr = result.Data;
            Console.WriteLine($"{fr.ProductCode} {fr.Period:yyyy-MM} {fr.Value} {fr.Unit} → {outputPath}");
        }
        else
        {
            logger.LogError("Failed to get fuel rate: {Error}", result.ErrorMessage);
            Console.WriteLine($"{result.ErrorMessage}");
            Environment.Exit(1);
        }
    }
}
