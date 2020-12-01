using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class ApiVaccineData
    {
        public string Candidate { get; set; } = default!;
        public string Mechanism { get; set; } = default!;
        public List<string> Sponsors { get; set; } = default!;
        public string Details { get; set; } = default!;
        public string TrialPhase { get; set; } = default!;
        public List<string> Institutions { get; set; } = default!;
    }
}