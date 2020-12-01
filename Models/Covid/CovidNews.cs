using System.Collections.Generic;
using Covid19.Client.Extensions;

namespace Covid19.Client.Models
{
    public class CovidNews
    {
        public List<CovidNewsItem> Items { get; set; } = default!;
        public string ErrorDetails { get; set; } = default!;
        public bool HasError => !ErrorDetails!.IsNullOrWhiteSpace();
    }
}