using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class ApiContinentTotals : ApiTotals
    {
        public string Continent { get; set; } = default!;
        public ApiCoordinates ContinentInfo { get; set; } = default!;
        public List<string> Countries { get; set; } = default!;
    }
}