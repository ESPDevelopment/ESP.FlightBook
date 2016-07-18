using ESP.FlightBook.Api.Models;
using System;

namespace ESP.FlightBook.Api.ViewModels
{
    public class AircraftDTO
    {
        // Aircraft properties
        public int AircraftId { get; set; }
        public int LogbookId { get; set; }
        public string AircraftIdentifier { get; set; }
        public string AircraftType { get; set; }
        public string AircraftCategory { get; set; }
        public string AircraftClass { get; set; }
        public string AircraftMake { get; set; }
        public string AircraftModel { get; set; }
        public int AircraftYear { get; set; }
        public string GearType { get; set; }
        public string EngineType { get; set; }
        public bool IsComplex { get; set; }
        public bool IsHighPerformance { get; set; }
        public bool IsPressurized { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ChangedOn { get; set; }
        public LogbookDTO Logbook { get; set; }

        /// <summary>
        /// Converts resource to a data transfer object
        /// </summary>
        /// <param name="aircraft">Resource to be converted</param>
        /// <returns>Data transfer object representing the resource</returns>
        public static AircraftDTO ToDto(Aircraft aircraft)
        {
            return new AircraftDTO
            {
                AircraftCategory = aircraft.AircraftCategory,
                AircraftClass = aircraft.AircraftClass,
                AircraftId = aircraft.AircraftId,
                AircraftIdentifier = aircraft.AircraftIdentifier,
                AircraftMake = aircraft.AircraftMake,
                AircraftModel = aircraft.AircraftModel,
                AircraftType = aircraft.AircraftType,
                AircraftYear = aircraft.AircraftYear,
                ChangedOn = aircraft.ChangedOn,
                CreatedOn = aircraft.CreatedOn,
                EngineType = aircraft.EngineType,
                GearType = aircraft.GearType,
                IsComplex = aircraft.IsComplex,
                IsHighPerformance = aircraft.IsHighPerformance,
                IsPressurized = aircraft.IsPressurized,
                LogbookId = aircraft.LogbookId
            };
        }
    }
}
