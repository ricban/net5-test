namespace Covid19.Client.Models
{
    public class ChartJsOptions
    {
        public bool Responsive { get; set; } = true;
        public ChartJsLegend Legend { get; set; } = default!;
        public ChartJsTitle Title { get; set; } = default!;
        public ChartJsTooltips Tooltips { get; set; } = default!;
        public ChartJsScales Scales { get; set; } = default!;

        // use by RadialGauge

        public string TrackColor { get; set; } = default!;
        public ChartJsCenterArea CenterArea { get; set; } = default!;
    }
}