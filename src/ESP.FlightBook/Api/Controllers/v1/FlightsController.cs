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
    public class FlightsController : LogbookApiController
    {
        protected readonly ILogger<FlightsController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public FlightsController(ApplicationDbContext apiDbContext, ILogger<FlightsController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns specified Flight resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="page">Returns results from the given page</param>
        /// <param name="itemsPerPage">Determines the number of items per page</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/flights<remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/flights", Name = "GetAllFlightsRoute")]
        public async Task<IActionResult> GetAllFlights(int logbookId, int page = 0, int itemsPerPage = 0, string flightDateStart = null, string flightDateEnd = null, string aircraftIdentifier = null, string aircraftType = null, bool isComplex = false, bool isRetractable = false)
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

            // Construct base query
            var query = from f in _apiDbContext.Flights
                        where f.LogbookId == logbookId
                        select f;

            // Include aircraft and approach objects
            query = query
                .Include(f => f.Aircraft)
                .Include(f => f.Approaches);

            // Apply flight date start
            if (flightDateStart != null && flightDateStart.Length > 0)
            {
                try
                {
                    DateTime startDate = DateTime.Parse(flightDateStart);
                    query = query.Where(f => f.FlightDate >= startDate);
                }
                catch { }
            }

            // Apply flight date end
            if (flightDateEnd != null && flightDateEnd.Length > 0)
            {
                try
                {
                    DateTime endDate = DateTime.Parse(flightDateEnd);
                    query = query.Where(f => f.FlightDate <= endDate);
                }
                catch { }
            }

            // Apply aircraft identifier filter
            if (aircraftIdentifier != null && aircraftIdentifier.Length > 0)
            {
                query = query.Where(f => f.Aircraft.AircraftIdentifier == aircraftIdentifier);
            }

            // Apply aircraft type filter
            if (aircraftType != null && aircraftType.Length > 0)
            {
                query = query.Where(f => f.Aircraft.AircraftType == aircraftType);
            }

            // Apply complex filter
            if (isComplex == true)
            {
                query = query.Where(f => f.Aircraft.IsComplex == true);
            }

            // Apply retractable filter
            if (isRetractable == true)
            {
                string[] retractables = { "Retractable Tailwheel", "Retractable Tricycle" };
                query = query.Where(f => retractables.Contains(f.Aircraft.GearType));
            }

            // Order by flight date
            query = query.OrderByDescending(f => f.FlightDate);

            // Get count of matching rows before paging
            int rowCount = await query.CountAsync();

            // Limit to a specific "page"
            if (itemsPerPage > 0)
            {
                query = query
                    .Skip(page * itemsPerPage)
                    .Take(itemsPerPage);
            }

            // Select items
            List<Flight> flightList = new List<Flight>();
            flightList = await query.ToListAsync();

            // Construct response headers
            Response.Headers.Add("X-eFlightBook-Pagination-Total", rowCount.ToString());
            Response.Headers.Add("X-eFlightBook-Pagination-Limit", itemsPerPage.ToString());
            if (itemsPerPage > 0)
            {
                int totalPages = TotalPageCount(rowCount, itemsPerPage);
                page = GetBestPage(totalPages, page);
                Response.Headers.Add("X-eFlightBook-Pagination-TotalPages", totalPages.ToString());
                Response.Headers.Add("X-eFlightBook-Pagination-Page", page.ToString());
            } else
            {
                Response.Headers.Add("X-eFlightBook-Pagination-TotalPages", "1");
                Response.Headers.Add("X-eFlightBook-Pagination-Page", "0");
            }

            /*
            // Determine how many matching rows we expect
            int rowCount = await _apiDbContext.Flights.CountAsync(f => f.LogbookId == logbookId);
            Response.Headers.Add("X-eFlightBook-Pagination-Total", rowCount.ToString());
            Response.Headers.Add("X-eFlightBook-Pagination-Limit", itemsPerPage.ToString());

            // Create a list to store items retrieved
            List<Flight> flightList = new List<Flight>();

            // Get subset of items if the itemsPerPage value is more than 0,
            // otherwise get all items
            if (rowCount > 0)
            {
                if (itemsPerPage > 0)
                {
                    // Calculate number of pages needed to support the requested items per page and then
                    // adjust the requested page number to align to the total pages available
                    int totalPages = TotalPageCount(rowCount, itemsPerPage);
                    page = GetBestPage(totalPages, page);
                    Response.Headers.Add("X-eFlightBook-Pagination-TotalPages", totalPages.ToString());
                    Response.Headers.Add("X-eFlightBook-Pagination-Page", page.ToString());


                    // Retrieve the requested items
                    flightList = await _apiDbContext.Flights
                        .AsNoTracking()
                        .Include(f => f.Aircraft)
                        .Include(f => f.Approaches)
                        .Where(f => f.LogbookId == logbookId)
                        .OrderByDescending(f => f.FlightDate)
                        .Skip(page * itemsPerPage)
                        .Take(itemsPerPage)
                        .ToListAsync();
                }
                else
                {
                    // Retrieve all items
                    flightList = await _apiDbContext.Flights
                        .AsNoTracking()
                        .Include(f => f.Aircraft)
                        .Include(f => f.Approaches)
                        .Where(f => f.LogbookId == logbookId)
                        .OrderByDescending(f => f.FlightDate)
                        .ToListAsync();
                    Response.Headers.Add("X-eFlightBook-Pagination-TotalPages", "1");
                    Response.Headers.Add("X-eFlightBook-Pagination-Page", "0");
                }
            }
            */

            // Convert to DTO
            List<FlightDTO> dtoList = new List<FlightDTO>();
            foreach (Flight flight in flightList)
            {
                FlightDTO dto = FlightDTO.ToDto(flight);
                if (flight.Aircraft != null)
                {
                    dto.Aircraft = AircraftDTO.ToDto(flight.Aircraft);
                }
                foreach(Approach approach in flight.Approaches)
                {
                    dto.Approaches.Add(ApproachDTO.ToDto(approach));
                }
                dtoList.Add(dto);
            }

            // Return the result
            return Ok(dtoList);
        }

        /// <summary>
        /// Returns specified Flight resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="page">Returns results from the given page</param>
        /// <param name="itemsPerPage">Determines the number of items per page</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (401) Unauthorized - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}/flights<remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}/flights", Name = "GetAllFlightsByAircraftRoute")]
        public async Task<IActionResult> GetAllFlightsByAircraft(int logbookId, int aircraftId, int page = 0, int itemsPerPage = 0)
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

            // Determine how many matching rows we expect
            int rowCount = await _apiDbContext.Flights.CountAsync(f => f.AircraftId == aircraftId && f.LogbookId == logbookId);
            Response.Headers.Add("X-eFlightBook-Pagination-Total", rowCount.ToString());
            Response.Headers.Add("X-eFlightBook-Pagination-Limit", itemsPerPage.ToString());

            // Create a list to store items retrieved
            List<Flight> flightList = new List<Flight>();

            // Get subset of items if the itemsPerPage value is more than 0,
            // otherwise get all items
            if (rowCount > 0)
            {
                if (itemsPerPage > 0)
                {
                    // Calculate number of pages needed to support the requested items per page and then
                    // adjust the requested page number to align to the total pages available
                    int totalPages = TotalPageCount(rowCount, itemsPerPage);
                    page = GetBestPage(totalPages, page);
                    Response.Headers.Add("X-eFlightBook-Pagination-TotalPages", totalPages.ToString());
                    Response.Headers.Add("X-eFlightBook-Pagination-Page", page.ToString());

                    // Retrieve the requested items
                    flightList = await _apiDbContext.Flights
                        .AsNoTracking()
                        .Include(f => f.Aircraft)
                        .Include(f => f.Approaches)
                        .Where(f => f.AircraftId == aircraftId && f.LogbookId == logbookId)
                        .OrderByDescending(f => f.FlightDate)
                        .Skip(page * itemsPerPage)
                        .Take(itemsPerPage)
                        .ToListAsync();
                }
                else
                {
                    // Retrieve all items
                    flightList = await _apiDbContext.Flights
                        .AsNoTracking()
                        .Include(f => f.Aircraft)
                        .Include(f => f.Approaches)
                        .Where(f => f.AircraftId == aircraftId && f.LogbookId == logbookId)
                        .OrderByDescending(f => f.FlightDate)
                        .ToListAsync();
                    Response.Headers.Add("X-eFlightBook-Pagination-TotalPages", "1");
                    Response.Headers.Add("X-eFlightBook-Pagination-Page", "0");

                }
            }

            // Convert to DTO
            List<FlightDTO> dtoList = new List<FlightDTO>();
            foreach (Flight flight in flightList)
            {
                FlightDTO dto = FlightDTO.ToDto(flight);
                if (flight.Aircraft != null)
                {
                    dto.Aircraft = AircraftDTO.ToDto(flight.Aircraft);
                }
                foreach (Approach approach in flight.Approaches)
                {
                    dto.Approaches.Add(ApproachDTO.ToDto(approach));
                }
                dtoList.Add(dto);
            }

            // Return the result
            return Ok(dtoList);
        }

        /// <summary>
        /// Returns specified Flight resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="flightId">Unique flight identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/flight/{flightId:int}</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/flights/{flightId:int}", Name = "GetFlightByIdRoute")]
        public async Task<IActionResult> GetFlight(int logbookId, int flightId)
        {
            // Find specified resources
            Flight flight = await _apiDbContext.Flights
                .AsNoTracking()
                .Include(f => f.Aircraft)
                .Include(f => f.Approaches)
                .FirstOrDefaultAsync(f => f.FlightId == flightId && f.LogbookId == logbookId);

            // Return not found result
            if (flight == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = flight.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Convert to DTO
            FlightDTO dto = FlightDTO.ToDto(flight);
            if (flight.Aircraft != null)
            {
                dto.Aircraft = AircraftDTO.ToDto(flight.Aircraft);
            }
            foreach (Approach approach in flight.Approaches)
            {
                dto.Approaches.Add(ApproachDTO.ToDto(approach));
            }

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Flight resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="flight">The flight to be created</param>
        /// <returns>
        /// (201) Created - If the resource was successfully created
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (409) Conflict - If the resource could not be created
        /// <remarks>POST api/v1/logbooks/{logbookId:int}/flights</remarks>
        [HttpPost("api/v1/logbooks/{logbookId:int}/flights", Name = "PostFlightRoute")]
        public async Task<IActionResult> PostFlight(int logbookId, [FromBody] Flight flight)
        {
            // Validate request data
            if (flight == null)
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
                        _logger.LogInformation("[PostFlight] {0}", message.ErrorMessage);
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
            flight.LogbookId = logbookId;
            flight.UserId = GetUserIdClaim();
            flight.CreatedOn = DateTime.UtcNow;
            flight.ChangedOn = flight.CreatedOn;
            if (flight.Approaches != null)
            {
                foreach (Approach approach in flight.Approaches)
                {
                    approach.ApproachId = 0;
                    approach.UserId = flight.UserId;
                }
            }

            // Attempt to add new resource
            _apiDbContext.Flights.Add(flight);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PostFlight]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            FlightDTO dto = FlightDTO.ToDto(flight);
            if (flight.Approaches != null)
            {
                foreach(Approach approach in flight.Approaches)
                {
                    dto.Approaches.Add(ApproachDTO.ToDto(approach));
                }
            }

            // Return success
            return CreatedAtRoute("GetFlightByIdRoute",
                new
                {
                    logbookId = dto.LogbookId,
                    flightId = dto.FlightId
                }, dto);
        }

        /// <summary>
        /// Updates an existing Flight resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="flightId">Unique flight identifier</param>
        /// <param name="flight">Flight to be udpated</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>PUT api/v1/logbooks/{logbookId:int}/flight/{flightId:int}</remarks>
        [HttpPut("api/v1/logbooks/{logbookId:int}/flights/{flightId:int}", Name = "PutFlightRoute")]
        public async Task<IActionResult> PutFlight(int logbookId, int flightId, [FromBody] Flight flight)
        {
            // Validate request data
            if (flight == null)
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
                        _logger.LogInformation("[PutFlight] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find specified resource
            Flight existingFlight = await _apiDbContext.Flights
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FlightId == flightId && f.LogbookId == logbookId);

            // Return not found result
            if (existingFlight == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingFlight.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Set non-editable attributes
            flight.FlightId = existingFlight.FlightId;
            flight.LogbookId = existingFlight.LogbookId;
            flight.UserId = existingFlight.UserId;
            flight.CreatedOn = existingFlight.CreatedOn;
            flight.ChangedOn = DateTime.UtcNow;
            flight.Aircraft = null;
            if (flight.Approaches != null)
            {
                foreach (Approach approach in flight.Approaches)
                {
                    approach.FlightId = existingFlight.FlightId;
                    approach.UserId = existingFlight.UserId;
                }
            }

            // Attempt to update resource
            _apiDbContext.Entry(flight).State = EntityState.Modified;
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PutFlight]", e);
                return new StatusCodeResult(409);
            }

            // Return the result
            return new NoContentResult();
        }


        /// <summary>
        /// Deletes an existing Flight resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="flightId">Unique flight identifier</param>
        /// <returns>
        /// (200) OK - If the resource was successfully deleted
        /// (400) Bad Request - If the specified resource data was not valid
        /// (401) Unauthorized - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>DELETE api/v1/logbooks/{logbookId:int}/flight/{flightId:int}</remarks>
        [HttpDelete("api/v1/logbooks/{logbookId:int}/flights/{flightId:int}", Name = "DeleteFlightRoute")]
        public async Task<IActionResult> DeleteFlight(int logbookId, int flightId)
        {
            // Check to see if the resource exists
            Flight existingFlight = await _apiDbContext.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Approaches)
                .FirstOrDefaultAsync(f => f.FlightId == flightId && f.LogbookId == logbookId);

            // Return not found result
            if (existingFlight == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingFlight.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Attempt to delete the resource
            _apiDbContext.Flights.Remove(existingFlight);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[DeleteFlight]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            FlightDTO dto = FlightDTO.ToDto(existingFlight);
            if (existingFlight.Aircraft != null)
            {
                dto.Aircraft = AircraftDTO.ToDto(existingFlight.Aircraft);
            }
            foreach (Approach approach in existingFlight.Approaches)
            {
                dto.Approaches.Add(ApproachDTO.ToDto(approach));
            }

            // Return result
            return Ok(dto);
        }
    }
}