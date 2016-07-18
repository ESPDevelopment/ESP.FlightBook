using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents a type of currency
    /// </summary>
    public class CurrencyType
    {
        public enum CalculationTypes { FlightReview, General, IFR };

        /// <summary>
        /// Unique identifier of the currency type
        /// </summary>
        public int CurrencyTypeId { get; set; }

        /// <summary>
        /// Category associated with the currency type
        /// </summary>
        [Required]
        public string Category { get; set; }

        /// <summary>
        /// Label associated with the currency type
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// Type of aircraft category that applies to the currency
        /// </summary>
        public string AircraftCategory { get; set; }

        /// <summary>
        /// Type of aircraft class that applies to the currency
        /// </summary>
        public string AircraftClass { get; set; }

        /// <summary>
        /// Abbreviation for the aircraft category and class
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Indicates the type of calculation required
        /// </summary>
        public CalculationTypes CalculationType { get; set; }

        /// <summary>
        /// Indicates whether tailwheel is required
        /// </summary>
        public bool RequiresTailwheel { get; set; }

        /// <summary>
        /// Order in which the currency type should be sorted
        /// </summary>
        public int SortOrder { get; set; }
    }
}
