using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents a type of Pilot certificate
    /// </summary>
    public class CertificateType
    {
        /// <summary>
        /// Unique identifier of the certificate type
        /// </summary>
        public int CertificateTypeId { get; set; }

        /// <summary>
        /// Label associated with the certificate type
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// Order in which the certificate type should be sorted
        /// </summary>
        public int SortOrder { get; set; }
    }
}
