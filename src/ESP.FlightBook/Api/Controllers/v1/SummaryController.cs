using ESP.FlightBook.Api.Models;
using ESP.FlightBook.Api.ViewModels;
using ESP.FlightBook.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESP.FlightBook.Api.Controllers.v1
{
    public class SummaryController : LogbookApiController
    {
        protected readonly ILogger<SummaryController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public SummaryController(ApplicationDbContext apiDbContext, ILogger<SummaryController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns a summary of flight hours for the specified logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested resource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/summary/aircraft</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/summary/aircraft", Name = "GetAircraftSummaryRoute")]
        public async Task<IActionResult> GetAircraftSummary(int logbookId)
        {
            // Find associated logbook
            Logbook logbook = await _apiDbContext.Logbooks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LogbookId == logbookId);

            // Return not found result
            if (logbook == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = logbook.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Create the view model
            Dictionary<string, AircraftSummary> aircraftSummaryDict = new Dictionary<string, AircraftSummary>();
            DateTime summaryDate = DateTime.Now;

            // Select flights for specified pilot
            List<Flight> flights = new List<Flight>();
            flights = await _apiDbContext.Flights
                .Include(f => f.Aircraft)
                .Where(f => f.LogbookId == logbookId)
                .OrderBy(f => f.Aircraft.AircraftIdentifier)
                .ToListAsync();

            // Iterate through flights
            foreach (Flight flight in flights)
            {
                // Create new aircraft summary or retrieve existing
                AircraftSummary aircraftSummary = null;
                if (aircraftSummaryDict.ContainsKey(flight.Aircraft.AircraftIdentifier))
                {
                    aircraftSummary = aircraftSummaryDict[flight.Aircraft.AircraftIdentifier];
                }
                else
                {
                    aircraftSummary = new AircraftSummary();
                    aircraftSummary.Aircraft = AircraftDTO.ToDto(flight.Aircraft);
                    aircraftSummaryDict.Add(flight.Aircraft.AircraftIdentifier, aircraftSummary);
                }

                // Increment statistics
                aircraftSummary.Approaches += (flight.Approaches == null) ? 0 : flight.Approaches.Count;
                aircraftSummary.Holds += flight.NumberOfHolds;
                aircraftSummary.HoursActualInstrument += flight.FlightTimeActualInstrument;
                aircraftSummary.HoursDay += flight.FlightTimeDay;
                aircraftSummary.HoursDual += flight.FlightTimeDual;
                aircraftSummary.HoursNight += flight.FlightTimeNight;
                aircraftSummary.HoursPIC += flight.FlightTimePIC;
                aircraftSummary.HoursSimulatedInstrument += flight.FlightTimeSimulatedInstrument;
                aircraftSummary.HoursSolo += flight.FlightTimeSolo;
                aircraftSummary.HoursTotal += flight.FlightTimeTotal;
                aircraftSummary.HoursXC += flight.FlightTimeCrossCountry;
                aircraftSummary.LandingsDay += flight.NumberOfLandingsDay;
                aircraftSummary.LandingsNight += flight.NumberOfLandingsNight;
                aircraftSummary.LandingsTotal += (flight.NumberOfLandingsDay + flight.NumberOfLandingsNight);
            }

            // Create summary list
            AircraftSummaryViewModel viewModel = new AircraftSummaryViewModel();
            viewModel.Aircraft = new List<AircraftSummary>();
            foreach (KeyValuePair<string, AircraftSummary> item in aircraftSummaryDict)
            {
                viewModel.Aircraft.Add(item.Value);
            }

            // Return the result
            return Ok(viewModel);
        }

        /// <summary>
        /// Returns a summary of flight hours for the specified logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested resource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/summary/hours</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/summary/hours", Name = "GetHoursSummaryRoute")]
        public async Task<IActionResult> GetHoursSummary(int logbookId)
        {
            // Find associated logbook
            Logbook logbook = await _apiDbContext.Logbooks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LogbookId == logbookId);

            // Return not found result
            if (logbook == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = logbook.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Create the view model
            HoursSummaryViewModel viewModel = new HoursSummaryViewModel();
            viewModel.SummaryDate = DateTime.Now;

            // Create year map
            viewModel.TotalHoursByYear = new Dictionary<int, decimal>();

            // Create an aircraft type map
            viewModel.TotalHoursByType = new Dictionary<string, decimal>();

            // Calculate recency dates
            DateTime daysAgo7 = viewModel.SummaryDate.AddDays(-7);
            DateTime daysAgo30 = viewModel.SummaryDate.AddDays(-30);
            DateTime daysAgo60 = viewModel.SummaryDate.AddDays(-60);
            DateTime daysAgo90 = viewModel.SummaryDate.AddDays(-90);
            DateTime daysAgo6Months = viewModel.SummaryDate.AddMonths(-6);
            DateTime daysAgo12Months = viewModel.SummaryDate.AddMonths(-12);

            // Select flights for specified pilot
            List<Flight> flights = new List<Flight>();
            flights = await _apiDbContext.Flights
                .Include(f => f.Aircraft)
                .Where(f => f.LogbookId == logbookId)
                .OrderByDescending(f => f.FlightDate)
                .ToListAsync();

            // Calculate hours summary data
            foreach (Flight flight in flights)
            {
                // Update year map
                int year = flight.FlightDate.Year;
                if (viewModel.TotalHoursByYear.ContainsKey(year))
                {
                    viewModel.TotalHoursByYear[year] += flight.FlightTimeTotal;
                }
                else
                {
                    viewModel.TotalHoursByYear.Add(year, flight.FlightTimeTotal);
                }

                // Update type map
                if (viewModel.TotalHoursByType.ContainsKey(flight.Aircraft.AircraftType))
                {
                    viewModel.TotalHoursByType[flight.Aircraft.AircraftType] += flight.FlightTimeTotal;
                }
                else
                {
                    viewModel.TotalHoursByType.Add(flight.Aircraft.AircraftType, flight.FlightTimeTotal);
                }

                // Total hours
                viewModel.HoursTotalTotal += flight.FlightTimeTotal;
                viewModel.HoursPICTotal += flight.FlightTimePIC;
                viewModel.HoursDualTotal += flight.FlightTimeDual;
                viewModel.HoursSoloTotal += flight.FlightTimeSolo;
                viewModel.HoursXCTotal += flight.FlightTimeCrossCountry;
                viewModel.HoursDayTotal += flight.FlightTimeDay;
                viewModel.HoursNightTotal += flight.FlightTimeNight;
                viewModel.HoursActualInstrumentTotal += flight.FlightTimeActualInstrument;
                viewModel.HoursSimulatedInstrumentTotal += flight.FlightTimeSimulatedInstrument;
                viewModel.HoursComplexTotal += (flight.Aircraft.IsComplex) ? flight.FlightTimeTotal : 0;
                viewModel.HoursHPTotal += (flight.Aircraft.IsHighPerformance) ? flight.FlightTimeTotal : 0;
                viewModel.HoursPressurizedTotal += (flight.Aircraft.IsPressurized) ? flight.FlightTimeTotal : 0;

                // Last 7 days
                if (flight.FlightDate >= daysAgo7)
                {
                    viewModel.HoursTotalLast7Days += flight.FlightTimeTotal;
                    viewModel.HoursPICLast7Days += flight.FlightTimePIC;
                    viewModel.HoursDualLast7Days += flight.FlightTimeDual;
                    viewModel.HoursSoloLast7Days += flight.FlightTimeSolo;
                    viewModel.HoursXCLast7Days += flight.FlightTimeCrossCountry;
                    viewModel.HoursDayLast7Days += flight.FlightTimeDay;
                    viewModel.HoursNightLast7Days += flight.FlightTimeNight;
                    viewModel.HoursActualInstrumentLast7Days += flight.FlightTimeActualInstrument;
                    viewModel.HoursSimulatedInstrumentLast7Days += flight.FlightTimeSimulatedInstrument;
                    viewModel.HoursComplexLast7Days += (flight.Aircraft.IsComplex) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursHPLast7Days += (flight.Aircraft.IsHighPerformance) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursPressurizedLast7Days += (flight.Aircraft.IsPressurized) ? flight.FlightTimeTotal : 0;
                }

                // Last 30 days
                if (flight.FlightDate >= daysAgo30)
                {
                    viewModel.HoursTotalLast30Days += flight.FlightTimeTotal;
                    viewModel.HoursPICLast30Days += flight.FlightTimePIC;
                    viewModel.HoursDualLast30Days += flight.FlightTimeDual;
                    viewModel.HoursSoloLast30Days += flight.FlightTimeSolo;
                    viewModel.HoursXCLast30Days += flight.FlightTimeCrossCountry;
                    viewModel.HoursDayLast30Days += flight.FlightTimeDay;
                    viewModel.HoursNightLast30Days += flight.FlightTimeNight;
                    viewModel.HoursActualInstrumentLast30Days += flight.FlightTimeActualInstrument;
                    viewModel.HoursSimulatedInstrumentLast30Days += flight.FlightTimeSimulatedInstrument;
                    viewModel.HoursComplexLast30Days += (flight.Aircraft.IsComplex) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursHPLast30Days += (flight.Aircraft.IsHighPerformance) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursPressurizedLast30Days += (flight.Aircraft.IsPressurized) ? flight.FlightTimeTotal : 0;
                }

                // Last 60 days
                if (flight.FlightDate >= daysAgo60)
                {
                    viewModel.HoursTotalLast60Days += flight.FlightTimeTotal;
                    viewModel.HoursPICLast60Days += flight.FlightTimePIC;
                    viewModel.HoursDualLast60Days += flight.FlightTimeDual;
                    viewModel.HoursSoloLast60Days += flight.FlightTimeSolo;
                    viewModel.HoursXCLast60Days += flight.FlightTimeCrossCountry;
                    viewModel.HoursDayLast60Days += flight.FlightTimeDay;
                    viewModel.HoursNightLast60Days += flight.FlightTimeNight;
                    viewModel.HoursActualInstrumentLast60Days += flight.FlightTimeActualInstrument;
                    viewModel.HoursSimulatedInstrumentLast60Days += flight.FlightTimeSimulatedInstrument;
                    viewModel.HoursComplexLast60Days += (flight.Aircraft.IsComplex) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursHPLast60Days += (flight.Aircraft.IsHighPerformance) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursPressurizedLast60Days += (flight.Aircraft.IsPressurized) ? flight.FlightTimeTotal : 0;
                }

                // Last 90 days
                if (flight.FlightDate >= daysAgo90)
                {
                    viewModel.HoursTotalLast90Days += flight.FlightTimeTotal;
                    viewModel.HoursPICLast90Days += flight.FlightTimePIC;
                    viewModel.HoursDualLast90Days += flight.FlightTimeDual;
                    viewModel.HoursSoloLast90Days += flight.FlightTimeSolo;
                    viewModel.HoursXCLast90Days += flight.FlightTimeCrossCountry;
                    viewModel.HoursDayLast90Days += flight.FlightTimeDay;
                    viewModel.HoursNightLast90Days += flight.FlightTimeNight;
                    viewModel.HoursActualInstrumentLast90Days += flight.FlightTimeActualInstrument;
                    viewModel.HoursSimulatedInstrumentLast90Days += flight.FlightTimeSimulatedInstrument;
                    viewModel.HoursComplexLast90Days += (flight.Aircraft.IsComplex) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursHPLast90Days += (flight.Aircraft.IsHighPerformance) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursPressurizedLast90Days += (flight.Aircraft.IsPressurized) ? flight.FlightTimeTotal : 0;
                }

                // Last 6 months
                if (flight.FlightDate >= daysAgo6Months)
                {
                    viewModel.HoursTotalLast6Months += flight.FlightTimeTotal;
                    viewModel.HoursPICLast6Months += flight.FlightTimePIC;
                    viewModel.HoursDualLast6Months += flight.FlightTimeDual;
                    viewModel.HoursSoloLast6Months += flight.FlightTimeSolo;
                    viewModel.HoursXCLast6Months += flight.FlightTimeCrossCountry;
                    viewModel.HoursDayLast6Months += flight.FlightTimeDay;
                    viewModel.HoursNightLast6Months += flight.FlightTimeNight;
                    viewModel.HoursActualInstrumentLast6Months += flight.FlightTimeActualInstrument;
                    viewModel.HoursSimulatedInstrumentLast6Months += flight.FlightTimeSimulatedInstrument;
                    viewModel.HoursComplexLast6Months += (flight.Aircraft.IsComplex) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursHPLast6Months += (flight.Aircraft.IsHighPerformance) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursPressurizedLast6Months += (flight.Aircraft.IsPressurized) ? flight.FlightTimeTotal : 0;
                }

                // Last 12 months
                if (flight.FlightDate >= daysAgo12Months)
                {
                    viewModel.HoursTotalLast12Months += flight.FlightTimeTotal;
                    viewModel.HoursPICLast12Months += flight.FlightTimePIC;
                    viewModel.HoursDualLast12Months += flight.FlightTimeDual;
                    viewModel.HoursSoloLast12Months += flight.FlightTimeSolo;
                    viewModel.HoursXCLast12Months += flight.FlightTimeCrossCountry;
                    viewModel.HoursDayLast12Months += flight.FlightTimeDay;
                    viewModel.HoursNightLast12Months += flight.FlightTimeNight;
                    viewModel.HoursActualInstrumentLast12Months += flight.FlightTimeActualInstrument;
                    viewModel.HoursSimulatedInstrumentLast12Months += flight.FlightTimeSimulatedInstrument;
                    viewModel.HoursComplexLast12Months += (flight.Aircraft.IsComplex) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursHPLast12Months += (flight.Aircraft.IsHighPerformance) ? flight.FlightTimeTotal : 0;
                    viewModel.HoursPressurizedLast12Months += (flight.Aircraft.IsPressurized) ? flight.FlightTimeTotal : 0;
                }
            }

            // Return the result
            return Ok(viewModel);
        }

        /// <summary>
        /// Returns a summary of landings and approaches for the specified logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested resource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/summary/landings</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/summary/landings", Name = "GetLandingsSummaryRoute")]
        public async Task<IActionResult> GetLandingsSummary(int logbookId)
        {
            // Find associated logbook
            Logbook logbook = await _apiDbContext.Logbooks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LogbookId == logbookId);

            // Return not found result
            if (logbook == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = logbook.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Create the view model
            LandingsSummaryViewModel viewModel = new LandingsSummaryViewModel();
            viewModel.SummaryDate = DateTime.Now;

            // Calculate recency dates
            DateTime daysAgo7 = viewModel.SummaryDate.AddDays(-7);
            DateTime daysAgo30 = viewModel.SummaryDate.AddDays(-30);
            DateTime daysAgo60 = viewModel.SummaryDate.AddDays(-60);
            DateTime daysAgo90 = viewModel.SummaryDate.AddDays(-90);
            DateTime daysAgo6Months = viewModel.SummaryDate.AddMonths(-6);
            DateTime daysAgo12Months = viewModel.SummaryDate.AddMonths(-12);

            // Select flights for specified pilot
            List<Flight> flights = new List<Flight>();
            flights = await _apiDbContext.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Approaches)
                .Where(f => f.LogbookId == logbookId)
                .OrderByDescending(f => f.FlightDate)
                .ToListAsync();

            // Calculate landings summary data
            foreach (Flight flight in flights)
            {
                // Total landings
                viewModel.LandingsTotalTotal += (flight.NumberOfLandingsDay + flight.NumberOfLandingsNight);
                viewModel.LandingsDayTotal += flight.NumberOfLandingsDay;
                viewModel.LandingsNightTotal += flight.NumberOfLandingsNight;
                viewModel.ApproachesTotal += (flight.Approaches == null) ? 0 : flight.Approaches.Count;
                viewModel.HoldsTotal += flight.NumberOfHolds;

                // Last 7 days
                if (flight.FlightDate >= daysAgo7)
                {
                    viewModel.LandingsTotalLast7Days += (flight.NumberOfLandingsDay + flight.NumberOfLandingsNight);
                    viewModel.LandingsDayLast7Days += flight.NumberOfLandingsDay;
                    viewModel.LandingsNightLast7Days += flight.NumberOfLandingsNight;
                    viewModel.ApproachesLast7Days += (flight.Approaches == null) ? 0 : flight.Approaches.Count;
                    viewModel.HoldsLast7Days += flight.NumberOfHolds;
                }

                // Last 30 days
                if (flight.FlightDate >= daysAgo30)
                {
                    viewModel.LandingsTotalLast30Days += (flight.NumberOfLandingsDay + flight.NumberOfLandingsNight);
                    viewModel.LandingsDayLast30Days += flight.NumberOfLandingsDay;
                    viewModel.LandingsNightLast30Days += flight.NumberOfLandingsNight;
                    viewModel.ApproachesLast30Days += (flight.Approaches == null) ? 0 : flight.Approaches.Count;
                    viewModel.HoldsLast30Days += flight.NumberOfHolds;
                }

                // Last 60 days
                if (flight.FlightDate >= daysAgo60)
                {
                    viewModel.LandingsTotalLast60Days += (flight.NumberOfLandingsDay + flight.NumberOfLandingsNight);
                    viewModel.LandingsDayLast60Days += flight.NumberOfLandingsDay;
                    viewModel.LandingsNightLast60Days += flight.NumberOfLandingsNight;
                    viewModel.ApproachesLast60Days += (flight.Approaches == null) ? 0 : flight.Approaches.Count;
                    viewModel.HoldsLast60Days += flight.NumberOfHolds;
                }

                // Last 90 days
                if (flight.FlightDate >= daysAgo90)
                {
                    viewModel.LandingsTotalLast90Days += (flight.NumberOfLandingsDay + flight.NumberOfLandingsNight);
                    viewModel.LandingsDayLast90Days += flight.NumberOfLandingsDay;
                    viewModel.LandingsNightLast90Days += flight.NumberOfLandingsNight;
                    viewModel.ApproachesLast90Days += (flight.Approaches == null) ? 0 : flight.Approaches.Count;
                    viewModel.HoldsLast90Days += flight.NumberOfHolds;
                }

                // Last 6 months
                if (flight.FlightDate >= daysAgo6Months)
                {
                    viewModel.LandingsTotalLast6Months += (flight.NumberOfLandingsDay + flight.NumberOfLandingsNight);
                    viewModel.LandingsDayLast6Months += flight.NumberOfLandingsDay;
                    viewModel.LandingsNightLast6Months += flight.NumberOfLandingsNight;
                    viewModel.ApproachesLast6Months += (flight.Approaches == null) ? 0 : flight.Approaches.Count;
                    viewModel.HoldsLast6Months += flight.NumberOfHolds;
                }

                // Last 12 months
                if (flight.FlightDate >= daysAgo12Months)
                {
                    viewModel.LandingsTotalLast12Months += (flight.NumberOfLandingsDay + flight.NumberOfLandingsNight);
                    viewModel.LandingsDayLast12Months += flight.NumberOfLandingsDay;
                    viewModel.LandingsNightLast12Months += flight.NumberOfLandingsNight;
                    viewModel.ApproachesLast12Months += (flight.Approaches == null) ? 0 : flight.Approaches.Count;
                    viewModel.HoldsLast12Months += flight.NumberOfHolds;
                }
            }

            // Return the result
            return Ok(viewModel);
        }
    }
}
