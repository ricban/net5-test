namespace Covid19.Client.Models
{
    public class ApiTotals : IApiTotals
    {
        public long Updated { get; set; }
        public int Cases { get; set; }
        public int TodayCases { get; set; }
        public int Deaths { get; set; }
        public int TodayDeaths { get; set; }
        public int Recovered { get; set; }
        public int TodayRecovered { get; set; }
        public int Active { get; set; }
        public int Critical { get; set; }
        public float CasesPerOneMillion { get; set; }
        public float DeathsPerOneMillion { get; set; }
        public int Tests { get; set; }
        public float TestsPerOneMillion { get; set; }
        public long Population { get; set; }
        public int OneCasePerPeople { get; set; }
        public int OneDeathPerPeople { get; set; }
        public int OneTestPerPeople { get; set; }
        public float ActivePerOneMillion { get; set; }
        public float RecoveredPerOneMillion { get; set; }
        public float CriticalPerOneMillion { get; set; }
    }
}