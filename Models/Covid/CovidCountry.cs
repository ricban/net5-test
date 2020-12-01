namespace Covid19.Client.Models
{
    public class CovidCountry
    {
        public CovidCountry()
        {
        }

        public CovidCountry(CovidCountry country)
        {
            Id = country.Id;
            Name = country.Name;
            Flag = country.Flag;
            Continent = country.Continent;
        }

        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Flag { get; set; } = default!;
        public string Continent { get; set; } = default!;
    }
}