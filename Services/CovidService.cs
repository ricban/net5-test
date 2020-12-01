using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Covid19.Client.Enums;
using Covid19.Client.Extensions;
using Covid19.Client.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;

namespace Covid19.Client.Services
{
    public class CovidService
    {
        private readonly DiseaseShService _diseaseSh;
        private readonly CoronaTrackerService _coronaTracker;
        private readonly TravelAdvisoryService _travelAdvisory;
        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly IJSRuntime _jsRuntime;
        private Dictionary<ContentKey, CovidError> _errors = default!;
        private bool _global;

        public CovidService(DiseaseShService diseaseSh, CoronaTrackerService coronaTracker, TravelAdvisoryService travelAdvisory, AppSettings appSettings, IMemoryCache memoryCache, IJSRuntime jsRuntime)
        {
            _diseaseSh = diseaseSh;
            _coronaTracker = coronaTracker;
            _travelAdvisory = travelAdvisory;
            _appSettings = appSettings;
            _memoryCache = memoryCache;
            _jsRuntime = jsRuntime;
        }

        public async Task<Covid> GetDataAsync(string countryName)
        {
            _global = IsGlobal(countryName);
            _errors = new Dictionary<ContentKey, CovidError>();

            if (!_memoryCache.TryGetValue(countryName, out Covid covid))
            {
                var tokenExpirationHours = TimeSpan.FromHours(_appSettings.Interval.Default);
                var absoluteExpiration = DateTime.Now.Add(tokenExpirationHours);
                var countries = await GetCountriesAsync().ConfigureAwait(false);
                var countryInfo = countries.First(f => f.Name.Equals(countryName));

                if (_global)
                {
                    covid = await GetGlobalDataAsync(countryInfo, tokenExpirationHours, absoluteExpiration).ConfigureAwait(false);
                }
                else
                {
                    covid = await GetCountryDataAsync(countryInfo, tokenExpirationHours, absoluteExpiration).ConfigureAwait(false);
                }

                covid.View = new();

                if (_appSettings.Maps.TryGetValue(covid.Country.Name, out AppSettingsMaps? risklineMap))
                {
                    covid.InfectionRiskMap = risklineMap.InfectionRisk;
                    covid.TravelRiskMap = risklineMap.TravelRisk;
                    covid.MapAspectRatio = risklineMap.AspectRatio;
                }

                var yesterday = covid.Timelines.LastOrDefault();
                var confirmedYesterday = 0;
                var deathsYesterday = 0;
                var recoveredYesterday = 0;
                var activeYesterday = 0;

                if (yesterday != null)
                {
                    confirmedYesterday = yesterday.Cases;
                    deathsYesterday = yesterday.Deaths;
                    recoveredYesterday = yesterday.Recovered;
                    activeYesterday = confirmedYesterday - deathsYesterday - recoveredYesterday;

                    if (activeYesterday < 0)
                    {
                        activeYesterday = 0;
                    }
                }

                covid.ActiveToday = covid.CasesToday - covid.DeathsToday - covid.RecoveredToday;

                if (covid.ActiveToday < 0)
                {
                    covid.ActiveToday = 0;
                }

                covid.CasesDiff = covid.Cases - confirmedYesterday;
                covid.DeathsDiff = covid.Deaths - deathsYesterday;
                covid.RecoveredDiff = covid.Recovered - recoveredYesterday;
                covid.ActiveDiff = covid.CasesDiff - covid.DeathsDiff - covid.RecoveredDiff;
                covid.CasesPct = (confirmedYesterday == 0) ? 0 : Math.Abs((float)decimal.Divide(covid.CasesDiff, confirmedYesterday));
                covid.DeathsPct = (deathsYesterday == 0) ? 0 : Math.Abs((float)decimal.Divide(covid.DeathsDiff, deathsYesterday));
                covid.RecoveredPct = (recoveredYesterday == 0) ? 0 : Math.Abs((float)decimal.Divide(covid.RecoveredDiff, recoveredYesterday));
                covid.ActivePct = (activeYesterday == 0) ? 0 : Math.Abs((float)decimal.Divide(covid.ActiveDiff, activeYesterday));
                covid.Chart = BuildChartConfiguirations(covid);
                covid.Errors = _errors;

                await SetLastViewedCountryAsync(countryName).ConfigureAwait(false);

                _memoryCache.Set(countryInfo.Name, covid, GetMemoryCacheEntryOptions(tokenExpirationHours, absoluteExpiration, CacheItemPriority.Low));
            }

            return covid;
        }

