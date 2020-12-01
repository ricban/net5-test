namespace Covid19.Client.Models
{
    public class CovidRegion : CovidStats
    {
        public string Name { get; set; } = default!;
        public int Critical { get; set; }
        public int Tests { get; set; }
    }
}