namespace Covid19.Client.Models
{
    public interface IApiTotals
    {
        int Active { get; set; }
        float ActivePerOneMillion { get; set; }
        int Cases { get; set; }
        float CasesPerOneMillion { get; set; }
        int Critical { get; set; }
        float CriticalPerOneMillion { get; set; }
        int Deaths { get; set; }
        float DeathsPerOneMillion { get; set; }
        int OneCasePerPeople { get; set; }
        int OneDeathPerPeople { get; set; }
        int OneTestPerPeople { get; set; }
        long Population { get; set; }
        int Recovered { get; set; }
        float RecoveredPerOneMillion { get; set; }
        int Tests { get; set; }
        float TestsPerOneMillion { get; set; }
        int TodayCases { get; set; }
        int TodayDeaths { get; set; }
        int TodayRecovered { get; set; }
        long Updated { get; set; }
    }
}