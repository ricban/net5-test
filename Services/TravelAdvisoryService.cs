using System.Net.Http;
using System.Threading.Tasks;
using Covid19.Client.Extensions;
using Covid19.Client.Models;

namespace Covid19.Client.Services
{
    public class TravelAdvisoryService : BaseHttpClientService
    {
        public TravelAdvisoryService(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        ///  Get travel warning status of a country
        /// </summary>
        /// <param name="countryIso2">ISO 2 letter country code</param>
        /// <returns>Task<ApiTravelAdvisory></returns>
        public async Task<ApiTravelAdvisory> GetTravelAdvisoryAsync(string countryIso2)
        {
            var response = await HttpClient.GetStringAsync($"api?countrycode={countryIso2}").ConfigureAwait(false);
            var json = response.Replace($"\"{countryIso2}\":", "\"content\":");

            return json.Deserialize<ApiTravelAdvisory>()!;
        }
    }
}