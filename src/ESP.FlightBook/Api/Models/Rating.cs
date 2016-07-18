using System;
using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents a rating applied to a certificate
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// Unique identifier of the rating
        /// </summary>
        public int RatingId { get; set; }

        /// <summary>
        /// Unique identifier of the certificate
        /// </summary>
        public int CertificateId { get; set; }

        /// <summary>
        /// The unique identifier of the registered user
        /// </summary>
        [StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// Date the rating was granted
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime RatingDate { get; set; }

        /// <summary>
        /// Type of rating
        /// </summary>
        [Required]
        [StringLength(50)]
        public string RatingType { get; set; }

        /// <summary>
        /// Remarks associated with the rating
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
        /// Navigation property for the related Certificate entity
        /// </summary>
        public Certificate Certificate { get; set; }
    }
}
