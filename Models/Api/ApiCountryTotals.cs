namespace Covid19.Client.Models
{
    public class ApiCountryTotals : ApiTotals
    {
        public string Country { get; set; } = default!;
        public ApiCountryInfo CountryInfo { get; set; } = default!;
        public string Continent { get; set; } = default!;
    }
}