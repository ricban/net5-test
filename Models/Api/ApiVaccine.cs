using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class ApiVaccine
    {
        public string Source { get; set; } = default!;
        public string TotalCandidates { get; set; } = default!;
        public List<ApiPhase> Phases { get; set; } = default!;
        public List<ApiVaccineData> Data { get; set; } = default!;
    }
}