using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents a certificate
    /// </summary>
    public class Certificate
    {
        /// <summary>
        /// Unique identifier of the certificate
        /// </summary>
        public int CertificateId { get; set; }

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
        /// Date the certificate was granted
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime CertificateDate { get; set; }

        /// <summary>
        /// Type of certificate
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CertificateType { get; set; }

        /// <summary>
        /// Certificate number (can be a string)
        /// </summary>
        [Required]
        [StringLength(20)]
        public string CertificateNumber { get; set; }

        /// <summary>
        /// Date the certificate expires, if it expires
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Remarks associated with the certificate
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
        /// Navigation property for the related Logbook entity
        /// </summary>
        public Logbook Logbook { get; set; }

        /// <summary>
        /// Navigation property for the related Rating entities
        /// </summary>
        public ICollection<Rating> Ratings { get; set; }
    }
}