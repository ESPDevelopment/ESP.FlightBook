using System;
using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents an instrument approach to landing
    /// </summary>
    public class Approach
    {
        /// <summary>
        /// Unique identifier of the approach
        /// </summary>
        public int ApproachId { get; set; }

        /// <summary>
        /// Unique identifier of the flight
        /// </summary>
        public int FlightId { get; set; }

        /// <summary>
        /// The unique identifier of the registered user
        /// </summary>
        [StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// ICAO code for the approach airport
        /// </summary>
        [Required]
        public string AirportCode { get; set; }

        /// <summary>
        /// Type of approach
        /// </summary>
        [Required]
        public string ApproachType { get; set; }

        /// <summary>
        /// Runway associated with the approach
        /// </summary>
        [Required]
        public string Runway { get; set; }

        /// <summary>
        /// Indicates whether the approach was a circle-to-land approach
        /// </summary>
        public bool IsCircleToLand { get; set; }

        /// <summary>
        /// Remarks associated with the approach
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// The date and time the resource was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The date and time the resource was last changed
        /// </summary>
        public DateTime ChangedOn { get; set; }

        /// <summary>
        /// Navigation property for the related Flight entity
        /// </summary>
        public Flight Flight { get; set; }
    }
}
