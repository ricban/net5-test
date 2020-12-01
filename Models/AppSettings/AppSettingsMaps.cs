namespace Covid19.Client.Models
{
    public class AppSettingsMaps
    {
        public string InfectionRisk { get; set; } = default!;
        public string TravelRisk { get; set; } = default!;
        public int AspectRatio { get; set; }
    }
}