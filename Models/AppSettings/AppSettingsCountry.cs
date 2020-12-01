using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class AppSettingsCountry
    {
        public AppSettingsCountryWorld World { get; set; } = default!;
        public Dictionary<string, string> TranslateNames { get; set; } = default!;
    }
}