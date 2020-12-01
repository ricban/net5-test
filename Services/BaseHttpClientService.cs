using System.Net.Http;

namespace Covid19.Client.Services
{
    public abstract class BaseHttpClientService
    {
        protected readonly HttpClient HttpClient;

        protected BaseHttpClientService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public string GetBaseUrl()
        {
            return HttpClient.BaseAddress?.ToString()!;
        }
    }
}