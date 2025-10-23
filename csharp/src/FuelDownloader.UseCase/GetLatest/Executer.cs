namespace FuelDownloader.UseCase.GetLatest;

using FuelDownloader.Domain;
using FuelDownloader.Infra.CsvExport;
using FuelDownloader.Infra.Eia;
using FuelDownloader.Infra.Postgres;
using Microsoft.Extensions.Logging;
using System.Threading;

public class Executor
{
    private readonly Client _client;
    private readonly Repo _repo;
    private readonly ILogger<Executor> _logger;

    public Executor(Client client, Repo repo)
    {
        _client = client;
        _repo = repo;
    }

    public async Task<GetLatestResult> ExecuteAsync(string outputPath, string area = "NUS", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching latest diesel price for area {Area}", area);

            var fuelRate = await _client.FetchLatestDieselAsync(area);
            if (fuelRate == null)
            {
                return GetLatestResult.Failure("Failed to get data from EIA API");
            }

            await _repo.UpsertAsync(fuelRate);
            await Writer.WriteAsync(outputPath, fuelRate);

            _logger.LogInformation("Successfully processed fuel rate: {Product} {Period} {Value}",
                fuelRate.ProductCode, fuelRate.Period, fuelRate.Value);

            return GetLatestResult.Success(fuelRate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing GetLatest use case");
            return GetLatestResult.Failure($"Unexpected error: {ex.Message}");
        }
    }
}