        #region GLOBAL
        private async Task<Covid> GetGlobalDataAsync(CovidCountry countryInfo, TimeSpan tokenExpirationHours, DateTime absoluteExpiration)
        {
            var totalsTask = _diseaseSh.GetGlobalTotals();
            var timelinesTask = _diseaseSh.GetGlobalTimeSeriesData(_appSettings.Timeline.Limit);
            var continentsTask = CreateContinentsAsync();
            var regionsTask = GetTotalsForAllCountriesAsync(tokenExpirationHours, absoluteExpiration);
            var vaccinesTask = GetVaccineData();
            var therapeuticsTask = GetTherapeuticData();

            await Task.WhenAll(totalsTask, timelinesTask, continentsTask, regionsTask, vaccinesTask, therapeuticsTask).ConfigureAwait(false);

            var totals = await totalsTask.ConfigureAwait(false);
            var covid = InitializeCovid(countryInfo, totals);

            covid.AffectedCountries = totals.AffectedCountries;
            covid.Timelines = CreateTimeline(await timelinesTask.ConfigureAwait(false));
            covid.Regions = CreateRegions(await regionsTask.ConfigureAwait(false));
            covid.Continents = await continentsTask.ConfigureAwait(false);
            covid.Vaccines = await vaccinesTask.ConfigureAwait(false);
            covid.Therapeutics = await therapeuticsTask.ConfigureAwait(false);

            return covid;
        }

        private async Task<List<CovidRegion>> CreateContinentsAsync()
        {
            var data = await _diseaseSh.GetTotalsForAllContinents().ConfigureAwait(false);

            return data.Select(s => new CovidRegion
            {
                Name = s.Continent,
                Cases = s.Cases,
                Deaths = s.Deaths,
                Recovered = s.Recovered,
                Critical = s.Critical,
                Tests = s.Tests
            }).OrderByDescending(o => o.Cases).ToList();
        }

        private List<CovidRegion> CreateRegions(List<ApiCountryTotals> data)
        {
            return data.Select(s => new CovidRegion
            {
                Name = s.Country,
                Cases = s.Cases,
                Deaths = s.Deaths,
                Recovered = s.Recovered,
                Critical = s.Critical,
                Tests = s.Tests
            }).OrderByDescending(o => o.Cases).ToList();
        }
        #endregion

        #region COUNTRY
        private async Task<Covid> GetCountryDataAsync(CovidCountry countryInfo, TimeSpan tokenExpirationHours, DateTime absoluteExpiration)
        {
            var totalsTask = _diseaseSh.GetCountryTotals(countryInfo.Id);
            var timelineTask = _diseaseSh.GetCountryTimeSeriesData(countryInfo.Id, _appSettings.Timeline.Limit);
            var regionTask = CreateRegionsAsync(countryInfo, tokenExpirationHours, absoluteExpiration);
            var newsTask = GetNewsAsync(countryInfo);
            var travelAdvisoryTask = GetTravelAdvisoryAsync(countryInfo);
            var vaccinesTask = GetVaccineData();
            var therapeuticsTask = GetTherapeuticData();

            await Task.WhenAll(totalsTask, timelineTask, regionTask, newsTask, travelAdvisoryTask, vaccinesTask, therapeuticsTask).ConfigureAwait(false);

            var covid = InitializeCovid(countryInfo, await totalsTask.ConfigureAwait(false));

            covid.Timelines = CreateTimeline((await timelineTask.ConfigureAwait(false)).Timeline!);
            covid.Regions = await regionTask.ConfigureAwait(false);
            covid.News = await newsTask.ConfigureAwait(false);
            covid.TravelAdvisory = await travelAdvisoryTask.ConfigureAwait(false);
            covid.Vaccines = await vaccinesTask.ConfigureAwait(false);
            covid.Therapeutics = await therapeuticsTask.ConfigureAwait(false);

            return covid;
        }

