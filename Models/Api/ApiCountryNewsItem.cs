namespace Covid19.Client.Models
{
    public class ApiCountryNewsItem
    {
        public int Nid { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string Author { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string UrlToImage { get; set; } = default!;
        public string PublishedAt { get; set; } = default!;
        public string AddedOn { get; set; } = default!;
        public string SiteName { get; set; } = default!;
        public string Language { get; set; } = default!;
        public string CountryCode { get; set; } = default!;
        public int Status { get; set; }
    }
}