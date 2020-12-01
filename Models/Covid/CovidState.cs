using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class CovidState
    {
        public List<CovidCountry> Countries { get; set; } = default!;
        public Dictionary<string, Covid> ViewedCountries { get; set; } = default!;
    }
}