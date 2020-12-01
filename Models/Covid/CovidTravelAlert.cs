using System;

namespace Covid19.Client.Models
{
    public class CovidTravelAlert
    {
        public DateTime PublishedDate { get; set; }
        public string AlertMessage { get; set; } = default!;
    }
}