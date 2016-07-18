using ESP.FlightBook.Api.Models;
using System;
using System.Collections.Generic;

namespace ESP.FlightBook.Api.ViewModels
{
    public class LogbookDTO
    {
        // Logbook properties
        public int LogbookId { get; set; }
        public string Title { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ChangedOn { get; set; }
        public Pilot Pilot { get; set; }
        public ICollection<EndorsementDTO> Endorsements { get; set; }
        public ICollection<CertificateDTO> Certificates { get; set; }
        public ICollection<AircraftDTO> Aircraft { get; set; }
        public ICollection<FlightDTO> Flights { get; set; }

        /// <summary>
        /// Converts resource to a data transfer object
        /// </summary>
        /// <param name="logbook">Resource to be converted</param>
        /// <returns>Data transfer object representing the resource</returns>
        public static LogbookDTO ToDto(Logbook logbook)
        {
            return new LogbookDTO
            {
                Aircraft = new List<AircraftDTO>(),
                Certificates = new List<CertificateDTO>(),
                ChangedOn = logbook.ChangedOn,
                CreatedOn = logbook.CreatedOn,
                Endorsements = new List<EndorsementDTO>(),
                Flights = new List<FlightDTO>(),
                LogbookId = logbook.LogbookId,
                Pilot = null,
                Remarks = logbook.Remarks,
                Title = logbook.Title
            };
        }
    }
}
