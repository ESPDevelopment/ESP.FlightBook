using ESP.FlightBook.Api.Models;
using System.Collections.Generic;

namespace ESP.FlightBook.Api.ViewModels
{
    public class ExportDTO
    {
        public PilotDTO Pilot { get; set; }
        public List<AircraftDTO> AircraftList { get; set; }
        public List<CertificateDTO> CertificateList { get; set; }
        public List<EndorsementDTO> EndorsementList { get; set; }
        public List<FlightDTO> FlightList { get; set; }
        public List<RatingDTO> RatingList { get; set; }
        public List<CurrencyDTO> CurrencyList { get; set; }
        public List<CurrencyType> CurrencyTypeList { get; set; }
    }
}
