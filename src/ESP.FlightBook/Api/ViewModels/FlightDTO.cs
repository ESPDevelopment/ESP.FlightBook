using ESP.FlightBook.Api.Models;
using System;
using System.Collections.Generic;

namespace ESP.FlightBook.Api.ViewModels
{
    public class FlightDTO
    {
        // Flight properties
        public int FlightId { get; set; }
        public int LogbookId { get; set; }
        public int AircraftId { get; set; }
        public DateTime FlightDate { get; set; }
        public string DepartureCode { get; set; }
        public string DestinationCode { get; set; }
        public string Route { get; set; }
        public string Remarks { get; set; }
        public int NumberOfLandingsDay { get; set; }
        public int NumberOfLandingsNight { get; set; }
        public int NumberOfHolds { get; set; }
        public decimal FlightTimeTotal { get; set; }
        public decimal FlightTimeDay { get; set; }
        public decimal FlightTimeNight { get; set; }
        public decimal FlightTimeActualInstrument { get; set; }
        public decimal FlightTimeSimulatedInstrument { get; set; }
        public decimal FlightTimeCrossCountry { get; set; }
        public decimal FlightTimeSolo { get; set; }
        public decimal FlightTimeDual { get; set; }
        public decimal FlightTimePIC { get; set; }
        public bool IsCheckRide { get; set; }
        public bool IsFlightReview { get; set; }
        public bool IsInstrumentProficiencyCheck { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ChangedOn { get; set; }
        public AircraftDTO Aircraft { get; set; }
        public ICollection<ApproachDTO> Approaches { get; set; }

        /// <summary>
        /// Converts resource to a data transfer object
        /// </summary>
        /// <param name="flight">Resource to be converted</param>
        /// <returns>Data transfer object representing the resource</returns>
        public static FlightDTO ToDto(Flight flight)
        {
            return new FlightDTO
            {
                Aircraft = null,
                AircraftId = flight.AircraftId,
                Approaches = new List<ApproachDTO>(),
                ChangedOn = flight.ChangedOn,
                CreatedOn = flight.CreatedOn,
                DepartureCode = flight.DepartureCode,
                DestinationCode = flight.DestinationCode,
                FlightDate = flight.FlightDate,
                FlightId = flight.FlightId,
                FlightTimeActualInstrument = flight.FlightTimeActualInstrument,
                FlightTimeCrossCountry = flight.FlightTimeCrossCountry,
                FlightTimeDay = flight.FlightTimeDay,
                FlightTimeDual = flight.FlightTimeDual,
                FlightTimeNight = flight.FlightTimeNight,
                FlightTimePIC = flight.FlightTimePIC,
                FlightTimeSimulatedInstrument = flight.FlightTimeSimulatedInstrument,
                FlightTimeSolo = flight.FlightTimeSolo,
                FlightTimeTotal = flight.FlightTimeTotal,
                IsCheckRide = flight.IsCheckRide,
                IsFlightReview = flight.IsFlightReview,
                IsInstrumentProficiencyCheck = flight.IsInstrumentProficiencyCheck,
                LogbookId = flight.LogbookId,
                NumberOfHolds = flight.NumberOfHolds,
                NumberOfLandingsDay = flight.NumberOfLandingsDay,
                NumberOfLandingsNight = flight.NumberOfLandingsNight,
                Remarks = flight.Remarks,
                Route = flight.Route
            };
        }
    }
}
