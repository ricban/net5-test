namespace Covid19.Client.Models
{
    public class ChartJsTooltips
    {
        public bool Enabled { get; set; } = default!;
        public string Mode { get; set; } = default!;
        public string Axis { get; set; } = default!;
        public bool Intersect { get; set; } = default!;
    }
}