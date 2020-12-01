using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Covid19.Client.Models;

namespace Covid19.Client.Services
{
    public class DiseaseShService : BaseHttpClientService
    {
        public DiseaseShService(HttpClient httpClient) : base(httpClient)
        {
        }

        #region GLOBAL scope
        /// <summary>
        /// Get global COVID-19 totals
        /// </summary>
        /// <returns>Task<ApiGlobalTotals></returns>
        public Task<ApiGlobalTotals> GetGlobalTotals()
        {
            return HttpClient.GetFromJsonAsync<ApiGlobalTotals>("all")!;
        }

        /// <summary>
        /// Get global accumulated COVID-19 time series data
        /// </summary>
        /// <param name="lastdays">Number of days to return. Use 'all' for the full data set (e.g. 15, all, 24)</param>
        /// <returns>Task<ApiGlobalTimeline></returns>
        public Task<ApiGlobalTimeline> GetGlobalTimeSeriesData(int lastdays)
        {
            return GetGlobalTimeSeriesData(lastdays.ToString());
        }

        /// <summary>
        /// Get global accumulated COVID-19 time series data
        /// </summary>
        /// <param name="lastdays">Number of days to return. Use 'all' for the full data set (e.g. 15, all, 24)</param>
        /// <returns>Task<ApiGlobalTimeline></returns>
        public Task<ApiGlobalTimeline> GetGlobalTimeSeriesData(string lastdays)
        {
            return HttpClient.GetFromJsonAsync<ApiGlobalTimeline>($"historical/all?lastdays={lastdays}")!;
        }

        /// <summary>
        /// Get COVID-19 totals for all continents
        /// </summary>
        /// <returns>Task<List<ApiContinentTotals>></returns>
        public Task<List<ApiContinentTotals>> GetTotalsForAllContinents()
        {
            return HttpClient.GetFromJsonAsync<List<ApiContinentTotals>>("continents")!;
        }

        /// <summary>
        /// Get COVID-19 totals for all countries
        /// </summary>
        /// <returns>Task<List<ApiCountryTotals>></returns>
        public Task<List<ApiCountryTotals>> GetTotalsForAllCountries()
        {
            return HttpClient.GetFromJsonAsync<List<ApiCountryTotals>>("countries")!;
        }

        /// <summary>
        /// Get vaccine trial data
        /// </summary>
        /// <returns>Task<ApiVaccine></returns>
        public Task<ApiVaccine> GetVaccineTrialData()
        {
            return HttpClient.GetFromJsonAsync<ApiVaccine>("vaccine")!;
        }

        /// <summary>
        /// COVID-19 therapeutic trial data
        /// </summary>
        /// <returns>Task<ApiTherapeutic></returns>
        public Task<ApiTherapeutic> GetTherapeuticsTrialData()
        {
            return HttpClient.GetFromJsonAsync<ApiTherapeutic>("therapeutics")!;
        }
        #endregion

        #region COUNTRY scope
        /// <summary>
        /// Get COVID-19 totals for a specific country
        /// </summary>
        /// <param name="country">A country name, iso2, iso3, or country ID code</param>
        /// <returns>Task<ApiCountryTotals></returns>
        public Task<ApiCountryTotals> GetCountryTotals(string country)
        {
            return HttpClient.GetFromJsonAsync<ApiCountryTotals>($"countries/{country}?strict=true")!;
        }

        /// <summary>
        /// Get COVID-19 time series data for a specific country
        /// </summary>
        /// <param name="country">A country name, iso2, iso3, or country ID code</param>
        /// <param name="lastdays">Number of days to return. Use 'all' for the full data set (e.g. 15, all, 24)</param>
        /// <returns>Task<ApiCountryTimeline></returns>
        public Task<ApiCountryTimeline> GetCountryTimeSeriesData(string country, int lastdays)
        {
            return GetCountryTimeSeriesData(country, lastdays.ToString());
        }

        /// <summary>
        /// Get COVID-19 time series data for a specific country
        /// </summary>
        /// <param name="country">A country name, iso2, iso3, or country ID code</param>
        /// <param name="lastdays">Number of days to return. Use 'all' for the full data set (e.g. 15, all, 24)</param>
        /// <returns>Task<ApiCountryTimeline></returns>
        public Task<ApiCountryTimeline> GetCountryTimeSeriesData(string country, string lastdays)
        {
            return HttpClient.GetFromJsonAsync<ApiCountryTimeline>($"historical/{country }?lastdays={lastdays}")!;
        }

        /// <summary>
        /// Get COVID-19 totals for all countries and their provinces
        /// </summary>
        /// <returns>Task<List<ApiCountryProvinceTotals>></returns>
        public Task<List<ApiCountryProvinceTotals>> GetTotalsForAllCountriesAndTheirProvinces()
        {
            return HttpClient.GetFromJsonAsync<List<ApiCountryProvinceTotals>>("jhucsse")!;
        }
        #endregion
    }
}