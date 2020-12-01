namespace Covid19.Client.Models
{
    public class ChartJsAxes
    {
        public bool Display { get; set; } = default!;
        public bool Stacked { get; set; } = default!;
        public ChartJsScaleLabel ScaleLabel { get; set; } = default!;
    }
}