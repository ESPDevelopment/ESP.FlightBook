using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents an aircraft gear type
    /// </summary>
    public class GearType
    {
        /// <summary>
        /// Unique identifier of the gear type
        /// </summary>
        public int GearTypeId { get; set; }

        /// <summary>
        /// Label associated with the gear type
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// Abbreviation associated with the gear type
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Order in which the gear type should be sorted
        /// </summary>
        public int SortOrder { get; set; }
    }
}
