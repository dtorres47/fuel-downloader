using FuelDownloader.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDownloader.Infra.Postgres
{
    public class Repo
    {
        private readonly string _connectionString;

        public Repo(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Placeholder: upsert a FuelRate into Postgres
        public async Task UpsertAsync(FuelRate fuelRate)
        {
            // TODO: Connect to Postgres, run SQL command
            await Task.CompletedTask;
        }
    }
}
