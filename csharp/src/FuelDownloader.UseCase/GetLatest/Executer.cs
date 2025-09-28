namespace FuelDownloader.UseCase.GetLatest;

using FuelDownloader.Domain;
using FuelDownloader.Infra.Eia;
using FuelDownloader.Infra.Postgres;
using FuelDownloader.Infra.CsvExport;

public class Executor
{
    private readonly Client _client;
    private readonly Repo _repo;

    public Executor(Client client, Repo repo)
    {
        _client = client;
        _repo = repo;
    }

    public async Task<FuelRate?> ExecuteAsync(string outputPath, string area = "NUS")
    {
        var fr = await _client.FetchLatestDieselAsync(area);
        if (fr == null) return null;

        await _repo.UpsertAsync(fr);
        await Writer.WriteAsync(outputPath, fr);

        return fr;
    }
}
