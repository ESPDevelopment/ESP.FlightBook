using ESP.FlightBook.Api.Models;
using System;
using System.Collections.Generic;

namespace ESP.FlightBook.Tests.Api.DataAccess
{
    public class MockDataFactory
    {
        /// <summary>
        /// Returns an Aircraft resource with test data
        /// </summary>
        /// <param name="userId">Unique registered user identifier</param>
        /// <param name="aircraftId">Unique airicraft identifier</param>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="logbook">Related Logbook resource</param>
        /// <returns>An aircraft resource</returns>
        public static Aircraft CreateMockAircraft(string userId, int aircraftId = 1, int logbookId = 1)
        {
            DateTime now = DateTime.Now;

            Aircraft aircraft = new Aircraft
            {
                AircraftCategory = string.Format("AircraftCategory_{0}", aircraftId),
                AircraftClass = string.Format("AircraftClass_{0}", aircraftId),
                AircraftId = aircraftId,
                AircraftIdentifier = string.Format("N00{0}", aircraftId),
                AircraftMake = string.Format("AircraftMake_{0}", aircraftId),
                AircraftModel = string.Format("AircraftModel_{0}", aircraftId),
                AircraftType = string.Format("AircraftType_{0}", aircraftId),
                AircraftYear = 1980 + aircraftId,
                ChangedOn = now,
                CreatedOn = now,
                EngineType = string.Format("EngineType_{0}", aircraftId),
                Flights = null,
                GearType = string.Format("GearType_{0}", aircraftId),
                IsComplex = false,
                IsHighPerformance = false,
                IsPressurized = false,
                Logbook = null,
                LogbookId = logbookId,
                UserId = userId
            };

            return aircraft;
        }

        /// <summary>
        /// Returns an Approach resource with test data
        /// </summary>
        /// <param name="userId">Unique registered user identifier</param>
        /// <param name="approachId">Unique approach identifier</param>
        /// <param name="flightId">Unique flight identifier</param>
        /// <param name="flight">Related Flight resource</param>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="logbook">Related Logbook resource</param>
        /// <returns>An approach resource</returns>
        public static Approach CreateMockApproach(string userId, int approachId = 1, int flightId = 1, int logbookId = 1)
        {
            DateTime now = DateTime.Now;

            Approach approach = new Approach
            {
                AirportCode = string.Format("AirportCode_{0}", approachId),
                ApproachId = approachId,
                ApproachType = string.Format("ApproachType_{0}", approachId),
                ChangedOn = now,
                CreatedOn = now,
                Flight = null,
                FlightId = flightId,
                IsCircleToLand = false,
                Remarks = string.Format("Remarks_{0}", approachId),
                Runway = string.Format("Runway_{0}", approachId),
                UserId = userId
            };

            return approach;
        }

        /// <summary>
        /// Returns a Certificate resource with test data
        /// </summary>
        /// <param name="userId">Unique registered user identifier</param>
        /// <param name="certificateId">Unique certificate identifier</param>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="logbook">Related Logbook resource</param>
        /// <param name="ratings">Related Ratings resources</param>
        /// <returns>A Certificate resource</returns>
        public static Certificate CreateMockCertificate(string userId, int certificateId = 1, int logbookId = 1, List<Rating> ratings = null)
        {
            DateTime now = DateTime.Now;

            Certificate certificate = new Certificate
            {
                CertificateDate = new DateTime(1980 + certificateId, 1, 1),
                CertificateId = certificateId,
                CertificateNumber = string.Format("CertificateNumber_{0}", certificateId),
                CertificateType = string.Format("CertificateType_{0}", certificateId),
                ChangedOn = now,
                CreatedOn = now,
                ExpirationDate = new DateTime(1990 + certificateId, 1, 1),
                Logbook = null,
                LogbookId = logbookId,
                Ratings = ratings,
                Remarks = string.Format("Remarks_{0}", certificateId),
                UserId = userId
            };

            return certificate;
        }

        /// <summary>
        /// Returns an Endorsement resource with test data
        /// </summary>
        /// <param name="userId">Unique registered user identifier</param>
        /// <param name="endorsementId">Unique endorsement identifier</param>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="logbook">Related Logbook resource</param>
        /// <returns>An Endorsement resource</returns>
        public static Endorsement CreateMockEndorsement(string userId, int endorsementId = 1, int logbookId = 1)
        {
            DateTime now = DateTime.Now;

            Endorsement endorsement = new Endorsement
            {
                CFIExpiration = string.Format("CFIExpiration_{0}", endorsementId),
                CFIName = string.Format("CFIName_{0}", endorsementId),
                CFINumber = string.Format("CFINumber_{0}", endorsementId),
                ChangedOn = now,
                CreatedOn = now,
                EndorsementDate = new DateTime(1990 + endorsementId),
                EndorsementId = endorsementId,
                Logbook = null,
                LogbookId = logbookId,
                Text = string.Format("Text_{0}", endorsementId),
                Title = string.Format("Title_{0}", endorsementId),
                UserId = userId
            };

            return endorsement;
        }

