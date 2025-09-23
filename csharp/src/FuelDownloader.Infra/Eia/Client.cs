using FuelDownloader.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDownloader.Infra.Eia
{
    public class Client
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public Client(string apiKey, HttpClient? httpClient = null)
        {
            _apiKey = apiKey;
            _httpClient = httpClient ?? new HttpClient();
        }

        // Placeholder: fetch the latest diesel fuel rate
        public async Task<FuelRate?> FetchLatestDieselAsync(string area = "NUS")
        {
            // TODO: Call the EIA API, parse JSON, return FuelRate
            await Task.CompletedTask;
            return null;
        }
    }
