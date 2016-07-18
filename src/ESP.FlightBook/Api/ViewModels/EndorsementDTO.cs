using ESP.FlightBook.Api.Models;
using System;

namespace ESP.FlightBook.Api.ViewModels
{
    public class EndorsementDTO
    {
        // Endorsement list
        public int EndorsementId { get; set; }
        public int LogbookId { get; set; }
        public DateTime EndorsementDate { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string CFIName { get; set; }
        public string CFINumber { get; set; }
        public string CFIExpiration { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ChangedOn { get; set; }
        public Logbook Logbook { get; set; }

        /// <summary>
        /// Converts resource to a data transfer object
        /// </summary>
        /// <param name="endorsement">Resource to be converted</param>
        /// <returns>Data transfer object representing the resource</returns>
        public static EndorsementDTO ToDto(Endorsement endorsement)
        {
            return new EndorsementDTO
            {
                CFIExpiration = endorsement.CFIExpiration,
                CFIName = endorsement.CFIName,
                CFINumber = endorsement.CFINumber,
                ChangedOn = endorsement.ChangedOn,
                CreatedOn = endorsement.CreatedOn,
                EndorsementDate = endorsement.EndorsementDate,
                EndorsementId = endorsement.EndorsementId,
                Logbook = null,
                LogbookId = endorsement.LogbookId,
                Text = endorsement.Text,
                Title = endorsement.Title
            };
        }
    }
}
