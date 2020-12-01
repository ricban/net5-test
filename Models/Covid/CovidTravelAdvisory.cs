using System;
using Covid19.Client.Extensions;

namespace Covid19.Client.Models
{
    public class CovidTravelAdvisory
    {
        public string Country { get; set; } = default!;
        public float Score { get; set; }
        public int Source { get; set; }
        public string DetailsUrl { get; set; } = default!;
        public DateTime Updated { get; set; }
        public string ErrorDetails { get; set; } = default!;
        public bool HasError => !ErrorDetails!.IsNullOrWhiteSpace();
    }
}