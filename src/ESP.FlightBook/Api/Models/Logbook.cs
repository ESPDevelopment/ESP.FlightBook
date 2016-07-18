using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents a pilot's logbook
    /// </summary>
    public class Logbook
    {
        /// <summary>
        /// The unique identifier of the logbook
        /// </summary>
        public int LogbookId { get; set; }

        /// <summary>
        /// The unique identifier of the registered user
        /// </summary>
        [Required]
        [StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// The title associated with the logbook
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Remarks associated with the logbook
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// The date and time the logbook was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The date and time the logbook was last changed
        /// </summary>
        public DateTime ChangedOn { get; set; }

        /// <summary>
        /// Navigation property for related Pilot entity
        /// </summary>
        public Pilot Pilot { get; set; }

        /// <summary>
        /// Navigation property for related Endorsement entities
        /// </summary>
        public ICollection<Endorsement> Endorsements { get; set; }

        /// <summary>
        /// Navigation property for related Certificate entities
        /// </summary>
        public ICollection<Certificate> Certificates { get; set; }

        /// <summary>
        /// Navigation property for related Aircraft entities
        /// </summary>
        public ICollection<Aircraft> Aircraft { get; set; }

        /// <summary>
        /// Navigation property for related Flight entities
        /// </summary>
        public ICollection<Flight> Flights { get; set; }

        /// <summary>
        /// Navigation property for related Currency entities
        /// </summary>
        public ICollection<Currency> Currencies { get; set; }
    }
}
