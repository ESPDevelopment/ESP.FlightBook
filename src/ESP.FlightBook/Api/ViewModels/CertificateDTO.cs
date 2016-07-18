using ESP.FlightBook.Api.Models;
using System;
using System.Collections.Generic;

namespace ESP.FlightBook.Api.ViewModels
{
    public class CertificateDTO
    {
        // Certificate properties
        public int CertificateId { get; set; }
        public int LogbookId { get; set; }
        public DateTime CertificateDate { get; set; }
        public string CertificateType { get; set; }
        public string CertificateNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ChangedOn { get; set; }
        public Logbook Logbook { get; set; }
        public ICollection<RatingDTO> Ratings { get; set; }

        /// <summary>
        /// Converts resource to a data transfer object
        /// </summary>
        /// <param name="certificate">Resource to be converted</param>
        /// <returns>Data transfer object representing the resource</returns>
        public static CertificateDTO ToDto(Certificate certificate)
        {
            return new CertificateDTO
            {
                CertificateDate = certificate.CertificateDate,
                CertificateId = certificate.CertificateId,
                CertificateNumber = certificate.CertificateNumber,
                CertificateType = certificate.CertificateType,
                ChangedOn = certificate.ChangedOn,
                CreatedOn = certificate.CreatedOn,
                ExpirationDate = certificate.ExpirationDate,
                LogbookId = certificate.LogbookId,
                Ratings = new List<RatingDTO>(),
                Remarks = certificate.Remarks
            };
        }
    }
}
