using ESP.FlightBook.Api.Models;
using System;

namespace ESP.FlightBook.Api.ViewModels
{
    public class CurrencyDTO
    {
        // Currency properties
        public int CurrencyId { get; set; }
        public int LogbookId { get; set; }
        public int CurrencyTypeId { get; set; }
        public bool IsNightCurrency { get; set; }
        public bool IsCurrent { get; set; }
        public int DaysRemaining { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ChangedOn { get; set; }
        public LogbookDTO Logbook { get; set; }
        public CurrencyType CurrencyType { get; set; }

        /// <summary>
        /// Converts resource to a data transfer object
        /// </summary>
        /// <param name="currency">Resource to be converted</param>
        /// <returns>Data transfer object representing the resource</returns>
        public static CurrencyDTO ToDto(Currency currency)
        {
            return new CurrencyDTO
            {
                ChangedOn = currency.ChangedOn,
                CreatedOn = currency.CreatedOn,
                CurrencyId = currency.CurrencyId,
                CurrencyType = currency.CurrencyType,
                CurrencyTypeId = currency.CurrencyTypeId,
                DaysRemaining = currency.DaysRemaining,
                ExpirationDate = null,
                IsCurrent = currency.IsCurrent,
                IsNightCurrency = currency.IsNightCurrency,
                Logbook = null,
                LogbookId = currency.LogbookId
            };
        }
    }
}
