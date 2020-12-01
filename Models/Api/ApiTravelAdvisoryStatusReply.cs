namespace Covid19.Client.Models
{
    public class ApiTravelAdvisoryStatusReply
    {
        public string Cache { get; set; } = default!;
        public int Code { get; set; }
        public string Status { get; set; } = default!;
        public string Note { get; set; } = default!;
        public int Count { get; set; }
    }
}