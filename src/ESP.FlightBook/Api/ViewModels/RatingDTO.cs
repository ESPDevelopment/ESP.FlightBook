using ESP.FlightBook.Api.Models;
using System;

namespace ESP.FlightBook.Api.ViewModels
{
    public class RatingDTO
    {
        // Rating properties
        public int RatingId { get; set; }
        public int CertificateId { get; set; }
        public DateTime RatingDate { get; set; }
        public string RatingType { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ChangedOn { get; set; }
        public Certificate Certificate { get; set; }

        /// <summary>
        /// Converts resource to a data transfer object
        /// </summary>
        /// <param name="rating">Resource to be converted</param>
        /// <returns>Data transfer object representing the resource</returns>
        public static RatingDTO ToDto(Rating rating)
        {
            return new RatingDTO
            {
                Certificate = null,
                CertificateId = rating.CertificateId,
                ChangedOn = rating.ChangedOn,
                CreatedOn = rating.CreatedOn,
                RatingDate = rating.RatingDate,
                RatingId = rating.RatingId,
                RatingType = rating.RatingType,
                Remarks = rating.Remarks
            };
        }
    }
}