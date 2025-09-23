using FuelDownloader.Domain;
using FuelDownloader.Infra.Eia;
using FuelDownloader.Infra.Postgres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDownloader.UseCase.GetLatest
{
    public class Executor
    {
        private readonly Client _client;
        private readonly Repo _repo;

        public Executor(Client client, Repo repo)
        {
            _client = client;
            _repo = repo;
        }

        // Placeholder: fetch → upsert → export
        public async Task<FuelRate?> ExecuteAsync(string outputPath, string area = "NUS")
        {
            // TODO: fetch from EIA
            // TODO: upsert into Postgres
            // TODO: write CSV
            await Task.CompletedTask;
            return null;
        }
    }
