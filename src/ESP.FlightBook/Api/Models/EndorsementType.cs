using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents an endorsement type template
    /// </summary>
    public class EndorsementType
    {
        /// <summary>
        /// Unique identifier of the endorsement type
        /// </summary>
        public int EndorsementTypeId { get; set; }

        /// <summary>
        /// Label associated with the endorsement type
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// Category of endorsement type
        /// </summary>
        [Required]
        public string Category { get; set; }

        /// <summary>
        /// Template text for the endorsement type
        /// </summary>
        [Required]
        public string Template { get; set; }

        /// <summary>
        /// Order in which the endorsement type should be listed
        /// </summary>
        public int SortOrder { get; set; }
    }
}
