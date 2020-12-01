namespace Covid19.Client.Models
{
    public class ApiCountryProvinceTotals
    {
        public string Country { get; set; } = default!;
        public string County { get; set; } = default!;
        public string UpdatedAt { get; set; } = default!;
        public ApiStats Stats { get; set; } = default!;
        public ApiCoordinates Coordinates { get; set; } = default!;
        public string Province { get; set; } = default!;
    }
}