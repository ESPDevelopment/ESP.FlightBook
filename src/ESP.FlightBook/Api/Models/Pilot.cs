using System;
using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents a pilot's profile
    /// </summary>
    public class Pilot
    {
        /// <summary>
        /// Unique identifier of the pilot's profile
        /// </summary>
        public int PilotId { get; set; }

        /// <summary>
        /// Unique logbook identifier
        /// </summary>
        public int LogbookId { get; set; }

        /// <summary>
        /// The unique identifier of the registered user
        /// </summary>
        [StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// The pilot's primary email address
        /// </summary>
        [EmailAddress]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The pilot's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The pilot's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The first line of the pilot's physical address
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// The second line of the pilot's physical address
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// The city of the pilot's physical address
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The state or province of the pilot's physical address
        /// </summary>
        public string StateOrProvince { get; set; }

        /// <summary>
        /// The country of the pilot's physical address
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// The postal code of the pilot's physical address
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// The pilot's home phone number
        /// </summary>
        public string HomePhoneNumber { get; set; }

        /// <summary>
        /// The pilot's cell phone number
        /// </summary>
        public string CellPhoneNumber { get; set; }

        /// <summary>
        /// The date and time the resource was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The date and time the resource was last changed
        /// </summary>
        public DateTime ChangedOn { get; set; }

        /// <summary>
        /// Navigation property for the associated Logbook
        /// </summary>
        public Logbook Logbook { get; set; }
    }
}