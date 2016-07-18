using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents a certificate rating type
    /// </summary>
    public class RatingType
    {
        /// <summary>
        /// Unique identifier of the rating type
        /// </summary>
        public int RatingTypeId { get; set; }

        /// <summary>
        /// Label associated with the rating type
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// Order in which the rating type should be sorted
        /// </summary>
        public int SortOrder { get; set; }
    }
}
