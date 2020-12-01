using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Covid19.Client.Models;
using Covid19.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Covid19.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // Configuration
            var configuration = new AppSettings();

            builder.Configuration.Bind(configuration);

            // Memory Cache

            builder.Services.AddMemoryCache();

            // Depedency Injection

            builder.Services.AddHttpClient<DiseaseShService>(client => {
                client.BaseAddress = new Uri(configuration.Api.DiseaseSh);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            });

            builder.Services.AddHttpClient<CoronaTrackerService>(client =>
            {
                client.BaseAddress = new Uri(configuration.Api.CoronaTracker);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { MaxAge = TimeSpan.FromSeconds(86400) };
            });

            builder.Services.AddHttpClient<TravelAdvisoryService>(client =>
            {
                client.BaseAddress = new Uri(configuration.Api.TravelAdvisory);
                client.Timeout = TimeSpan.FromSeconds(30);
                // Need to disable as it throws an Internal Server Exception
                //client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { MaxAge = TimeSpan.FromSeconds(86400) };
            });

            // builder.Services.AddScoped<AppState, AppState>();
            builder.Services.AddSingleton(configuration);

            builder.Services.AddSingleton(sp =>
            {
                var diseaseShService = sp.GetRequiredService<DiseaseShService>();
                var coronaTrackerService = sp.GetRequiredService<CoronaTrackerService>();
                var travelAdvisoryService = sp.GetRequiredService<TravelAdvisoryService>();
                var memoryCache = sp.GetRequiredService<IMemoryCache>();
                var jsRuntime = sp.GetRequiredService<IJSRuntime>();

                return new CovidService(diseaseShService, coronaTrackerService, travelAdvisoryService, configuration, memoryCache, jsRuntime);
            });

            await builder.Build().RunAsync().ConfigureAwait(false);
        }
    }
}