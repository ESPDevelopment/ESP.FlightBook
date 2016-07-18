using System;
using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Api.Models
{
    /// <summary>
    /// Represents a pilot's currency
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Unique identifier of the currency
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Unique identifier of the logbook
        /// </summary>
        public int LogbookId { get; set; }

        /// <summary>
        /// Unique identifier of the currency type
        /// </summary>
        public int CurrencyTypeId { get; set; }

        /// <summary>
        /// The unique identifier of the registered user
        /// </summary>
        [StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// Indicates whether currency is for night currency
        /// </summary>
        public bool IsNightCurrency { get; set; }

        /// <summary>
        /// Indicates whether currency has been satisfied
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Indicates the number of days currency remaining
        /// </summary>
        public int DaysRemaining { get; set; }

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
        /// Navigation property for the related CurrencyType entity
        /// </summary>
        public CurrencyType CurrencyType { get; set; }
    }
}