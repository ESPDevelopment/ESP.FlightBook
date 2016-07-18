using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents an aircraft
    /// </summary>
    public class Aircraft
    {
        /// <summary>
        /// Unique identifier of the aircraft
        /// </summary>
        public int AircraftId { get; set; }

        /// <summary>
        /// Unique identifier of the logbook
        /// </summary>
        public int LogbookId { get; set; }

        /// <summary>
        /// The unique identifier of the registered user
        /// </summary>
        [StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// Aircraft identifier
        /// </summary>
        [Required]
        [StringLength(10)]
        public string AircraftIdentifier { get; set; }

        /// <summary>
        /// Type of aircraft
        /// </summary>
        [Required]
        [StringLength(10)]
        public string AircraftType { get; set; }

        /// <summary>
        /// Category of aircraft
        /// </summary>
        public string AircraftCategory { get; set; }

        /// <summary>
        /// Class of aircraft
        /// </summary>
        public string AircraftClass { get; set; }

        /// <summary>
        /// Make of aircraft (manufacturer)
        /// </summary>
        public string AircraftMake { get; set; }

        /// <summary>
        /// Model of aircraft
        /// </summary>
        public string AircraftModel { get; set; }

        /// <summary>
        /// Year the aircraft was manufactured
        /// </summary>
        public int AircraftYear { get; set; }

        /// <summary>
        /// Type of landing gear
        /// </summary>
        public string GearType { get; set; }

        /// <summary>
        /// Type of engine
        /// </summary>
        public string EngineType { get; set; }

        /// <summary>
        /// Complex aircraft
        /// </summary>
        public bool IsComplex { get; set; }

        /// <summary>
        /// High performance aircraft
        /// </summary>
        public bool IsHighPerformance { get; set; }

        /// <summary>
        /// Pressurized aircraft
        /// </summary>
        public bool IsPressurized { get; set; }

        /// <summary>
        /// The date and time the resource was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The date and time the resource was last changed
        /// </summary>
        public DateTime ChangedOn { get; set; }

        /// <summary>
        /// Navigation property for the related Logbook entity
        /// </summary>
        public Logbook Logbook { get; set; }

        /// <summary>
        /// Navigation property for the related Flight entities
        /// </summary>
        public ICollection<Flight> Flights { get; set; }
    }
}
