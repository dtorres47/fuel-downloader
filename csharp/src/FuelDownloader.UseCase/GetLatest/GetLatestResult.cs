using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelDownloader.Domain;

namespace FuelDownloader.UseCase.GetLatest
{
    public class GetLatestResult
    {
        public bool IsSuccess { get; init; }
        public FuelRate? Data { get; init; }
        public string? ErrorMessage { get; init; }

        public static GetLatestResult Success(FuelRate data) =>
            new() { IsSuccess = true, Data = data };

        public static GetLatestResult Failure(string error) =>
            new() { IsSuccess = false, ErrorMessage = error };
    }
}