        /// <summary>
        /// Returns a Flight resource with test data
        /// </summary>
        /// <param name="userId">Unique registered user identifier</param>
        /// <param name="flightId">Unique flight identifier</param>
        /// <param name="aircraftId">Unique aircraft identifier</param>
        /// <param name="aircraft">Related Aircraft resource</param>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="logbook">Related Logbook resource</param>
        /// <returns>An aircraft resource</returns>
        public static Flight CreateMockFlight(string userId, int flightId = 1, int aircraftId = 1, int logbookId = 1, List<Approach> approaches = null)
        {
            DateTime now = DateTime.Now;

            Flight flight = new Flight
            {
                Aircraft = null,
                AircraftId = aircraftId,
                Approaches = approaches,
                ChangedOn = now,
                CreatedOn = now,
                DepartureCode = string.Format("DepartureCode_{0}", flightId),
                DestinationCode = string.Format("DestinationCode_{0}", flightId),
                FlightDate = new DateTime(2000 + flightId, 1, 1),
                FlightId = flightId,
                FlightTimeActualInstrument = 0.0M,
                FlightTimeCrossCountry = 0.0M,
                FlightTimeDay = 0.0M,
                FlightTimeDual = 0.0M,
                FlightTimeNight = 0.0M,
                FlightTimePIC = 0.0M,
                FlightTimeSimulatedInstrument = 0.0M,
                FlightTimeSolo = 0.0M,
                FlightTimeTotal = 0.0M,
                IsCheckRide = false,
                IsFlightReview = false,
                IsInstrumentProficiencyCheck = false,
                LogbookId = logbookId,
                NumberOfHolds = 0,
                NumberOfLandingsDay = 1,
                NumberOfLandingsNight = 1,
                Remarks = string.Format("Remarks_{0}", flightId),
                Route = string.Format("Route_{0}", flightId),
                UserId = userId
            };

            return flight;
        }

        /// <summary>
        /// Returns a Logbook resource with test data
        /// </summary>
        /// <param name="userId">Unique registered user identifier</param>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>A logbook resource</returns>
        public static Logbook CreateMockLogbook(string userId, int logbookId = 1)
        {
            DateTime now = DateTime.Now;

            Logbook logbook = new Logbook()
            {
                Aircraft = null,
                Certificates = null,
                ChangedOn = now,
                CreatedOn = now,
                Currencies = null,
                Endorsements = null,
                Flights = null,
                LogbookId = logbookId,
                Pilot = null,
                Title = string.Format("Title_{0}", logbookId),
                UserId = userId
            };
            return logbook;
        }

        /// <summary>
        /// Returns a Pilot resource with test data
        /// </summary>
        /// <param name="userId">Unique registered user identifier</param>
        /// <param name="pilotId">Unique pilot identifier</param>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="logbook">Related logbook resource</param>
        /// <returns>A pilot resource</returns>
        public static Pilot CreateMockPilot(string userId, int pilotId = 1, int logbookId = 1)
        {
            DateTime now = DateTime.Now;

            Pilot pilot = new Pilot
            {
                AddressLine1 = string.Format("AddressLine1_{0}", pilotId),
                AddressLine2 = string.Format("AddressLine2_{0}", pilotId),
                CellPhoneNumber = string.Format("{0} {1}", "425-555-1212", pilotId),
                ChangedOn = now,
                City = string.Format("City_{0}", pilotId),
                Country = string.Format("Country_{0}", pilotId),
                CreatedOn = now,
                EmailAddress = string.Format("EmailAddress_{0}", pilotId),
                FirstName = string.Format("FirstName_{0}", pilotId),
                HomePhoneNumber = string.Format("{0} {1}", "425-555-2121", pilotId),
                LastName = string.Format("LastName_{0}", pilotId),
                Logbook = null,
                LogbookId = logbookId,
                PilotId = pilotId,
                PostalCode = string.Format("PostalCode_{0}", pilotId),
                StateOrProvince = string.Format("StateOrProvince_{0}", pilotId),
                UserId = userId
            };

            return pilot;
        }

        /// <summary>
        /// Returns a Rating resource with test data
        /// </summary>
        /// <param name="userId">Unique registered user identifier</param>
        /// <param name="ratingId">Unique rating identifier</param>
        /// <param name="certificateId">Unique certificate identifier</param>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="certificate">Related certificate resource</param>
        /// <param name="logbook">Related logbook resource</param>
        /// <returns>A Rating resource</returns>
        public static Rating CreateMockRating(string userId, int ratingId = 1, int certificateId = 1, int logbookId = 1)
        {
            DateTime now = DateTime.Now;

            Rating rating = new Rating
            {
                Certificate = null,
                CertificateId = certificateId,
                ChangedOn = now,
                CreatedOn = now,
                RatingDate = new DateTime(1980 + ratingId, 1, 1),
                RatingId = ratingId,
                RatingType = string.Format("RatingType_{0}", ratingId),
                Remarks = string.Format("Remarks_{0}", ratingId),
                UserId = userId
            };

            return rating;
        }
    }
}
