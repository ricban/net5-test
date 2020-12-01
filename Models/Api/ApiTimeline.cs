using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class ApiTimeline
    {
        public Dictionary<string, int> Cases { get; set; } = default!;
        public Dictionary<string, int> Deaths { get; set; } = default!;
        public Dictionary<string, int> Recovered { get; set; } = default!;
    }
}