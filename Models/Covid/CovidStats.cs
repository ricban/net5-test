namespace Covid19.Client.Models
{
    public class CovidStats
    {
        public int Cases { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }

        public int Active
        {
            get
            {
                var result = Cases - Deaths - Recovered;
                return result < 0 ? 0 : result;
            }
        }
    }
}