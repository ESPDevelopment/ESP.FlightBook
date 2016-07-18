using System;

namespace ESP.FlightBook.Api.ViewModels
{
    public class LandingsSummaryViewModel
    {
        // Summary Date
        public DateTime SummaryDate { get; set; }

        // Total Landings
        public decimal LandingsTotalTotal { get; set; }
        public decimal LandingsTotalLast7Days { get; set; }
        public decimal LandingsTotalLast30Days { get; set; }
        public decimal LandingsTotalLast60Days { get; set; }
        public decimal LandingsTotalLast90Days { get; set; }
        public decimal LandingsTotalLast6Months { get; set; }
        public decimal LandingsTotalLast12Months { get; set; }

        // Day Landings
        public decimal LandingsDayTotal { get; set; }
        public decimal LandingsDayLast7Days { get; set; }
        public decimal LandingsDayLast30Days { get; set; }
        public decimal LandingsDayLast60Days { get; set; }
        public decimal LandingsDayLast90Days { get; set; }
        public decimal LandingsDayLast6Months { get; set; }
        public decimal LandingsDayLast12Months { get; set; }

        // Night Landings
        public decimal LandingsNightTotal { get; set; }
        public decimal LandingsNightLast7Days { get; set; }
        public decimal LandingsNightLast30Days { get; set; }
        public decimal LandingsNightLast60Days { get; set; }
        public decimal LandingsNightLast90Days { get; set; }
        public decimal LandingsNightLast6Months { get; set; }
        public decimal LandingsNightLast12Months { get; set; }

        // Approaches
        public decimal ApproachesTotal { get; set; }
        public decimal ApproachesLast7Days { get; set; }
        public decimal ApproachesLast30Days { get; set; }
        public decimal ApproachesLast60Days { get; set; }
        public decimal ApproachesLast90Days { get; set; }
        public decimal ApproachesLast6Months { get; set; }
        public decimal ApproachesLast12Months { get; set; }

        // Holds
        public decimal HoldsTotal { get; set; }
        public decimal HoldsLast7Days { get; set; }
        public decimal HoldsLast30Days { get; set; }
        public decimal HoldsLast60Days { get; set; }
        public decimal HoldsLast90Days { get; set; }
        public decimal HoldsLast6Months { get; set; }
        public decimal HoldsLast12Months { get; set; }
    }
}
