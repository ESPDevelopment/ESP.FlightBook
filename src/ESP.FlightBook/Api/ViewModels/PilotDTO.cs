using ESP.FlightBook.Api.Models;
using System;

namespace ESP.FlightBook.Api.ViewModels
{
    public class PilotDTO
    {
        // Pilot properties
        public int PilotId { get; set; }
        public int LogbookId { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string HomePhoneNumber { get; set; }
        public string CellPhoneNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ChangedOn { get; set; }
        public Logbook Logbook { get; set; }

        /// <summary>
        /// Converts resource to a data transfer object
        /// </summary>
        /// <param name="pilot">Resource to be converted</param>
        /// <returns>Data transfer object representing the resource</returns>
        public static PilotDTO ToDto(Pilot pilot)
        {
            return new PilotDTO
            {
                AddressLine1 = pilot.AddressLine1,
                AddressLine2 = pilot.AddressLine2,
                CellPhoneNumber = pilot.CellPhoneNumber,
                ChangedOn = pilot.ChangedOn,
                City = pilot.City,
                Country = pilot.Country,
                CreatedOn = pilot.CreatedOn,
                EmailAddress = pilot.EmailAddress,
                FirstName = pilot.FirstName,
                HomePhoneNumber = pilot.HomePhoneNumber,
                LastName = pilot.LastName,
                Logbook = null,
                LogbookId = pilot.LogbookId,
                PilotId = pilot.PilotId,
                PostalCode = pilot.PostalCode,
                StateOrProvince = pilot.StateOrProvince
            };
        }
    }
}