        private async Task<List<CovidRegion>> CreateRegionsAsync(CovidCountry countryInfo, TimeSpan tokenExpirationHours, DateTime absoluteExpiration)
        {
            if (!_memoryCache.TryGetValue(CacheKey.CountryProvinceTotals, out List<ApiCountryProvinceTotals> data))
            {
                data = await _diseaseSh.GetTotalsForAllCountriesAndTheirProvinces().ConfigureAwait(false);

                _memoryCache.Set(CacheKey.CountryProvinceTotals, data, GetMemoryCacheEntryOptions(tokenExpirationHours, absoluteExpiration, CacheItemPriority.Low));
            }

            if (!_appSettings.Country.TranslateNames.TryGetValue(countryInfo.Name, out string? name))
            {
                name = countryInfo.Name;
            }

            return data.Where(w => (w.Country == name || w.Country == countryInfo.Id) && !w.Province.IsNullOrEmpty())
                .Select(s => new CovidRegion
                {
                    Name = s.Province,
                    Cases = s.Stats.Confirmed,
                    Deaths = s.Stats.Deaths,
                    Recovered = s.Stats.Recovered,
                }).OrderByDescending(o => o.Cases).ToList();
        }

        private async Task<CovidNews> GetNewsAsync(CovidCountry country)
        {
            var result = new CovidNews();

            try
            {
                if (!_appSettings.News.NameReplacement.TryGetValue(country.Id, out string? countryName))
                {
                    countryName = country.Name;
                }

                var data = await _coronaTracker.GetNews(countryName, _appSettings.News.DisplayLimit).ConfigureAwait(false);

                result.Items = data.Items.ConvertAll(s => new CovidNewsItem
                {
                    Title = s.Title.Trim(),
                    Description = fixDescription(s.Description),
                    Url = s.Url,
                    UrlToImage = s.UrlToImage,
                    PublishedAt = DateTime.Parse(s.PublishedAt)
                }).Where(w => DateTime.Today.Subtract(w.PublishedAt).TotalDays <= _appSettings.News.NewsDayAgeLimit).ToList();
            }
            catch (Exception ex)
            {
                result.ErrorDetails = ex.ToHtml();
            }

            return result;

            string fixDescription(string value)
            {
                var newValue = value.Trim();

                if (newValue.Length > _appSettings.News.DescriptionDisplayLimit)
                {
                    return newValue.Truncate(_appSettings.News.DescriptionDisplayLimit)!;
                }

                return $"{newValue}{(newValue.EndsWith(".") ? "" : "...")}";
            }
        }

        private async Task<CovidTravelAdvisory> GetTravelAdvisoryAsync(CovidCountry country)
        {
            var result = new CovidTravelAdvisory();

            if (!_appSettings.Maps.ContainsKey(country.Name))
            {
                try
                {
                    var response = await _travelAdvisory.GetTravelAdvisoryAsync(country.Id).ConfigureAwait(false);

                    result.Country = response.Data.Content.Name;
                    result.Score = response.Data.Content.Advisory.Score;
                    result.Source = response.Data.Content.Advisory.Sources_active;
                    result.DetailsUrl = response.Data.Content.Advisory.Source;
                    result.Updated = DateTime.Parse(response.Data.Content.Advisory.Updated).ToLocalTime();
                }
                catch (Exception ex)
                {
                    result.ErrorDetails = ex.ToHtml();
                }
            }

            return result;
        }
        #endregion

