using System;
using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents a logbook endorsement
    /// </summary>
    public class Endorsement
    {
        /// <summary>
        /// Unique identifier of the endorsement
        /// </summary>
        public int EndorsementId { get; set; }

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
        /// Date the endorsement was granted
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndorsementDate { get; set; }

        /// <summary>
        /// Endorsement title
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Full text of the endorsement
        /// </summary>
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// Name of the certified flight instructor that signed the endorsement
        /// </summary>
        public string CFIName { get; set; }

        /// <summary>
        /// Certificate number of the certified flight instructor that signed the endorsement
        /// </summary>
        public string CFINumber { get; set; }

        /// <summary>
        /// Expiration date of the certified flight instructors credentials
        /// </summary>
        public string CFIExpiration { get; set; }

        /// <summary>
        /// The date and time the resource was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The date and time the resource was last changed
        /// </summary>
        public DateTime ChangedOn { get; set; }

        /// <summary>
        /// Navigation property for the associated Logbook entity
        /// </summary>
        public Logbook Logbook { get; set; }
    }
}