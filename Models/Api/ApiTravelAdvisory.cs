namespace Covid19.Client.Models
{
    public class ApiTravelAdvisory
    {
        public ApiTravelAdvisoryStatus Api_status { get; set; } = default!;
        public ApiTravelAdvisoryData Data { get; set; } = default!;
    }
}