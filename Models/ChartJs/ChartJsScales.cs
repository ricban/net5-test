using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class ChartJsScales
    {
        public List<ChartJsAxes> XAxes { get; set; } = default!;
        public List<ChartJsAxes> YAxes { get; set; } = default!;
    }
}