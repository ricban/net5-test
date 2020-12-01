using System;

namespace Covid19.Client.Models
{
    public class CovidNewsItem
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string UrlToImage { get; set; } = default!;
        public DateTime PublishedAt { get; set; }
    }
}