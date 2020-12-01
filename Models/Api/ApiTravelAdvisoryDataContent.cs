namespace Covid19.Client.Models
{
    public class ApiTravelAdvisoryDataContent
    {
        public string Iso_alpha2 { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Continent { get; set; } = default!;
        public ApiTravelAdvisoryDataContentAdvisory Advisory { get; set; } = default!;
    }
}