using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class ChartJsDataset
    {
        public string Label { get; set; } = default!;
        public string BackgroundColor { get; set; } = default!;
        public string BorderColor { get; set; } = default!;
        public int BorderWidth { get; set; } = default!;
        public List<int> Data { get; set; } = default!;
        public bool Fill { get; set; } = default!;
        public int MinBarLength { get; set; } = default!;
    }
}