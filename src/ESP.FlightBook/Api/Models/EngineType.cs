using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents an engine type
    /// </summary>
    public class EngineType
    {
        /// <summary>
        /// Unique identifier of the engine type
        /// </summary>
        public int EngineTypeId { get; set; }

        /// <summary>
        /// Label associated with the engine type
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// Order in which the engine type should be sorted
        /// </summary>
        public int SortOrder { get; set; }
    }
}
