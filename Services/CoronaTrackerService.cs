using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Covid19.Client.Models;

namespace Covid19.Client.Services
{
    public class CoronaTrackerService : BaseHttpClientService
    {
        public CoronaTrackerService(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        /// Get trending news related to COVID-19 for a specific country
        /// </summary>
        /// <param name="countryName">Country Name</param>
        /// <param name="limit">Limit the number of results</param>
        /// <returns>Task<ApiCountryNews></returns>
        public Task<ApiCountryNews> GetNews(string countryName, int limit)
        {
            return HttpClient.GetFromJsonAsync<ApiCountryNews>($"news/trending?limit={limit}&offset&country={countryName}&countryCode")!;
        }
    }
}