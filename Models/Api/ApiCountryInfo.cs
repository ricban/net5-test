namespace Covid19.Client.Models
{
    public class ApiCountryInfo : ApiCoords
    {
        public int? _id { get; set; }
        public string Iso2 { get; set; } = default!;
        public string Iso3 { get; set; } = default!;
        public string Flag { get; set; } = default!;
    }
}
