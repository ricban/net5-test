namespace Covid19.Client.Models
{
    public class ApiTravelAdvisoryDataContentAdvisory
    {
        public float Score { get; set; }
        public int Sources_active { get; set; }
        public string Message { get; set; } = default!;
        public string Updated { get; set; } = default!;
        public string Source { get; set; } = default!;
    }
}