using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents an endorsement type template
    /// </summary>
    public class ApproachType
    {
        /// <summary>
        /// Unique identifier of the approach type
        /// </summary>
        public int ApproachTypeId { get; set; }

        /// <summary>
        /// Label associated with the approach type
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// Order in which the approach type should be listed
        /// </summary>
        public int SortOrder { get; set; }

    }
}