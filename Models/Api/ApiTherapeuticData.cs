using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class ApiTherapeuticData
    {
        public string MedicationClass { get; set; } = default!;
        public List<string> TradeName { get; set; } = default!;
        public string Details { get; set; } = default!;
        public List<string> DeveloperResearcher { get; set; } = default!;
        public List<string> Sponsors { get; set; } = default!;
        public string TrialPhase { get; set; } = default!;
        public string LastUpdate { get; set; } = default!;
    }
}