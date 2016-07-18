using ESP.FlightBook.Api.Models;
using System;

namespace ESP.FlightBook.Api.ViewModels
{
    public class ApproachDTO
    {
        // Approach properties
        public int ApproachId { get; set; }
        public int FlightId { get; set; }
        public string AirportCode { get; set; }
        public string ApproachType { get; set; }
        public string Runway { get; set; }
        public bool IsCircleToLand { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ChangedOn { get; set; }
        public Flight Flight { get; set; }

        /// <summary>
        /// Converts resource to a data transfer object
        /// </summary>
        /// <param name="approach">Resource to be converted</param>
        /// <returns>Data transfer object representing the resource</returns>
        public static ApproachDTO ToDto(Approach approach)
        {
            return new ApproachDTO
            {
                AirportCode = approach.AirportCode,
                ApproachId = approach.ApproachId,
                ApproachType = approach.ApproachType,
                ChangedOn = approach.ChangedOn,
                CreatedOn = approach.CreatedOn,
                Flight = null,
                FlightId = approach.FlightId,
                IsCircleToLand = approach.IsCircleToLand,
                Remarks = approach.Remarks,
                Runway = approach.Runway
            };
        }
    }
}
