using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class AppSettings
    {
        public AppSettingsApp App { get; set; } = default!;
        public AppSettingsApi Api { get; set; } = default!;
        public AppSettingsColor Color { get; set; } = default!;
        public AppSettingsImage Image { get; set; } = default!;
        public AppSettingsInterval Interval { get; set; } = default!;
        public AppSettingsTimeline Timeline { get; set; } = default!;
        public AppSettingsCountry Country { get; set; } = default!;
        public AppSettingsLink Link { get; set; } = default!;
        public AppSettingsNews News { get; set; } = default!;
        public Dictionary<string, AppSettingsMaps> Maps { get; set; } = default!;
    }
}