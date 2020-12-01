namespace Covid19.Client.Models
{
    public class CovidView
    {
        public bool StatisticShowChart { get; set; }
        public bool TimelineViewAsChart { get; set; } = true;
        public bool BreakdownByContinent { get; set; }
        public bool BreakdownByContinentViewAsChart { get; set; }
        public bool BreakdownByRegionViewAsChart { get; set; }
    }
}