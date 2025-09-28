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
        public FuelRate? FuelRate { get; set; }
        public bool Success => FuelRate != null;
    }
}