        #region SHARED / COMMON
        private Covid InitializeCovid(CovidCountry countryInfo, IApiTotals data)
        {
            return new()
            {
                Country = new CovidCountry(countryInfo),
                Population = data.Population,
                Cases = data.Cases,
                Deaths = data.Deaths,
                Recovered = data.Recovered,
                CasesToday = data.TodayCases,
                DeathsToday = data.TodayDeaths,
                RecoveredToday = data.TodayRecovered,
                Tests = data.Tests,
                Critical = data.Critical,
                LastUpdated = DateTimeOffset.FromUnixTimeMilliseconds(data.Updated).LocalDateTime
            };
        }

        private async Task<CovidResearch> GetVaccineData()
        {
            if (!_memoryCache.TryGetValue(CacheKey.Vaccine, out CovidResearch data))
            {
                try
                {
                    var result = await _diseaseSh.GetVaccineTrialData().ConfigureAwait(false);

                    data = new CovidResearch
                    {
                        Source = result.Source,
                        Candidates = result.Data.Count
                    };

                    _memoryCache.Set(CacheKey.Vaccine, data, GetMemoryCacheEntryOptions(_appSettings.Interval.Vaccine));
                }
                catch (Exception ex)
                {
                    _errors.Add(ContentKey.Vaccines, new CovidError { Message = "Vaccine tracker information is not available at this time.", Details = ex.ToHtml() });
                }
            }

            return data;
        }

        private async Task<CovidResearch> GetTherapeuticData()
        {
            if (!_memoryCache.TryGetValue(CacheKey.Therapeutic, out CovidResearch data))
            {
                try
                {
                    var result = await _diseaseSh.GetTherapeuticsTrialData().ConfigureAwait(false);

                    data = new CovidResearch
                    {
                        Source = result.Source,
                        Candidates = result.Data.Count
                    };

                    _memoryCache.Set(CacheKey.Therapeutic, data, GetMemoryCacheEntryOptions(_appSettings.Interval.Therapeutic));
                }
                catch (Exception ex)
                {
                    _errors.Add(ContentKey.Therapeutics, new CovidError { Message = "Therapeutics tracker information is not available at this time.", Details = ex.ToHtml() });
                }
            }

            return data;
        }

        public async Task<List<CovidCountry>> GetCountriesAsync()
        {
            if (!_memoryCache.TryGetValue(CacheKey.Countries, out List<CovidCountry> data))
            {
                var tokenExpirationHours = TimeSpan.FromHours(_appSettings.Interval.Default);
                var absoluteExpiration = DateTime.Now.Add(tokenExpirationHours);
                var result = await GetTotalsForAllCountriesAsync(tokenExpirationHours, absoluteExpiration).ConfigureAwait(false);

                data = result
                    .OrderByDescending(o => o.Cases)
                    .Select(s => new CovidCountry
                    {
                        Id = s.CountryInfo.Iso2,
                        Name = s.Country,
                        Flag = s.CountryInfo.Flag,
                        Continent = s.Continent
                    }).ToList();

                data.Insert(0, GlobalInfo());

                _memoryCache.Set(CacheKey.Countries, data, GetMemoryCacheEntryOptions(_appSettings.Interval.Countries));
            }

            return data;
        }

        public bool IsGlobal(string countryName) => (countryName ?? string.Empty).Equals(_appSettings.Country.World.Name, StringComparison.InvariantCultureIgnoreCase);

