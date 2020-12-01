using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class AppSettingsNews
    {
        public int DisplayLimit { get; set; }
        public double NewsDayAgeLimit { get; set; }
        public int DescriptionDisplayLimit { get; set; }
        public Dictionary<string, string> NameReplacement { get; set; } = default!;
    }
}