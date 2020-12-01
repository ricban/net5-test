using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class ApiCountryTimeline
    {
        public string? Country { get; set; }
        public List<string>? Province { get; set; }
        public ApiTimeline? Timeline { get; set; }
    }
}