        public CovidCountry GlobalInfo()
        {
            return new CovidCountry
            {
                Id = _appSettings.Country.World.Name,
                Name = _appSettings.Country.World.Name,
                Flag = _appSettings.Country.World.Flag
            };
        }

        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(double interval, CacheItemPriority priority = CacheItemPriority.Normal, PostEvictionDelegate? callback = null)
        {
            var tokenExpirationHours = TimeSpan.FromHours(interval);
            var absoluteExpiration = DateTime.Now.Add(tokenExpirationHours);

            return GetMemoryCacheEntryOptions(tokenExpirationHours, absoluteExpiration, priority, callback);
        }

        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(TimeSpan tokenExpirationHours, DateTime absoluteExpiration, CacheItemPriority priority = CacheItemPriority.Normal, PostEvictionDelegate? callback = null)
        {
            var memoryCache = (MemoryCache)_memoryCache;
            var expirationToken = new CancellationChangeToken(new CancellationTokenSource(tokenExpirationHours).Token);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
               .SetPriority(priority)
               .SetAbsoluteExpiration(absoluteExpiration)
               .AddExpirationToken(expirationToken)
               .SetSize(memoryCache.Count + 1);

            if (callback != null)
            {
                cacheEntryOptions.RegisterPostEvictionCallback(callback: callback);
            }

            return cacheEntryOptions;
        }

        private async Task<List<ApiCountryTotals>> GetTotalsForAllCountriesAsync(TimeSpan tokenExpirationHours, DateTime absoluteExpiration)
        {
            if (!_memoryCache.TryGetValue(CacheKey.TotalsForAllCountries, out List<ApiCountryTotals> data))
            {
                data = await _diseaseSh.GetTotalsForAllCountries().ConfigureAwait(false);
                _memoryCache.Set(CacheKey.TotalsForAllCountries, data, GetMemoryCacheEntryOptions(tokenExpirationHours, absoluteExpiration, CacheItemPriority.Low));
            }

            return data;
        }

        private List<CovidTimeline> CreateTimeline(ApiTimeline data)
        {
            var result = new List<CovidTimeline>();

            if (data != null)
            {
                foreach (var item in data.Cases)
                {
                    result.Add(new CovidTimeline
                    {
                        Date = DateTime.ParseExact(item.Key, "M/d/yy", CultureInfo.InvariantCulture),
                        Cases = item.Value,
                        Deaths = data.Deaths[item.Key],
                        Recovered = data.Recovered[item.Key]
                    });
                }
            }

            return result.OrderBy(o => o.Date).ToList();
        }

