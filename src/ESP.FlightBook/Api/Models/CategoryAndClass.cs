using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents an aircraft category and class
    /// </summary>
    public class CategoryAndClass
    {
        /// <summary>
        /// Unique identifier of a category and class
        /// </summary>
        public int CategoryAndClassId { get; set; }

        /// <summary>
        /// Label of the category and class
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// The aircraft category
        /// </summary>
        [Required]
        public string Category { get; set; }

        /// <summary>
        /// The aircraft class
        /// </summary>
        [Required]
        public string Class { get; set; }

        /// <summary>
        /// Appbreviation of the category/class
        /// </summary>
        public string Abbreviation { get; set; }
    }
}
