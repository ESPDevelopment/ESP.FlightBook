using System.Collections.Generic;

namespace ESP.FlightBook.Api.ViewModels
{
    public class AircraftSummaryViewModel
    {
        public List<AircraftSummary> Aircraft { get; set; }
    }

    public class AircraftSummary
    {
        public AircraftDTO Aircraft { get; set; }
        public decimal HoursTotal { get; set; }
        public decimal HoursPIC { get; set; }
        public decimal HoursDual { get; set; }
        public decimal HoursSolo { get; set; }
        public decimal HoursXC { get; set; }
        public decimal HoursDay { get; set; }
        public decimal HoursNight { get; set; }
        public decimal HoursActualInstrument { get; set; }
        public decimal HoursSimulatedInstrument { get; set; }
        public int LandingsTotal { get; set; }
        public int LandingsDay { get; set; }
        public int LandingsNight { get; set; }
        public int Approaches { get; set; }
        public int Holds { get; set; }
    }
}