        private CovidChart BuildChartConfiguirations(Covid covid)
        {
            var serializerOptions = ChartSerializerOptions();

            var chartConfigurations = new CovidChart
            {
                Statistics = new ChartJs
                (
                    ChartJsType.Bar,
                    new ChartJsData
                    {
                        Datasets = new List<ChartJsDataset>
                        {
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Cases),
                                BackgroundColor =  _appSettings.Color.Cases,
                                BorderColor =  _appSettings.Color.Cases,
                                BorderWidth = 1,
                                MinBarLength = 2,
                                Data = new List<int> { covid.Cases }
                            },
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Active),
                                BackgroundColor =  _appSettings.Color.Active,
                                BorderColor =  _appSettings.Color.Active,
                                BorderWidth = 1,
                                MinBarLength = 2,
                                Data = new List<int> { covid.Active }
                            },
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Recovered),
                                BackgroundColor =  _appSettings.Color.Recovered,
                                BorderColor =  _appSettings.Color.Recovered,
                                BorderWidth = 1,
                                MinBarLength = 2,
                                Data = new List<int> { covid.Recovered }
                            },
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Deaths),
                                BackgroundColor =  _appSettings.Color.Deaths,
                                BorderColor =  _appSettings.Color.Deaths,
                                BorderWidth = 1,
                                MinBarLength = 2,
                                Data = new List<int> { covid.Deaths }
                            }
                        }
                    },
                    new ChartJsOptions
                    {
                        Legend = new ChartJsLegend { Display = true, Position = ChartJsPosition.Bottom },
                        Title = new ChartJsTitle { Display = false },
                    }
                ).Serialize(serializerOptions),

                FatalityRate = new ChartJs
                (
                    ChartJsType.RadialGauge,
                    new ChartJsData
                    {
                        Labels = new List<string> { "Fatality Rate" },
                        Datasets = new List<ChartJsDataset>
                        {
                            new ChartJsDataset
                            {
                                BackgroundColor = _appSettings.Color.Deaths,
                                Data = new List<int> { covid.FatalityRate }
                            }
                        }
                    },
                    new ChartJsOptions
                    {
                        Title = new ChartJsTitle { Display = true, Text = "Fatality Rate", Position = ChartJsPosition.Bottom },
                        Tooltips = new ChartJsTooltips { Enabled = false },
                        TrackColor = "rgba(196, 33, 39, 0.20)",
                        CenterArea = new ChartJsCenterArea { DisplayText = true, FontColor = _appSettings.Color.Deaths }
                    }
                ).Serialize(serializerOptions),

                RecoveryRate = new ChartJs
                (
                    ChartJsType.RadialGauge,
                    new ChartJsData
                    {
                        Labels = new List<string> { "Recovery Rate" },
                        Datasets = new List<ChartJsDataset>
                        {
                            new ChartJsDataset
                            {
                                BackgroundColor = _appSettings.Color.Recovered,
                                Data = new List<int> { covid.RecoveryRate }
                            }
                        }
                    },
                    new ChartJsOptions
                    {
                        Title = new ChartJsTitle { Display = true, Text = "Recovery Rate", Position = ChartJsPosition.Bottom },
                        Tooltips = new ChartJsTooltips { Enabled = false },
                        TrackColor = "rgba(56, 161, 105, 0.20)",
                        CenterArea = new ChartJsCenterArea { DisplayText = true, FontColor = _appSettings.Color.Recovered }
                    }
                ).Serialize(serializerOptions),

                Timeline = new ChartJs
                (
                    ChartJsType.Line,
                    new ChartJsData
                    {
                        Labels = covid.Timelines.ConvertAll(i => i.Date.ToString(_appSettings.Timeline.DateFormat)),
                        Datasets = new List<ChartJsDataset>
                        {
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Cases),
                                BackgroundColor = _appSettings.Color.Cases,
                                BorderColor = _appSettings.Color.Cases,
                                BorderWidth = 1,
                                Fill = false,
                                Data = covid.Timelines.ConvertAll(i => i.Cases)
                            },
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Active),
                                BackgroundColor = _appSettings.Color.Active,
                                BorderColor = _appSettings.Color.Active,
                                BorderWidth = 1,
                                Fill = false,
                                Data = covid.Timelines.ConvertAll(i => i.Active)
                            },
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Recovered),
                                BackgroundColor = _appSettings.Color.Recovered,
                                BorderColor = _appSettings.Color.Recovered,
                                BorderWidth = 1,
                                Fill = false,
                                Data = covid.Timelines.ConvertAll(i => i.Recovered)
                            },
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Deaths),
                                BackgroundColor = _appSettings.Color.Deaths,
                                BorderColor = _appSettings.Color.Deaths,
                                BorderWidth = 1,
                                Fill = false,
                                Data = covid.Timelines.ConvertAll(i => i.Deaths)
                            }
                        }
                    },
                    new ChartJsOptions
                    {
                        Legend = new ChartJsLegend { Display = true, Position = ChartJsPosition.Top },
                        Title = new ChartJsTitle { Display = false },
                        Tooltips = new ChartJsTooltips { Enabled = true, Mode = ChartJsTooltipsMode.Index, Intersect = false }
                    }
                ).Serialize(serializerOptions)
            };

            if (covid.Continents?.Count > 0)
            {
                chartConfigurations.ContinentBreakdown = new ChartJs
                (
                    ChartJsType.HorizontalBar,
                    new ChartJsData
                    {
                        Labels = covid.Continents.ConvertAll(i => i.Name),
                        Datasets = new List<ChartJsDataset>
                        {
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Cases),
                                BackgroundColor = _appSettings.Color.Cases,
                                BorderColor = _appSettings.Color.Cases,
                                BorderWidth = 1,
                                MinBarLength = 2,
                                Data = covid.Continents.ConvertAll(i => i.Cases)
                            },
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Active),
                                BackgroundColor = _appSettings.Color.Active,
                                BorderColor = _appSettings.Color.Active,
                                BorderWidth = 1,
                                MinBarLength = 2,
                                Data = covid.Continents.ConvertAll(i => i.Active)
                            },
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Recovered),
                                BackgroundColor = _appSettings.Color.Recovered,
                                BorderColor = _appSettings.Color.Recovered,
                                BorderWidth = 1,
                                MinBarLength = 2,
                                Data = covid.Continents.ConvertAll(i => i.Recovered)
                            },
                            new ChartJsDataset
                            {
                                Label = nameof(_appSettings.Color.Deaths),
                                BackgroundColor = _appSettings.Color.Deaths,
                                BorderColor = _appSettings.Color.Deaths,
                                BorderWidth = 1,
                                MinBarLength = 2,
                                Data = covid.Continents.ConvertAll(i => i.Deaths)
                            }
                        }
                    },
                    new ChartJsOptions
                    {
                        Legend = new ChartJsLegend { Display = true, Position = ChartJsPosition.Top },
                        Tooltips = new ChartJsTooltips { Enabled = true, Mode = ChartJsTooltipsMode.Index, Axis = ChartJsAxis.Y }
                    }
                ).Serialize(serializerOptions);
            }

            return chartConfigurations;
        }

        public string GetRegionsChartConfiguration(List<CovidRegion> datasource)
        {
            return new ChartJs
            (
                ChartJsType.HorizontalBar,
                new ChartJsData
                {
                    Labels = datasource.ConvertAll(i => i.Name),
                    Datasets = new List<ChartJsDataset>
                    {
                        new ChartJsDataset
                        {
                            Label = nameof(_appSettings.Color.Cases),
                            BackgroundColor = _appSettings.Color.Cases,
                            BorderColor = _appSettings.Color.Cases,
                            BorderWidth = 1,
                            MinBarLength = 2,
                            Data = datasource.ConvertAll(i => i.Cases)
                        },
                        new ChartJsDataset
                        {
                            Label = nameof(_appSettings.Color.Active),
                            BackgroundColor = _appSettings.Color.Active,
                            BorderColor = _appSettings.Color.Active,
                            BorderWidth = 1,
                            MinBarLength = 2,
                            Data = datasource.ConvertAll(i => i.Active)
                        },
                        new ChartJsDataset
                        {
                            Label = nameof(_appSettings.Color.Recovered),
                            BackgroundColor = _appSettings.Color.Recovered,
                            BorderColor = _appSettings.Color.Recovered,
                            BorderWidth = 1,
                            MinBarLength = 2,
                            Data = datasource.ConvertAll(i => i.Recovered)
                        },
                        new ChartJsDataset
                        {
                            Label = nameof(_appSettings.Color.Deaths),
                            BackgroundColor = _appSettings.Color.Deaths,
                            BorderColor = _appSettings.Color.Deaths,
                            BorderWidth = 1,
                            MinBarLength = 2,
                            Data = datasource.ConvertAll(i => i.Deaths)
                        }
                    }
                },
                new ChartJsOptions
                {
                    Legend = new ChartJsLegend { Display = true, Position = ChartJsPosition.Top },
                    Tooltips = new ChartJsTooltips { Enabled = true, Mode = ChartJsTooltipsMode.Index, Axis = ChartJsAxis.Y }
                }
            ).Serialize(ChartSerializerOptions());
        }

        public static JsonSerializerOptions ChartSerializerOptions(bool writeIndented = false) => new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = writeIndented
        };
        #endregion

        private const string _countryStoreKey = "country";

        public async Task<string> GetLastViewedCountryAsync()
        {
            var value = await _jsRuntime.InvokeAsync<string>("JsInterop.localStore.get", _countryStoreKey);

            if (value.IsNullOrEmpty())
            {
                value = _appSettings.Country.World.Name;
            }

            return value;
        }

        public async Task SetLastViewedCountryAsync(string countryName)
        {
            await _jsRuntime.InvokeAsync<string>("JsInterop.localStore.set", _countryStoreKey, countryName);
        }
    }
}