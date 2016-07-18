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
    public class CurrenciesController : LogbookApiController
    {
        protected readonly ILogger<CurrenciesController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public CurrenciesController(ApplicationDbContext apiDbContext, ILogger<CurrenciesController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Calculate flight review currency
        /// </summary>
        /// <param name="currencyDto">Data transfer object to be return</param>
        /// <param name="flightList">List of flights for the pilot</param>
        private void CalcFlightReviewCurrency(CurrencyDTO currencyDto, List<Flight> flightList)
        {
            // Find most recent flight review
            DateTime lastFlightReview = DateTime.MinValue;
            foreach (Flight flight in flightList)
            {
                // Include only flight reviews
                if (flight.IsFlightReview == true)
                {
                    if (flight.FlightDate > lastFlightReview)
                    {
                        lastFlightReview = flight.FlightDate;
                    }
                }
            }

            // Check for missing flight review
            if (lastFlightReview == DateTime.MinValue)
            {
                currencyDto.IsCurrent = false;
                currencyDto.DaysRemaining = 0;
                currencyDto.ExpirationDate = null;
                return;
            }

            // Calculate currency and days remaining
            DateTime today = DateTime.Now;
            DateTime expirationDate = lastFlightReview.AddYears(2);
            DateTime expirationDay = new DateTime(expirationDate.Year, expirationDate.Month,
                DateTime.DaysInMonth(expirationDate.Year, expirationDate.Month));
            currencyDto.ExpirationDate = expirationDay;
            if (expirationDay > today)
            {
                currencyDto.IsCurrent = true;
                currencyDto.DaysRemaining = (expirationDate - DateTime.Now).Days;
            }
        }

        /// <summary>
        /// Calculate general flight currency
        /// </summary>
        /// <param name="currencyDto">Data transfer object to be return</param>
        /// <param name="flightList">List of flights for the pilot</param>
        private void CalcGeneralCurrency(CurrencyDTO currencyDto, List<Flight> flightList)
        {
            int landings = 0;

            // Initialize currency
            currencyDto.IsCurrent = false;
            currencyDto.DaysRemaining = 0;
            currencyDto.ExpirationDate = null;

            // Iterate through flights
            foreach (Flight flight in flightList)
            {
                // Validate aircraft category and class
                if (flight.Aircraft.AircraftCategory == currencyDto.CurrencyType.AircraftCategory &&
                    flight.Aircraft.AircraftClass == currencyDto.CurrencyType.AircraftClass)
                {
                    // Check tailwheel requirement
                    bool isTailwheel = flight.Aircraft.GearType.Contains("Tailwheel");
                    bool requiresTailwheel = currencyDto.CurrencyType.RequiresTailwheel;
                    if (requiresTailwheel == false || (requiresTailwheel && isTailwheel))
                    {
                        // Increment landings
                        landings += flight.NumberOfLandingsNight;
                        if (currencyDto.IsNightCurrency == false)
                        {
                            landings += flight.NumberOfLandingsDay;
                        }

                        // Calculate currency
                        if (landings >= 3)
                        {
                            DateTime today = DateTime.Now;
                            DateTime expirationDay = flight.FlightDate.AddDays(90);
                            currencyDto.ExpirationDate = expirationDay;
                            if (expirationDay > today)
                            {
                                currencyDto.IsCurrent = true;
                                currencyDto.DaysRemaining = (expirationDay - DateTime.Now).Days;
                            }
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculate IFR currency
        /// </summary>
        /// <param name="currencyDto">Data transfer object to be return</param>
        /// <param name="flightList">List of flights for the pilot</param>
        private void CalcIFRCurrency(CurrencyDTO currencyDto, List<Flight> flightList)
        {
            int approaches = 0;
            int holds = 0;

            // Initialize currency
            currencyDto.IsCurrent = false;
            currencyDto.DaysRemaining = 0;
            currencyDto.ExpirationDate = null;

            // Iterate through flights
            foreach (Flight flight in flightList)
            {
                // Validate aircraft category
                if (flight.Aircraft.AircraftCategory == currencyDto.CurrencyType.AircraftCategory)
                {
                    // Increment approaches
                    approaches += (flight.Approaches == null) ? 0 : flight.Approaches.Count;
                    holds += flight.NumberOfHolds;

                    // Check currency
                    if ((approaches >= 6 && holds >= 1) || flight.IsInstrumentProficiencyCheck)
                    {
                        DateTime today = DateTime.Now;
                        DateTime expirationDay = flight.FlightDate.AddMonths(6);
                        currencyDto.ExpirationDate = expirationDay;
                        if (expirationDay > today)
                        {
                            currencyDto.IsCurrent = true;
                            currencyDto.DaysRemaining = (expirationDay - DateTime.Now).Days;
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the Currency resources for the specified logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested resource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/currencies</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/currencies", Name = "GetCurrenciesRoute")]
        public async Task<IActionResult> GetCurrencies(int logbookId)
        {
            // Find associated logbook
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

            // Find currency for the specified pilot
            List<Currency> currencyList = await _apiDbContext.Currencies
                .Where(c => c.LogbookId == logbookId)
                .Include(c => c.CurrencyType)
                .ToListAsync();

            // Retrieve list of flights
            List<Flight> flightList = await _apiDbContext.Flights
                .Where(f => f.LogbookId == logbookId)
                .Include(f => f.Aircraft)
                .OrderByDescending(f => f.FlightDate)
                .ToListAsync();

            // Retrieve list of endorsements
            List<Endorsement> endorsementList = await _apiDbContext.Endorsements
                .Where(e => e.LogbookId == logbookId)
                .ToListAsync();

            // Format the output
            List<CurrencyDTO> currencyDtos = new List<CurrencyDTO>();
            foreach (Currency currency in currencyList)
            {
                CurrencyDTO currencyDto = CurrencyDTO.ToDto(currency);
                currencyDtos.Add(currencyDto);

                // Calculate currency
                switch (currency.CurrencyType.CalculationType)
                {
                    case CurrencyType.CalculationTypes.FlightReview:
                        CalcFlightReviewCurrency(currencyDto, flightList);
                        break;
                    case CurrencyType.CalculationTypes.General:
                        CalcGeneralCurrency(currencyDto, flightList);
                        break;
                    case CurrencyType.CalculationTypes.IFR:
                        CalcIFRCurrency(currencyDto, flightList);
                        break;
                }
            }

            // Return the result
            return Ok(currencyDtos);
        }

        /// <summary>
        /// Returns the specified Currency resource for the specified logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="currencyId">Unique currency identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Unauthorized - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/currencies/{currencyId:int}</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/currencies/{currencyId:int}", Name = "GetCurrencyByIdRoute")]
        public async Task<IActionResult> GetCurrency(int logbookId, int currencyId)
        {
            // Find specified resource
            Currency currency = await _apiDbContext.Currencies
                .AsNoTracking()
                .Include(c => c.CurrencyType)
                .FirstOrDefaultAsync(c => c.CurrencyId == currencyId && c.LogbookId == logbookId);

            // Return not found result
            if (currency == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = currency.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Convert to DTO
            CurrencyDTO dto = CurrencyDTO.ToDto(currency);

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Currency resource for the specified logbook
        /// </summary>
        /// <param name="logbookId">The unique pilot identifier</param>
        /// <param name="currency">The currency to be created</param>
        /// <returns>
        /// (201) Created - If the resource was successfully created
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (409) Conflict - If the resource could not be created
        /// <remarks>POST: api/Pilots/{pilotId}/Currencies</remarks>
        [HttpPost("api/v1/logbooks/{logbookId:int}/currencies", Name = "PostCurrencyRoute")]
        public async Task<IActionResult> PostCurrency(int logbookId, [FromBody] Currency currency)
        {
            // Validate request data
            if (currency == null)
            {
                return BadRequest();
            }

            // Validate the model
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values)
                {
                    foreach (var message in error.Errors)
                    {
                        _logger.LogInformation("[PostCurrency] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find associated logbook
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

            // Set non-editable attributes
            currency.LogbookId = logbookId;
            currency.UserId = GetUserIdClaim();
            currency.CurrencyType = null;
            currency.CreatedOn = DateTime.UtcNow;
            currency.ChangedOn = currency.CreatedOn;

            // Attempt to add new resource
            _apiDbContext.Currencies.Add(currency);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PostCurrency]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            CurrencyDTO dto = CurrencyDTO.ToDto(currency);

            // Return the result
            return CreatedAtRoute("GetCurrencyByIdRoute",
                new
                {
                    logbookId = dto.LogbookId,
                    currencyId = dto.CurrencyId
                }, dto);
        }

        /// <summary>
        /// Updates an existing Currency resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="currencyId">Unique currency identifier</param>
        /// <param name="currency">Endorsement to be udpated</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>PUT api/v1/logbooks/{logbookId:int}/currencies/{currencyId:int}</remarks>
        [HttpPut("api/v1/logbooks/{logbookId:int}/currencies/{currencyId:int}", Name = "PutCurrencyRoute")]
        public async Task<IActionResult> PutCurrency(int logbookId, int currencyId, [FromBody] Currency currency)
        {
            // Validate request data
            if (currency == null)
            {
                return BadRequest();
            }

            // Validate the model
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values)
                {
                    foreach (var message in error.Errors)
                    {
                        _logger.LogInformation("[PutCurrency] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find specified resource
            Currency existingCurrency = await _apiDbContext.Currencies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CurrencyId == currencyId && c.LogbookId == logbookId);

            // Return not found result
            if (existingCurrency == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingCurrency.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Set non-editable attributes
            currency.CurrencyId = existingCurrency.CurrencyId;
            currency.LogbookId = existingCurrency.LogbookId;
            currency.UserId = existingCurrency.UserId;
            currency.CreatedOn = existingCurrency.CreatedOn;
            currency.ChangedOn = DateTime.UtcNow;

            // Attempt to update resource
            _apiDbContext.Entry(currency).State = EntityState.Modified;
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PutCurrency]", e);
                return new StatusCodeResult(409);
            }

            // Return the result
            return new NoContentResult();
        }

        /// <summary>
        /// Deletes an existing Currency resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="currencyId">Unique currency identifier</param>
        /// <returns>
        /// (200) OK - If the resource was successfully deleted
        /// (400) Bad Request - If the specified resource data was not valid
        /// (401) Unauthorized - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>DELETE api/v1/logbooks/{logbookId:int}/currencies/{currencyId:int}</remarks>
        [HttpDelete("api/v1/logbooks/{logbookId:int}/currencies/{currencyId:int}", Name = "DeleteCurrencyRoute")]
        public async Task<IActionResult> DeleteCurrency(int logbookId, int currencyId)
        {
            // Check to see if the resource exists
            Currency existingCurrency = await _apiDbContext.Currencies
                .FirstOrDefaultAsync(c => c.CurrencyId == currencyId && c.LogbookId == logbookId);

            // Return not found result
            if (existingCurrency == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingCurrency.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Attempt to delete the resource
            _apiDbContext.Currencies.Remove(existingCurrency);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[DeleteCurrency]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            CurrencyDTO dto = CurrencyDTO.ToDto(existingCurrency);

            // Return result
            return Ok(dto);
        }
    }
}
