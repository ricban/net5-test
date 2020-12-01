using System.Collections.Generic;

namespace Covid19.Client.Models
{
    public class ApiCountryNews
    {
        public int Total { get; set; }
        public List<ApiCountryNewsItem> Items { get; set; } = default!;
    }
}