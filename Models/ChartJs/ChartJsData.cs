using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class ChartJsData
    {
        public List<string> Labels { get; set; } = default!;
        public List<ChartJsDataset> Datasets { get; set; } = default!;
    }
}
