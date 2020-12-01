namespace Covid19.Client.Models
{
    public class CovidChart
    {
        public string Statistics { get; set; } = default!;
        public string FatalityRate { get; set; } = default!;
        public string RecoveryRate { get; set; } = default!;
        public string Timeline { get; set; } = default!;
        public string ContinentBreakdown { get; set; } = default!;
    }
}