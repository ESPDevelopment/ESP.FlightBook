using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents a flight
    /// </summary>
    public class Flight
    {
        /// <summary>
        /// Unique identifier of the flight
        /// </summary>
        public int FlightId { get; set; }

        /// <summary>
        /// Unique identifier of the logbook
        /// </summary>
        public int LogbookId { get; set; }

        /// <summary>
        /// Unique identifier of the aircraft
        /// </summary>
        public int AircraftId { get; set; }

        /// <summary>
        /// The unique identifier of the registered user
        /// </summary>
        [StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// Date of the flight
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime FlightDate { get; set; }

        /// <summary>
        /// ICAO code for the departure airport
        /// </summary>
        [Required]
        [StringLength(5)]
        public string DepartureCode { get; set; }

        /// <summary>
        /// ICAO code for the destination airport
        /// </summary>
        [Required]
        [StringLength(5)]
        public string DestinationCode { get; set; }

        /// <summary>
        /// Optional route of flight
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Remarks associated with the flight
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// Number of landings made during the day
        /// </summary>
        public int NumberOfLandingsDay { get; set; }

        /// <summary>
        /// Number of landings made at night
        /// </summary>
        public int NumberOfLandingsNight { get; set; }

        /// <summary>
        /// Number of landings made at night
        /// </summary>
        public int NumberOfHolds { get; set; }

        /// <summary>
        /// Total amount of flight time
        /// </summary>
        public decimal FlightTimeTotal { get; set; }

        /// <summary>
        /// Amount of flight time that occurred during day conditions
        /// </summary>
        public decimal FlightTimeDay { get; set; }

        /// <summary>
        /// Amount of flight time that occurred during night conditions
        /// </summary>
        public decimal FlightTimeNight { get; set; }

        /// <summary>
        /// Amount of flight time that occurred during actual instrument conditions
        /// </summary>
        public decimal FlightTimeActualInstrument { get; set; }

        /// <summary>
        /// Amount of flight time that occurred during simulated instrument conditions
        /// </summary>
        public decimal FlightTimeSimulatedInstrument { get; set; }

        /// <summary>
        /// Amount of flight time that qualifies as cross-country time
        /// </summary>
        public decimal FlightTimeCrossCountry { get; set; }

        /// <summary>
        /// Amount of flight time that qualifies as solo time
        /// </summary>
        public decimal FlightTimeSolo { get; set; }

        /// <summary>
        /// Amount of flight time that qualifies a dual instruction time
        /// </summary>
        public decimal FlightTimeDual { get; set; }

        /// <summary>
        /// Amount of flight time that qualifies as pilot in command
        /// </summary>
        public decimal FlightTimePIC { get; set; }

        /// <summary>
        /// Indicates whether the flight qualifies as a check ride
        /// </summary>
        public bool IsCheckRide { get; set; }

        /// <summary>
        /// Indicates whether the flight qualifies as a biennial flight review
        /// </summary>
        public bool IsFlightReview { get; set; }

        /// <summary>
        /// Indicates whether the flight qualifies as an instrument proficiency check
        /// </summary>
        public bool IsInstrumentProficiencyCheck { get; set; }

        /// <summary>
        /// The date and time the resource was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The date and time the resource was last changed
        /// </summary>
        public DateTime ChangedOn { get; set; }

        /// <summary>
        /// Navigation property for the related Aircraft entity
        /// </summary>
        public Aircraft Aircraft { get; set; }

        /// <summary>
        /// Navigation property for the related Approach entities
        /// </summary>
        public ICollection<Approach> Approaches { get; set; }
    }
}