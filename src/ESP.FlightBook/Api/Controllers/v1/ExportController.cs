using ESP.FlightBook.Api.Models;
using ESP.FlightBook.Api.ViewModels;
using ESP.FlightBook.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESP.FlightBook.Api.Controllers.v1
{
    public class ExportController : LogbookApiController
    {
        protected readonly ILogger<ExportController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public ExportController(ApplicationDbContext apiDbContext, ILogger<ExportController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Exports the contents of a logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbook/{logbookId:int}/export</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/export", Name = "ExportLogbookByIdRoute")]
        public async Task<IActionResult> ExportLogbookV1(int logbookId)
        {
            // Retrieve logbook
            Logbook logbook = await _apiDbContext.Logbooks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LogbookId == logbookId);

            // Return not found result
            if (logbook == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = logbook.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Retrieve pilot information
            Pilot pilot = await _apiDbContext.Pilots
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.LogbookId == logbookId);

            // Retrieve endorsements
            List<Endorsement> endorsementList = await _apiDbContext.Endorsements
                .AsNoTracking()
                .Where(e => e.LogbookId == logbookId)
                .ToListAsync();

            // Retrieve certificates
            List<Certificate> certificateList = await _apiDbContext.Certificates
                .AsNoTracking()
                .Include(c => c.Ratings)
                .Where(c => c.LogbookId == logbookId)
                .ToListAsync();

            // Retrieve flight list
            List<Flight> flightList = await _apiDbContext.Flights
                .AsNoTracking()
                .Include(f => f.Approaches)
                .Where(f => f.LogbookId == logbookId)
                .ToListAsync();

            // Retrieve aircraft list
            List<Aircraft> aircraftList = await _apiDbContext.Aircraft
                .AsNoTracking()
                .Where(a => a.LogbookId == logbookId)
                .ToListAsync();

            // Retrieve currency list
            List<Currency> currencyList = await _apiDbContext.Currencies
                .AsNoTracking()
                .Where(c => c.LogbookId == logbookId)
                .ToListAsync();

            // Retrieve currency type list
            List<CurrencyType> currencyTypeList = await _apiDbContext.CurrencyTypes
                .AsNoTracking()
                .ToListAsync();

            // Construct view model
            ExportDTO dto = new ExportDTO();
            dto.FlightList = new List<FlightDTO>();
            dto.AircraftList = new List<AircraftDTO>();
            dto.EndorsementList = new List<EndorsementDTO>();
            dto.CertificateList = new List<CertificateDTO>();
            dto.CurrencyList = new List<CurrencyDTO>();
            dto.CurrencyTypeList = new List<CurrencyType>();

            // Add pilot information to the view model
            dto.Pilot = PilotDTO.ToDto(pilot);

            // Add endorsement information to the view model
            foreach (Endorsement endorsement in endorsementList)
            {
                dto.EndorsementList.Add(EndorsementDTO.ToDto(endorsement));
            }

            // Add certificate information to the view model
            foreach (Certificate certificate in certificateList)
            {
                CertificateDTO certificateDto = CertificateDTO.ToDto(certificate);
                foreach(Rating rating in certificate.Ratings)
                {
                    certificateDto.Ratings.Add(RatingDTO.ToDto(rating));
                }
                dto.CertificateList.Add(certificateDto);
            }

            // Add flight information to the view model
            foreach (Flight flight in flightList)
            {
                FlightDTO flightDto = FlightDTO.ToDto(flight);
                foreach(Approach approach in flight.Approaches)
                {
                    flightDto.Approaches.Add(ApproachDTO.ToDto(approach));
                }
                dto.FlightList.Add(flightDto);
            }

            // Add aircraft information to the view model
            foreach (Aircraft aircraft in aircraftList)
            {
                dto.AircraftList.Add(AircraftDTO.ToDto(aircraft));
            }

            // Add currency information to the view model
            foreach(Currency currency in currencyList)
            {
                dto.CurrencyList.Add(CurrencyDTO.ToDto(currency));
            }

            // Add currency type information to the view model
            foreach(CurrencyType currencyType in currencyTypeList)
            {
                dto.CurrencyTypeList.Add(currencyType);
            }

            // Return view model
            return Ok(dto);
        }

        /// <summary>
        /// Imports logbook data that was previously exported
        /// </summary>
        /// <param name="logbook">Logbook data to be imported</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (401) Unauthorized - If identity was not provided
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// <remarks>PUT: api/v1/logbooks/import</remarks>
        [HttpPut("api/v0/logbooks/import", Name = "ImportLogbookV0Route")]
        public async Task<IActionResult> ImportLogbookV0([FromBody] ExportDTO logbook)
        {
            // Validate request data
            if (logbook == null)
            {
                return BadRequest();
            }

            // Authorize the request
            string userId = GetUserIdClaim();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Get current date/time
            DateTime importDate = DateTime.Now;

            // Add new logbook
            Logbook newLogbook = new Logbook
            {
                ChangedOn = importDate,
                CreatedOn = importDate,
                Remarks = "Imported logbook",
                Title = string.Format("Logbook imported on {0:t}", importDate),
                UserId = userId
            };
            _apiDbContext.Logbooks.Add(newLogbook);
            try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }
            int logbookId = newLogbook.LogbookId;

            // Add new pilot
            _apiDbContext.Pilots.Add(new Pilot
            {
                AddressLine1 = logbook.Pilot.AddressLine1,
                AddressLine2 = logbook.Pilot.AddressLine2,
                CellPhoneNumber = logbook.Pilot.CellPhoneNumber,
                ChangedOn = importDate,
                City = logbook.Pilot.City,
                Country = logbook.Pilot.Country,
                CreatedOn = importDate,
                EmailAddress = logbook.Pilot.EmailAddress,
                FirstName = logbook.Pilot.FirstName,
                HomePhoneNumber = logbook.Pilot.HomePhoneNumber,
                LastName = logbook.Pilot.LastName,
                LogbookId = logbookId,
                PostalCode = logbook.Pilot.PostalCode,
                StateOrProvince = logbook.Pilot.StateOrProvince,
                UserId = userId
            });
            try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }

            // Add new endorsements
            foreach (EndorsementDTO endorsementDto in logbook.EndorsementList)
            {
                _apiDbContext.Endorsements.Add(new Endorsement
                {
                    CFIExpiration = endorsementDto.CFIExpiration,
                    CFIName = endorsementDto.CFIName,
                    CFINumber = endorsementDto.CFINumber,
                    ChangedOn = importDate,
                    CreatedOn = importDate,
                    EndorsementDate = endorsementDto.EndorsementDate,
                    LogbookId = logbookId,
                    Text = endorsementDto.Text,
                    Title = endorsementDto.Title,
                    UserId = userId
                });
            }
            try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }

            // Add new certificates
            Dictionary<int, int> certificateMap = new Dictionary<int, int>();
            foreach (CertificateDTO certificateDto in logbook.CertificateList)
            {
                Certificate certificate = new Certificate
                {
                    CertificateDate = certificateDto.CertificateDate,
                    CertificateNumber = certificateDto.CertificateNumber,
                    CertificateType = certificateDto.CertificateType,
                    ChangedOn = importDate,
                    CreatedOn = importDate,
                    ExpirationDate = certificateDto.ExpirationDate,
                    LogbookId = logbookId,
                    Remarks = certificateDto.Remarks,
                    UserId = userId
                };

                // Add certificate
                _apiDbContext.Certificates.Add(certificate);
                try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }
                certificateMap.Add(certificateDto.CertificateId, certificate.CertificateId);
            }

            // Add new ratings
            foreach (RatingDTO ratingDto in logbook.RatingList)
            {
                Rating rating = new Rating
                {
                    CertificateId = certificateMap[ratingDto.CertificateId],
                    ChangedOn = importDate,
                    CreatedOn = importDate,
                    RatingDate = ratingDto.RatingDate,
                    RatingType = ratingDto.RatingType,
                    Remarks = ratingDto.Remarks,
                    UserId = userId
                };
                _apiDbContext.Ratings.Add(rating);
            }
            try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }

            // Add new aircraft
            Dictionary<int, int> aircraftMap = new Dictionary<int, int>();
            foreach (AircraftDTO aircraftDto in logbook.AircraftList)
            {
                Aircraft newAircraft = new Aircraft
                {
                    AircraftCategory = aircraftDto.AircraftCategory,
                    AircraftClass = aircraftDto.AircraftClass,
                    AircraftIdentifier = aircraftDto.AircraftIdentifier,
                    AircraftMake = aircraftDto.AircraftMake,
                    AircraftModel = aircraftDto.AircraftModel,
                    AircraftType = aircraftDto.AircraftType,
                    AircraftYear = aircraftDto.AircraftYear,
                    ChangedOn = importDate,
                    CreatedOn = importDate,
                    EngineType = aircraftDto.EngineType,
                    GearType = aircraftDto.GearType,
                    IsComplex = aircraftDto.IsComplex,
                    IsHighPerformance = aircraftDto.IsHighPerformance,
                    IsPressurized = aircraftDto.IsPressurized,
                    LogbookId = logbookId,
                    UserId = userId
                };

                // Add aircraft
                _apiDbContext.Aircraft.Add(newAircraft);
                try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }
                aircraftMap.Add(aircraftDto.AircraftId, newAircraft.AircraftId);
            }

            // Add new flights
            Dictionary<int, int> flightMap = new Dictionary<int, int>();
            foreach (FlightDTO flightDto in logbook.FlightList)
            {
                Flight newFlight = new Flight
                {
                    AircraftId = aircraftMap[flightDto.AircraftId],
                    ChangedOn = importDate,
                    CreatedOn = importDate,
                    DepartureCode = flightDto.DepartureCode,
                    DestinationCode = flightDto.DestinationCode,
                    FlightDate = flightDto.FlightDate,
                    FlightTimeActualInstrument = flightDto.FlightTimeActualInstrument,
                    FlightTimeCrossCountry = flightDto.FlightTimeCrossCountry,
                    FlightTimeDay = flightDto.FlightTimeDay,
                    FlightTimeDual = flightDto.FlightTimeDual,
                    FlightTimeNight = flightDto.FlightTimeNight,
                    FlightTimePIC = flightDto.FlightTimePIC,
                    FlightTimeSimulatedInstrument = flightDto.FlightTimeSimulatedInstrument,
                    FlightTimeSolo = flightDto.FlightTimeSolo,
                    FlightTimeTotal = flightDto.FlightTimeTotal,
                    IsCheckRide = flightDto.IsCheckRide,
                    IsFlightReview = flightDto.IsFlightReview,
                    IsInstrumentProficiencyCheck = flightDto.IsInstrumentProficiencyCheck,
                    NumberOfHolds = flightDto.NumberOfHolds,
                    NumberOfLandingsDay = flightDto.NumberOfLandingsDay,
                    NumberOfLandingsNight = flightDto.NumberOfLandingsNight,
                    LogbookId = logbookId,
                    Remarks = flightDto.Remarks,
                    Route = flightDto.Route,
                    UserId = userId
                };

                // Add aircraft
                _apiDbContext.Flights.Add(newFlight);
                try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }
                flightMap.Add(flightDto.FlightId, newFlight.FlightId);
            }

            // Return the result
            return new NoContentResult();
        }

        /// <summary>
        /// Imports logbook data that was previously exported
        /// </summary>
        /// <param name="logbook">Logbook data to be imported</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (401) Unauthorized - If identity was not provided
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// <remarks>PUT: api/v1/logbooks/import</remarks>
        [HttpPut("api/v1/logbooks/import", Name = "ImportLogbookV1ByRoute")]
        public async Task<IActionResult> ImportLogbookV1([FromBody] ExportDTO logbook)
        {
            // Validate request data
            if (logbook == null)
            {
                return BadRequest();
            }

            // Authorize the request
            string userId = GetUserIdClaim();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Get current date/time
            DateTime importDate = DateTime.Now;

            // Add new logbook
            Logbook newLogbook = new Logbook
            {
                ChangedOn = importDate,
                CreatedOn = importDate,
                Remarks = string.Format("Logbook imported on {0:u}", importDate),
                Title = "Imported Logbook",
                UserId = userId
            };
            _apiDbContext.Logbooks.Add(newLogbook);
            try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }
            int logbookId = newLogbook.LogbookId;

            // Add new pilot
            _apiDbContext.Pilots.Add(new Pilot
            {
                AddressLine1 = logbook.Pilot.AddressLine1,
                AddressLine2 = logbook.Pilot.AddressLine2,
                CellPhoneNumber = logbook.Pilot.CellPhoneNumber,
                ChangedOn = logbook.Pilot.ChangedOn,
                City = logbook.Pilot.City,
                Country = logbook.Pilot.Country,
                CreatedOn = logbook.Pilot.CreatedOn,
                EmailAddress = logbook.Pilot.EmailAddress,
                FirstName = logbook.Pilot.FirstName,
                HomePhoneNumber = logbook.Pilot.HomePhoneNumber,
                LastName = logbook.Pilot.LastName,
                LogbookId = logbookId,
                PostalCode = logbook.Pilot.PostalCode,
                StateOrProvince = logbook.Pilot.StateOrProvince,
                UserId = userId
            });
            try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }

            // Add new endorsements
            foreach (EndorsementDTO endorsementDto in logbook.EndorsementList)
            {
                _apiDbContext.Endorsements.Add(new Endorsement
                {
                    CFIExpiration = endorsementDto.CFIExpiration,
                    CFIName = endorsementDto.CFIName,
                    CFINumber = endorsementDto.CFINumber,
                    ChangedOn = endorsementDto.ChangedOn,
                    CreatedOn = endorsementDto.CreatedOn,
                    EndorsementDate = endorsementDto.EndorsementDate,
                    LogbookId = logbookId,
                    Text = endorsementDto.Text,
                    Title = endorsementDto.Title,
                    UserId = userId
                });
            }
            try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }

            // Add new certificates
            foreach (CertificateDTO certificateDto in logbook.CertificateList)
            {
                Certificate newCertificate = new Certificate
                {
                    CertificateDate = certificateDto.CertificateDate,
                    CertificateNumber = certificateDto.CertificateNumber,
                    CertificateType = certificateDto.CertificateType,
                    ChangedOn = certificateDto.ChangedOn,
                    CreatedOn = certificateDto.CreatedOn,
                    ExpirationDate = certificateDto.ExpirationDate,
                    LogbookId = logbookId,
                    Ratings = new List<Rating>(),
                    Remarks = certificateDto.Remarks,
                    UserId = userId
                };

                foreach(RatingDTO ratingDto in certificateDto.Ratings)
                {
                    Rating newRating = new Rating
                    {
                        ChangedOn = ratingDto.ChangedOn,
                        CreatedOn = ratingDto.CreatedOn,
                        RatingDate = ratingDto.RatingDate,
                        RatingType = ratingDto.RatingType,
                        Remarks = ratingDto.Remarks,
                        UserId = userId
                    };
                    newCertificate.Ratings.Add(newRating);
                }

                // Add certificate
                _apiDbContext.Certificates.Add(newCertificate);
                try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }
            }

            // Add new aircraft
            Dictionary<int, int> aircraftMap = new Dictionary<int, int>();
            foreach (AircraftDTO aircraftDto in logbook.AircraftList)
            {
                Aircraft newAircraft = new Aircraft
                {
                    AircraftCategory = aircraftDto.AircraftCategory,
                    AircraftClass = aircraftDto.AircraftClass,
                    AircraftIdentifier = aircraftDto.AircraftIdentifier,
                    AircraftMake = aircraftDto.AircraftMake,
                    AircraftModel = aircraftDto.AircraftModel,
                    AircraftType = aircraftDto.AircraftType,
                    AircraftYear = aircraftDto.AircraftYear,
                    ChangedOn = aircraftDto.ChangedOn,
                    CreatedOn = aircraftDto.CreatedOn,
                    EngineType = aircraftDto.EngineType,
                    GearType = aircraftDto.GearType,
                    IsComplex = aircraftDto.IsComplex,
                    IsHighPerformance = aircraftDto.IsHighPerformance,
                    IsPressurized = aircraftDto.IsPressurized,
                    LogbookId = logbookId,
                    UserId = userId
                };

                // Add aircraft
                _apiDbContext.Aircraft.Add(newAircraft);
                try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }
                aircraftMap.Add(aircraftDto.AircraftId, newAircraft.AircraftId);
            }

            // Add new flights
            foreach (FlightDTO flightDto in logbook.FlightList)
            {
                Flight newFlight = new Flight
                {
                    AircraftId = aircraftMap[flightDto.AircraftId],
                    Approaches = new List<Approach>(),
                    ChangedOn = flightDto.ChangedOn,
                    CreatedOn = flightDto.CreatedOn,
                    DepartureCode = flightDto.DepartureCode,
                    DestinationCode = flightDto.DestinationCode,
                    FlightDate = flightDto.FlightDate,
                    FlightTimeActualInstrument = flightDto.FlightTimeActualInstrument,
                    FlightTimeCrossCountry = flightDto.FlightTimeCrossCountry,
                    FlightTimeDay = flightDto.FlightTimeDay,
                    FlightTimeDual = flightDto.FlightTimeDual,
                    FlightTimeNight = flightDto.FlightTimeNight,
                    FlightTimePIC = flightDto.FlightTimePIC,
                    FlightTimeSimulatedInstrument = flightDto.FlightTimeSimulatedInstrument,
                    FlightTimeSolo = flightDto.FlightTimeSolo,
                    FlightTimeTotal = flightDto.FlightTimeTotal,
                    IsCheckRide = flightDto.IsCheckRide,
                    IsFlightReview = flightDto.IsFlightReview,
                    IsInstrumentProficiencyCheck = flightDto.IsInstrumentProficiencyCheck,
                    NumberOfHolds = flightDto.NumberOfHolds,
                    NumberOfLandingsDay = flightDto.NumberOfLandingsDay,
                    NumberOfLandingsNight = flightDto.NumberOfLandingsNight,
                    LogbookId = logbookId,
                    Remarks = flightDto.Remarks,
                    Route = flightDto.Route,
                    UserId = userId
                };

                foreach(ApproachDTO approachDto in flightDto.Approaches)
                {
                    Approach newApproach = new Approach
                    {
                        AirportCode = approachDto.AirportCode,
                        ApproachType = approachDto.ApproachType,
                        ChangedOn = approachDto.ChangedOn,
                        CreatedOn = approachDto.CreatedOn,
                        IsCircleToLand = approachDto.IsCircleToLand,
                        Remarks = approachDto.Remarks,
                        Runway = approachDto.Runway,
                        UserId = userId
                    };

                    newFlight.Approaches.Add(newApproach);
                }

                // Add aircraft
                _apiDbContext.Flights.Add(newFlight);
                try { await _apiDbContext.SaveChangesAsync(); } catch { return new StatusCodeResult(409); }
            }

            // Map previous currency types
            Dictionary<int, int> currencyTypeMap = new Dictionary<int, int>();
            foreach(CurrencyType currencyType in logbook.CurrencyTypeList)
            {
                CurrencyType existingCurrencyType = await _apiDbContext.CurrencyTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Label == currencyType.Label);
                currencyTypeMap.Add(currencyType.CurrencyTypeId, existingCurrencyType.CurrencyTypeId);
            }

            // Add new currencies
            foreach (CurrencyDTO currencyDto in logbook.CurrencyList)
            {
                Currency newCurrency = new Currency
                {
                    ChangedOn = currencyDto.ChangedOn,
                    CreatedOn = currencyDto.CreatedOn,
                    CurrencyTypeId = currencyTypeMap[currencyDto.CurrencyTypeId],
                    DaysRemaining = currencyDto.DaysRemaining,
                    IsCurrent = currencyDto.IsCurrent,
                    IsNightCurrency = currencyDto.IsNightCurrency,
                    LogbookId = logbookId,
                    UserId = userId
                };

                // Add currency
                _apiDbContext.Currencies.Add(newCurrency);
                try
                {
                    await _apiDbContext.SaveChangesAsync();
                } catch (Exception e)
                {
                    _logger.LogWarning("[ImportLogbookV1]", e);
                    return new StatusCodeResult(409);
                }
            }

            // Return the result
            return new NoContentResult();
        }
    }
}
