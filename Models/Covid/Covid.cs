using System;
using System.Collections.Generic;
using Covid19.Client.Enums;

namespace Covid19.Client.Models
{
    public class Covid : CovidStats
    {
        public CovidCountry Country { get; set; } = default!;
        public long Population { get; set; }
        public long Tests { get; set; }
        public int Critical { get; set; }
        public int CasesToday { get; set; }
        public int DeathsToday { get; set; }
        public int RecoveredToday { get; set; }
        public int ActiveToday { get; set; }
        public int CasesDiff { get; set; }
        public int DeathsDiff { get; set; }
        public int RecoveredDiff { get; set; }
        public int ActiveDiff { get; set; }
        public float CasesPct { get; set; }
        public float DeathsPct { get; set; }
        public float RecoveredPct { get; set; }
        public float ActivePct { get; set; }
        public int FatalityRate => (int)Math.Round(decimal.Divide(Deaths, Cases) * 100, 0);
        public int RecoveryRate => (int)Math.Round(decimal.Divide(Recovered, Cases) * 100, 0);

        public List<CovidTimeline> Timelines { get; set; } = default!;
        public List<CovidRegion> Regions { get; set; } = default!;

        #region Only applies to GLOBAL
        public int AffectedCountries { get; set; }
        public List<CovidRegion> Continents { get; set; } = default!;
        public CovidResearch Vaccines { get; set; } = default!;
        public CovidResearch Therapeutics { get; set; } = default!;
        #endregion

        #region Only applies to COUNTRY
        public CovidNews News { get; set; } = default!;
        public CovidTravelAlert TravelAlert { get; set; } = default!;
        public CovidTravelAdvisory TravelAdvisory { get; set; } = default!;
        #endregion

        #region Riskline Map
        public string InfectionRiskMap { get; set; } = default!;
        public string TravelRiskMap { get; set; } = default!;
        public int MapAspectRatio { get; set; }
        #endregion

        public CovidChart Chart { get; set; } = default!;
        public DateTime LastUpdated { get; set; }
        public Dictionary<ContentKey, CovidError> Errors { get; set; } = default!;
        public CovidView View { get; set; } = default!;
    }
}