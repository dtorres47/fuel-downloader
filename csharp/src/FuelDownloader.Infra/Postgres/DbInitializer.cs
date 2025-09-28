using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDownloader.Infra.Postgres
{
    public static class DbInitializer
    {
        public static async Task ApplyMigrationsAsync(string connectionString)
        {
            // TODO: Apply schema from db/schema/eia_fuel_price.sql
            await Task.CompletedTask;
        }
    }
}
